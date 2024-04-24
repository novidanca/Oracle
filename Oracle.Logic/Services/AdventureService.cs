#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class AdventureService(OracleDbContext db) : ServiceBase(db)
{
	public async Task<List<Adventure>> GetRelevantAdventures(int numberOfAdventures = 5)
	{
		return await Db.Adventures.Where(x => !x.IsComplete)
			.Include(x => x.AdventureCharacters)
			.ThenInclude(x => x.Character)
			.OrderByDescending(x => x.StartDay).Take(numberOfAdventures)
			.AsSplitQuery()
			.ToListAsync();
	}

	public async Task<List<Adventure>> SearchAdventures(string query = "", int startDay = 0, int endDay = 0,
		int numberOfResults = 5, int skip = 0)
	{
		var formattedQuery = query.ToLower();
		return await Db.Adventures.Where(x => x.Name.ToLower().Contains(formattedQuery)
		                                      && x.StartDay >= startDay
		                                      && (endDay == 0 || x.StartDay + x.Duration <= endDay))
			.Skip(skip)
			.Take(numberOfResults)
			.Include(x => x.AdventureCharacters)
			.ThenInclude(x => x.Character)
			.AsSplitQuery()
			.ToListAsync();
	}

	public async Task<Adventure> GetAdventure(int adventureId)
	{
		return await Db.Adventures.Where(x => x.Id == adventureId)
			.Include(x => x.AdventureCharacters)
			.ThenInclude(x => x.Character)
			.AsSplitQuery()
			.FirstOrDefaultAsync();
	}

	public async Task AddAdventure(string name, string description, List<Character>? characters = null)
	{
		var adventure = new Adventure()
		{
			Name = name,
			Description = description
		};

		Db.Adventures.Add(adventure);
		await Db.SaveChangesAsync();

		if (characters != null)
		{
			foreach (var character in characters)
			{
				var adventureCharacter = new AdventureCharacter()
				{
					CharacterId = character.Id,
					AdventureId = adventure.Id
				};
				Db.AdventureCharacters.Add(adventureCharacter);
			}

			await Db.SaveChangesAsync();
		}
	}

	public async Task<int> GetMaxStartDay()
	{
		return await Db.Adventures.MaxAsync(x => x.StartDay);
	}
}