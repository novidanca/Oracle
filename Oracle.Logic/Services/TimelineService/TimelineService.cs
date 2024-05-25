#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services.TimelineService;

public class TimelineService(OracleDbContext db, TimelineAddService timelineAddService, StatusService statusService)
	: ServiceBase(db)
{
	#region Getters

	/// <summary>
	/// Returns a list of Timeline events for a character that start within the given range
	/// </summary>
	public async Task<List<CharacterTimelineVm>> GetTimelineForCharacter(int characterId, int startDay, int endDay)
	{
		var characterIds = new List<int> { characterId };

		var ids = await GetTimelineIds(characterIds, startDay, endDay);
		var timelineData = await GetTimeline(ids);
		var statusData = await statusService.GetStatuses(characterId, startDay, endDay);


		return AssembleTimelineVms(characterIds, timelineData, statusData, startDay, endDay);
	}

	/// <summary>
	/// Returns a list of Timeline events for many characters that start within the given range
	/// </summary>
	public async Task<List<CharacterTimelineVm>> GetTimelineForManyCharacters(List<int> characterIds, int startDay,
		int endDay)
	{
		var ids = await GetTimelineIds(characterIds, startDay, endDay);

		var timelineData = await GetTimeline(ids);
		var statusData = await statusService.GetStatuses(characterIds, startDay, endDay);

		return AssembleTimelineVms(characterIds, timelineData, statusData, startDay, endDay);
	}

	private async Task<List<int>> GetTimelineIds(List<int> characterIds, int startDay, int endDay)
	{
		return await Db.CharacterTimelines
			.Where(x => characterIds.Contains(x.CharacterId) &&
			            (
				            // Timeline starts or ends within the specified range.
				            (x.StartDay >= startDay && x.StartDay <= endDay) ||
				            (x.EndDay >= startDay && x.EndDay <= endDay) ||
				            // Timeline starts within the specified range and has no end day.
				            (x.StartDay >= startDay && x.StartDay <= endDay && x.EndDay == null) ||
				            // Timeline started before the end day and has no end day.
				            (x.StartDay < endDay && x.EndDay == null)
			            )
			)
			.AsNoTracking()
			.Select(x => x.Id)
			.ToListAsync();
	}


	private async Task<List<CharacterTimeline>> GetTimeline(List<int> timelineIds)
	{
		return await Db.CharacterTimelines.Where(x => timelineIds.Contains(x.Id))
			.Include(x => x.Character)
			.Include(x => x.Adventure)
			.Include(x => x.Activity)
			.ThenInclude(x => x.ActivityType)
			.Include(x => x.Status)
			.AsSplitQuery()
			.AsNoTracking()
			.ToListAsync();
	}


	private async Task<CharacterTimeline> GetTimeline(int timelineId)
	{
		return await Db.CharacterTimelines.Where(x => x.Id == timelineId)
			.Include(x => x.Character)
			.Include(x => x.Adventure)
			.Include(x => x.Activity)
			.ThenInclude(x => x.ActivityType)
			.Include(x => x.Status)
			.AsSplitQuery()
			.AsNoTracking()
			.FirstAsync();
	}

	public async Task<int?> GetConnectedEntityId(int timelineId)
	{
		var timeline = await Db.CharacterTimelines.FindAsync(timelineId);

		if (timeline != null)
		{
			if (timeline.AdventureId != null)
				return timeline.AdventureId;
			else if (timeline.ActivityId != null)
				return timeline.ActivityId;
			else if (timeline.CharacterStatusId != null) return timeline.CharacterStatusId;
		}

		return null;
	}

	#endregion

	#region Checks

	public async Task<IOutcome> IsCharacterAvailable(int characterId, int startDay, int? endDay = null)
	{
		var character = Db.Characters.First(x => x.Id == characterId);
		CharacterTimeline? blockingTimeline = null;

		if (endDay.HasValue)
		{
			blockingTimeline = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));
		}
		else
		{
			var query = Db.CharacterTimelines.Where(x => x.CharacterId == characterId).AsNoTracking();

			// Check for any endless events, then any events that end on or after the proposed start day
			blockingTimeline = await query.FirstOrDefaultAsync(x => x.EndDay == null)
			                   ?? await query.FirstOrDefaultAsync(x => x.EndDay != null && x.EndDay >= startDay);
		}


		if (blockingTimeline == null)
			return Outcomes.Success();

		var blockingActivity = await GetTimeline(blockingTimeline.Id);
		var activityType = blockingActivity.Adventure != null ? "adventure" :
			blockingActivity.Activity != null ? "activity" :
			blockingActivity.Status != null ? "status" :
			"";
		var description = blockingActivity.Adventure?.Name ?? blockingActivity.Activity?.ActivityType.Name ??
			blockingActivity.Status?.Description ?? "";

		return endDay.HasValue
			? Outcomes.Failure().WithMessage($"{character.Name} is not available on between {startDay} and {endDay}")
			: blockingActivity.EndDay == null
				? Outcomes.Failure()
					.WithMessage(
						$"{character.Name} cannot be added to an adventure with no Fixed Duration until until {activityType} {description} is complete")
				: Outcomes.Failure()
					.WithMessage(
						$"{character.Name} is occupied by {activityType} {description} that ends on or after this adventure's start day");
	}

	#endregion

	#region Adders

	/// <summary>
	/// Add an adventure to a character's timeline. Optionally 
	/// </summary>
	public async Task<IOutcome> AddToCharacterTimeline(Adventure adventure, int characterId, int? manualStartDay = null)
	{
		var characterAvailable = adventure.HasFixedDuration
			? await IsCharacterAvailable(characterId, adventure.StartDay, adventure.StartDay + adventure.Duration - 1)
			: await IsCharacterAvailable(characterId, adventure.StartDay);

		if (characterAvailable.Failure)
			return characterAvailable;

		return await timelineAddService.AddToCharacterTimeline(adventure, characterId, manualStartDay);
	}

	public async Task<IOutcome> AddToCharacterTimeline(Activity activity, int characterId)
	{
		var characterAvailable = await IsCharacterAvailable(characterId, activity.Date);

		if (characterAvailable.Failure)
			return characterAvailable;
		return await timelineAddService.AddToCharacterTimeline(activity, characterId);
	}

	#endregion

	#region VM Assembly

	public static List<CharacterTimelineVm> AssembleTimelineVms(List<int> characterIds,
		List<CharacterTimeline> timelineData,
		List<CharacterStatus> statusData, int startDate, int endDate)
	{
		List<CharacterTimelineVm> characterTimelines = [];

		foreach (var characterId in characterIds)
		{
			var characterTimeline = new CharacterTimelineVm()
			{
				CharacterId = characterId
			};

			List<TimelineDateVm> timeline = [];


			var timelineEvents =
				timelineData.Where(x => x.CharacterId == characterId).OrderBy(x => x.StartDay).ToList();


			foreach (var timelineEvent in timelineEvents)
			{
				var newVm = new TimelineDateVm()
				{
					TimelineId = timelineEvent.Id,
					CharacterId = characterId,
					StartDate = timelineEvent.StartDay,
					EndDate = timelineEvent.EndDay
				};

				if (timelineEvent.Adventure != null)
				{
					newVm.Type = TimelineEntityTypes.Adventure;
					newVm.Description = timelineEvent.Adventure.Name;
					newVm.EntityLink = $"/adventureDetail/{timelineEvent.AdventureId}";
					newVm.IsComplete = timelineEvent.Adventure.IsComplete;
				}
				else if (timelineEvent.Activity != null)
				{
					newVm.Type = TimelineEntityTypes.Activity;
					newVm.Description = timelineEvent.Activity.ActivityType.Name;
				}

				timeline.Add(newVm);
			}

			characterTimeline.Timeline = timeline;
			characterTimeline.Statuses = statusData
				.Where(x => x.CharacterId == characterId)
				.Select(x => new StatusVm()
				{
					StatusId = x.Id,
					CharacterId = x.CharacterId,
					Description = x.Description,
					CanQuest = x.CanQuest,
					StartDate = x.StartDay,
					EndDate = x.EndDay
				})
				.ToList();

			characterTimelines.Add(characterTimeline);
		}

		return characterTimelines;
	}

	#endregion

	#region Removers

	public async Task<IOutcome> RemoveFromCharacterTimeline(Activity activity, int characterId)
	{
		var timelineEntry =
			await Db.CharacterTimelines.FirstOrDefaultAsync(x =>
				x.ActivityId == activity.Id && x.CharacterId == characterId);

		if (timelineEntry == null)
			return Outcomes.Failure()
				.WithMessage(
					$"No matching timeline entry found for activity {activity.Id} and character {characterId}");

		Db.CharacterTimelines.Remove(timelineEntry);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task<IOutcome> RemoveFromCharacterTimeline(Adventure adventure, int characterId)
	{
		var timelineEntry =
			await Db.CharacterTimelines.FirstOrDefaultAsync(x =>
				x.AdventureId == adventure.Id && x.CharacterId == characterId);

		if (timelineEntry == null)
			return Outcomes.Failure()
				.WithMessage(
					$"No matching timeline entry found for adventure {adventure.Id} and character {characterId}");

		Db.CharacterTimelines.Remove(timelineEntry);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task<IOutcome> RemoveFromCharacterTimeline(CharacterStatus status, int characterId)
	{
		var timelineEntry =
			await Db.CharacterTimelines.FirstOrDefaultAsync(x =>
				x.CharacterStatusId == status.Id && x.CharacterId == characterId);

		if (timelineEntry == null)
			return Outcomes.Failure()
				.WithMessage($"No matching timeline entry found for status {status.Id} and character {characterId}");

		Db.CharacterTimelines.Remove(timelineEntry);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	#endregion

	#region Helpers

	public async Task<int> GetMaxTimelineDate()
	{
		var lastEndDate = await Db.CharacterTimelines
			.Where(x => x.EndDay != null)
			.OrderByDescending(x => x.EndDay)
			.Select(x => x.EndDay)
			.FirstOrDefaultAsync();

		if (lastEndDate != null)
			return lastEndDate.Value;

		return 0;
	}

	#endregion
}