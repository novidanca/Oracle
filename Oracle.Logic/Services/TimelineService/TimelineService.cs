#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services.TimelineService;

public class TimelineService(
	OracleDbContext db,
	TimelineAddService timelineAddService,
	ConditionService conditionService,
	TimelineNotesService noteService)
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

		var timelineDataTask = GetTimeline(ids);
		var conditionDataTask = conditionService.GetConditions(characterIds, startDay, endDay, false);
		var noteDataTask = noteService.GetTimelineNotes(characterIds, startDay, endDay, false);

		await Task.WhenAll(timelineDataTask, conditionDataTask, noteDataTask);

		var timelineData = timelineDataTask.Result;
		var conditionData = conditionDataTask.Result;
		var noteData = noteDataTask.Result;

		return AssembleTimelineVms(characterIds, timelineData, conditionData, noteData, startDay, endDay);
	}

	/// <summary>
	/// Returns a list of Timeline events for many characters that start within the given range
	/// </summary>
	public async Task<List<CharacterTimelineVm>> GetTimelineForManyCharacters(List<int> characterIds, int startDay,
		int endDay)
	{
		var ids = await GetTimelineIds(characterIds, startDay, endDay);

		var timelineDataTask = GetTimeline(ids);
		var conditionDataTask = conditionService.GetConditions(characterIds, startDay, endDay, false);
		var noteDataTask = noteService.GetTimelineNotes(characterIds, startDay, endDay, false);

		await Task.WhenAll(timelineDataTask, conditionDataTask, noteDataTask);

		var timelineData = timelineDataTask.Result;
		var conditionData = conditionDataTask.Result;
		var noteData = noteDataTask.Result;

		return AssembleTimelineVms(characterIds, timelineData, conditionData, noteData, startDay, endDay);
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
		}

		return null;
	}

	#endregion

	#region Checks

	public async Task<IOutcome> IsCharacterAvailable(int characterId, int startDay, int? endDay = null)
	{
		var character = await Db.Characters.FindAsync(characterId);
		if (character == null) return Outcomes.Failure().WithMessage($"Character with ID {characterId} not found.");

		var blockingCondition = await GetBlockingCondition(characterId, startDay, endDay);
		if (blockingCondition != null) return GetBlockingConditionOutcome(character.Name, blockingCondition);

		var blockingTimeline = await GetBlockingTimeline(characterId, startDay, endDay);
		if (blockingTimeline != null) return GetBlockingTimelineOutcome(character.Name, blockingTimeline);

		return Outcomes.Success();
	}

	private async Task<CharacterCondition?> GetBlockingCondition(int characterId, int startDay, int? endDay)
	{
		var query = Db.CharacterConditions.Where(x => x.CharacterId == characterId).AsNoTracking();

		if (endDay.HasValue)
			return await query.FirstOrDefaultAsync(x =>
				x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));
		else
			return await query.FirstOrDefaultAsync(x => x.StartDay <= startDay && x.EndDay == null)
			       ?? await query.FirstOrDefaultAsync(x =>
				       x.EndDay != null && x.StartDay <= startDay && x.EndDay >= startDay);
	}

	private async Task<CharacterTimeline?> GetBlockingTimeline(int characterId, int startDay, int? endDay)
	{
		var query = Db.CharacterTimelines.Where(x => x.CharacterId == characterId).AsNoTracking();

		if (endDay.HasValue)
			return await query.FirstOrDefaultAsync(x =>
				x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));
		else
			return await query.FirstOrDefaultAsync(x => x.StartDay <= startDay && x.EndDay == null)
			       ?? await query.FirstOrDefaultAsync(x =>
				       x.EndDay != null && x.StartDay <= startDay && x.EndDay >= startDay);
	}

	private IOutcome GetBlockingConditionOutcome(string characterName, CharacterCondition blockingCondition)
	{
		var description = blockingCondition.Description;

		return Outcomes.Failure()
			.WithMessage(
				$"{characterName} is blocked from activities by a condition: {description}.");
	}

	private IOutcome GetBlockingTimelineOutcome(string characterName, CharacterTimeline blockingTimeline)
	{
		var itemType = blockingTimeline.Adventure != null ? "adventure" : "activity";

		var description = blockingTimeline.Adventure != null
			? blockingTimeline.Adventure.Name
			: blockingTimeline.Activity?.ActivityType.Name;

		return Outcomes.Failure()
			.WithMessage(
				$"{characterName} is occupied by {itemType} {description}.");
	}

	#endregion

	#region Adders

	/// <summary>
	/// Add an adventure to a character's timeline. Optionally 
	/// </summary>
	public async Task<IOutcome> AddToCharacterTimeline(Adventure adventure, int characterId, int? manualStartDay = null)
	{
		var characterAvailable = adventure.HasFixedDuration
			? await IsCharacterAvailable(characterId, manualStartDay ?? adventure.StartDay,
				adventure.StartDay + adventure.Duration - 1)
			: await IsCharacterAvailable(characterId, manualStartDay ?? adventure.StartDay);

		if (characterAvailable.Failure)
			return characterAvailable;

		return await timelineAddService.AddToCharacterTimeline(adventure, characterId, manualStartDay);
	}

	public async Task<IOutcome> AddToCharacterTimeline(Activity activity, int characterId)
	{
		var characterAvailable = await IsCharacterAvailable(characterId, activity.StartDate);

		if (characterAvailable.Failure)
			return characterAvailable;
		return await timelineAddService.AddToCharacterTimeline(activity, characterId);
	}

	#endregion

	#region VM Assembly

	public static List<CharacterTimelineVm> AssembleTimelineVms(List<int> characterIds,
		List<CharacterTimeline> timelineData,
		List<CharacterCondition> conditionData, List<TimelineNote> noteData, int startDate, int endDate)
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
			characterTimeline.Conditions = conditionData
				.Where(x => x.CharacterId == characterId)
				.Select(x => new ConditionVm()
				{
					StatusId = x.Id,
					CharacterId = x.CharacterId,
					Description = x.Description,
					StartDate = x.StartDay,
					EndDate = x.EndDay
				})
				.ToList();

			characterTimeline.Notes = noteData
				.Where(x => x.CharacterId != null && x.CharacterId == characterId)
				.Select(x => new NoteVm()
				{
					NoteId = x.Id,
					CharacterId = (int)x.CharacterId!,
					Note = x.Note,
					StartDate = x.StartDate,
					EndDate = x.EndDate
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