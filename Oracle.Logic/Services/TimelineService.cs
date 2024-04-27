#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Core.Outcomes;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class TimelineService(OracleDbContext db) : ServiceBase(db)
{
	public async Task<IOutcome> IsCharacterAvailable(int characterId, int day)
	{
		var activity = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
			.Select(x => new {x.StartDay, x.EndDay, x.Character.Name})
			.AsNoTracking()
			.FirstOrDefaultAsync(x => day >= x.StartDay && (x.EndDay == null || day <= x.EndDay));

		if (activity == null)
			return Outcomes.Success();

		return Outcomes.Failure().WithMessage($"{activity.Name} is not available on day {day}");
	}

	public async Task<IOutcome> IsCharacterAvailable(int characterId, int startDay, int endDay)
	{
		var activity = await Db.CharacterTimelines.Where(x => x.CharacterId == characterId)
			.Select(x => new {x.StartDay, x.EndDay, x.Character.Name})
			.FirstOrDefaultAsync(x => x.StartDay <= endDay && (x.EndDay == null || x.EndDay >= startDay));

		if (activity == null)
			return Outcomes.Success();

		return Outcomes.Failure().WithMessage($"{activity.Name} is not available on between {startDay} and {endDay}");
	}


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