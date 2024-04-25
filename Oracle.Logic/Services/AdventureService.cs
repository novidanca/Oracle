#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class AdventureService(OracleDbContext db) : ServiceBase(db)
{
	#region Adventure Getters

	public async Task<Adventure> GetAdventure(int adventureId)
	{
		return await Db.Adventures.Where(x => x.Id == adventureId)
			.Include(x => x.AdventureCharacters)
			.ThenInclude(x => x.Character)
			.AsSplitQuery()
			.FirstOrDefaultAsync();
	}

	public async Task<List<Adventure>> GetAdventures(List<int> adventureIds)
	{
		return await Db.Adventures.Where(x => adventureIds.Contains(x.Id))
			.Include(x => x.AdventureCharacters)
			.ThenInclude(x => x.Character)
			.AsSplitQuery()
			.ToListAsync();
	}

	#endregion

	public async Task<List<Adventure>> GetRelevantAdventures(int numberOfAdventures = 5)
	{
		var adventureIds = await Db.Adventures.Where(x => !x.IsComplete)
			.OrderByDescending(x => x.StartDay).Take(numberOfAdventures)
			.Select(x => x.Id)
			.ToListAsync();

		return await GetAdventures(adventureIds);
	}

	public async Task<List<Adventure>> SearchAdventures(string query = "", int startDay = 0, int endDay = 0,
		int numberOfResults = 5, int skip = 0)
	{
		var formattedQuery = query.ToLower();

		var adventureIds = await Db.Adventures.Where(x => x.Name.ToLower().Contains(formattedQuery)
		                                                  && x.StartDay >= startDay
		                                                  && (endDay == 0 || x.StartDay + x.Duration <= endDay))
			.Select(x => x.Id)
			.Skip(skip)
			.Take(numberOfResults)
			.ToListAsync();


		return await GetAdventures(adventureIds);
	}

	public async Task AddAdventure(string name, string description)
	{
		var adventure = new Adventure()
		{
			Name = name,
			Description = description
		};

		Db.Adventures.Add(adventure);
		await Db.SaveChangesAsync();
	}

	public async Task<int> GetMaxStartDay()
	{
		return await Db.Adventures.MaxAsync(x => x.StartDay);
	}

	public async Task<Adventure> AddCharacterToAdventure(int adventureId, int characterId)
	{
		var advChar = new AdventureCharacter()
		{
			AdventureId = adventureId,
			CharacterId = characterId
		};

		Db.AdventureCharacters.Add(advChar);
		await Db.SaveChangesAsync();

		return await GetAdventure(adventureId);
	}

	public async Task<bool> RemoveCharacterFromAdventure(int adventureId, int characterId)
	{
		var advChar = await Db.AdventureCharacters.FirstOrDefaultAsync(x => x.CharacterId == characterId &&
		                                                                    x.AdventureId == adventureId);
		if (advChar == null)
			return false;

		Db.AdventureCharacters.Remove(advChar);
		await Db.SaveChangesAsync();

		return true;
	}
}