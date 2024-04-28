#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;
using System.Diagnostics;


#endregion

namespace Oracle.Logic.Services;

public class TimelineService(OracleDbContext db) : ServiceBase(db)
{
	#region Getters

	public async Task<CharacterTimeline> GetCharacterTimeline(int timelineId)
	{
		return await Db.CharacterTimelines.Where(x => x.Id == timelineId)
			.Include(x => x.Character)
			.Include(x => x.Adventure)
			.Include(x => x.Activity)
			.Include(x => x.Status)
			.AsSplitQuery()
			.FirstAsync();
	}

	#endregion

	#region Checks

	public async Task<IOutcome> IsCharacterAvailable(int characterId, int startDay, int? endDay = null)
	{
		var character = Db.Characters.First(x => x.Id == characterId);
		CharacterTimeline? blockingActivity;
		
		if (endDay.HasValue)
		{
			blockingActivity = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
				.FirstOrDefaultAsync(x => x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));
		}
		else
		{
			blockingActivity = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
				.FirstOrDefaultAsync(x => x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));
		}
		
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
				&& startDay >= x.StartDay && (x.EndDay == null || startDay <= x.EndDay)).Id;

		return await GetCharacterTimeline(timelineId);
	}

	#endregion
	

	public async Task<IOutcome> AddToCharacterTimeline(Adventure adventure, int characterId)
	{
		var characterAvailable = await IsCharacterAvailable(characterId, adventure.StartDay, adventure.StartDay + adventure.Duration);

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

			if (blockingTimeline != null)
			{
				blockingTimeline.EndDay = status.StartDay - 1;
			}
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


	//public async Task<List<Character>> GetAllAvailableCharacters(int targetDay,
	//	CharacterLoadOptions? loadOptions = null)
	//{
	//	List<int> unavailableIds = [];
	//
	//	// Get all characters with an activity that day
	//	unavailableIds.AddRange(await Db.Activities.Where(x => x.Date == targetDay)
	//		.AsNoTracking()
	//		.AsSplitQuery()
	//		.Select(x => x.Character.Id)
	//		.ToListAsync());
	//
	//
	//	//Get all character Ids of those on incomplete quests
	//	unavailableIds.AddRange(await Db.Characters
	//		.Where(x => !unavailableIds.Contains(x.Id))
	//		.Include(x => x.AdventureCharacters)
	//		.ThenInclude(x => x.Adventure)
	//		.Where(x => x.AdventureCharacters.Any(y => !y.Adventure.IsComplete && y.Adventure.IsStarted))
	//		.AsNoTracking()
	//		.AsSplitQuery()
	//		.Select(x => x.Id)
	//		.ToListAsync());
	//
	//	// Get characters on a quest that day
	//	unavailableIds.AddRange(await Db.Characters
	//		.Where(x => !unavailableIds.Contains(x.Id))
	//		.Include(x => x.AdventureCharacters)
	//		.ThenInclude(x => x.Adventure)
	//		.Where(x => x.AdventureCharacters.Any(y =>
	//			y.Adventure.IsStarted
	//			&& y.Adventure.IsComplete
	//			&& targetDay >= y.Adventure.StartDay
	//			&& targetDay <= y.Adventure.StartDay + y.Adventure.Duration))
	//		.AsNoTracking()
	//		.AsSplitQuery()
	//		.Select(x => x.Id).ToListAsync());
	//
	//	// Get all the ids of characters who are available
	//	var loadIds = await Db.Characters.Where(x => !unavailableIds.Contains(x.Id))
	//		.AsNoTracking()
	//		.Select(x => x.Id)
	//		.ToListAsync();
	//
	//	return await GetCharacters(loadIds, loadOptions);
	//}
}

public enum TimelineEntityTypes
{
	Adventure,
	Activity,
	BlockingStatus
}