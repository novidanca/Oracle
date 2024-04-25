﻿#region using

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

	#endregion

	#region Creation

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

	public async Task<bool> TryStartAdventure(int adventureId, int startDay, List<int> characterIds)
	{
		var adventure = await Db.Adventures.FirstOrDefaultAsync(x => x.Id == adventureId);

		// Check characters are free that day. If so, add them to the adventure.


		// Start the adventure

		return false;
	}

	#endregion

	#region Modification

	public async Task<bool> TryAddAdventureDay(int adventureId)
	{
		// Check adventure is started

		// Check characters are free on the new day. If so, add the day.

		return false;
	}


	public async Task<bool> TryEndAdventure(int adventureId)
	{
		// Check adventure is started

		// End Adventure
		// Set Complete = true

		// For each character, get the adventure on the Timeline and set the end date

		return false;
	}

	#endregion


	public async Task<int> GetMaxStartDay()
	{
		return await Db.Adventures.MaxAsync(x => x.StartDay);
	}

	public async Task<Adventure> TryAddCharacterToAdventure(int adventureId, int characterId)
	{
		// Get the adventure dates

		// Check the character is free within those dates

		// Connect the character and adventure
		var advChar = new AdventureCharacter()
		{
			AdventureId = adventureId,
			CharacterId = characterId
		};

		// Add the adventure to the timeline

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

		// Remove adventure from CharacterTimeline

		Db.AdventureCharacters.Remove(advChar);

		await Db.SaveChangesAsync();

		return true;
	}
}