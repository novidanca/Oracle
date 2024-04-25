#region using

using Oracle.Data;

#endregion

namespace Oracle.Logic.Services;

public class TimelineService(OracleDbContext db) : ServiceBase(db)
{
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