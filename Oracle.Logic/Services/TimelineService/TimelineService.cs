#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services.TimelineService;

public class TimelineService(OracleDbContext db) : ServiceBase(db)
{
	#region Getters

	/// <summary>
	/// Returns a list of Timeline events for a character that start within the given range
	/// </summary>
	public async Task<List<CharacterTimelineVm>> GetTimelineForCharacter(int characterId, int startDay, int endDay)
	{
		var ids = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId
		                                                 && x.StartDay >= startDay && x.StartDay <= endDay)
			.AsNoTracking()
			.Select(x => x.Id).ToListAsync();

		var timelineData = await GetTimeline(ids);
		var characterIds = new List<int> { characterId };

		return AssembleTimelineVms(characterIds, timelineData, startDay, endDay);
	}

	/// <summary>
	/// Returns a list of Timeline events for many characters that start within the given range
	/// </summary>
	public async Task<List<CharacterTimelineVm>> GetTimelineForManyCharacters(List<int> characterIds, int startDay,
		int endDay)
	{
		var ids = await Db.CharacterTimelines.Where(x => characterIds.Contains(x.CharacterId)
		                                                 && x.StartDay >= startDay && x.StartDay <= endDay)
			.AsNoTracking()
			.Select(x => x.Id).ToListAsync();

		var timelineData = await GetTimeline(ids);

		return AssembleTimelineVms(characterIds, timelineData, startDay, endDay);
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

	#endregion

	#region Checks

	public async Task<IOutcome> IsCharacterAvailable(int characterId, int startDay, int? endDay = null)
	{
		var character = Db.Characters.First(x => x.Id == characterId);
		CharacterTimeline? blockingActivity;

		if (endDay.HasValue)
			blockingActivity = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
				.FirstOrDefaultAsync(x => x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));
		else
			blockingActivity = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
				.FirstOrDefaultAsync(x => x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));

		if (blockingActivity == null)
			return Outcomes.Success();

		return endDay.HasValue
			? Outcomes.Failure().WithMessage($"{character.Name} is not available on between {startDay} and {endDay}")
			: Outcomes.Failure().WithMessage($"{character.Name} is not available on day {startDay}");
	}

	public async Task<CharacterTimeline?> GetBlockingTimelineEvent(int characterId, int startDay, int? endDay = null)
	{
		var characterAvailable = endDay.HasValue
			? await IsCharacterAvailable(characterId, startDay, endDay.Value)
			: await IsCharacterAvailable(characterId, startDay);

		if (characterAvailable.Success)
			return null;

		var timelineId = endDay.HasValue
			? Db.CharacterTimelines.First(x => x.Id == characterId
			                                   && x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay)).Id
			: Db.CharacterTimelines.First(x => x.Id == characterId
			                                   && startDay >= x.StartDay && (x.EndDay == null || startDay <= x.EndDay))
				.Id;

		return await GetTimeline(timelineId);
	}

	#endregion

	#region Adders

	public async Task<IOutcome> AddToCharacterTimeline(Adventure adventure, int characterId)
	{
		var characterAvailable =
			await IsCharacterAvailable(characterId, adventure.StartDay, adventure.StartDay + adventure.Duration);

		if (characterAvailable.Failure)
			return characterAvailable;

		var timeline = new CharacterTimeline()
		{
			AdventureId = adventure.Id,
			CharacterId = characterId,
			StartDay = adventure.StartDay,
			EndDay = adventure.IsComplete ? adventure.StartDay + adventure.Duration : null
		};

		Db.CharacterTimelines.Add(timeline);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task<IOutcome> AddToCharacterTimeline(Activity activity, int characterId)
	{
		var characterAvailable = await IsCharacterAvailable(characterId, activity.Date);

		if (characterAvailable.Failure)
			return characterAvailable;

		var timeline = new CharacterTimeline()
		{
			ActivityId = activity.Id,
			CharacterId = characterId,
			StartDay = activity.Date,
			EndDay = activity.Date
		};

		Db.CharacterTimelines.Add(timeline);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task<IOutcome> AddToCharacterTimeline(CharacterStatus status, int characterId)
	{
		if (status.CanQuest)
			return Outcomes.Failure().WithMessage("Cannot add a non-blocking status to the timeline.");

		var characterAvailable = await IsCharacterAvailable(characterId, status.StartDay, status.EndDay);

		if (characterAvailable.Failure)
		{
			var blockingTimeline = await GetBlockingTimelineEvent(characterId, status.StartDay, status.EndDay);
			// We have a blockingTimeline event. It's not null here.

			if (blockingTimeline != null) blockingTimeline.EndDay = status.StartDay - 1;
		}

		var timeline = new CharacterTimeline()
		{
			CharacterId = characterId,
			StartDay = status.StartDay,
			EndDay = status.EndDay
		};

		Db.CharacterTimelines.Add(timeline);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	#endregion

	public static List<CharacterTimelineVm> AssembleTimelineVms(List<int> characterIds,
		List<CharacterTimeline> timelineData,
		int startDate, int endDate)
	{
		List<CharacterTimelineVm> characterTimelines = [];

		foreach (var characterId in characterIds)
		{
			var characterTimeline = new CharacterTimelineVm()
			{
				CharacterId = characterId
			};

			List<TimelineDateVm> timeline = [];

			for (var i = startDate; i <= endDate; i++)
			{
				var newVm = new TimelineDateVm()
				{
					CharacterId = characterId,
					Date = i
				};

				var timelineEvent = timelineData.FirstOrDefault(x => x.CharacterId == characterId
				                                                     && i >= x.StartDay &&
				                                                     (x.EndDay == null || i <= x.EndDay));

				if (timelineEvent != null)
				{
					newVm.TimelineId = timelineEvent.Id;

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
					else if (timelineEvent.Status != null)
					{
						newVm.Type = TimelineEntityTypes.BlockingStatus;
						newVm.Description = timelineEvent.Status.Description;
						newVm.IsComplete = timelineEvent.Status.EndDay.HasValue;
					}
				}

				timeline.Add(newVm);
			}

			characterTimeline.Timeline = timeline;
			characterTimelines.Add(characterTimeline);
		}

		return characterTimelines;
	}
}