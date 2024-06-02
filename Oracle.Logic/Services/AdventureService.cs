#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class AdventureService(OracleDbContext db, TimelineService.TimelineService timelineService) : ServiceBase(db)
{
	#region Adventure Getters

	public async Task<Adventure> GetAdventure(int adventureId)
	{
		return await Db.Adventures.Where(x => x.Id == adventureId)
			.Include(x => x.AdventureCharacters)
			.ThenInclude(x => x.Character)
			.AsSplitQuery()
			.FirstAsync();
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

	public async Task<IOutcome> TryStartAdventure(int adventureId, List<int> characterIds)
	{
		var adventure = await Db.Adventures.FirstOrDefaultAsync(x => x.Id == adventureId);

		if (adventure == null)
			return Outcomes.Failure().WithMessage($"No adventure with id {adventureId} exists");

		// Check characters are free that day. If so, add them to the adventure.
		foreach (var character in characterIds)
		{
			var result = await timelineService.AddToCharacterTimeline(adventure, character);
			if (result.Failure)
				return result;
		}

		// Start the adventure
		adventure.IsStarted = true;
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	#endregion


	#region Modification

	public async Task<bool> TryAddAdventureDay(int adventureId)
	{
		// Check if the adventure is started and isn't fixed duration
		var adventure = await GetAdventure(adventureId);

		if (!adventure.IsStarted)
			return false;

		// Check if characters are free on the new day. If so, add the day.
		var newDay = adventure.StartDay + adventure.Duration;
		foreach (var character in adventure.AdventureCharacters)
		{
			var isAvailable = await timelineService.IsCharacterAvailable(character.CharacterId, newDay);
			if (!isAvailable.Success) return false;
		}

		// Add the new day to the adventure
		adventure.Duration++;

		if (adventure.HasFixedDuration)
		{
			var timelineEvents = await Db.CharacterTimelines.Where(x => x.AdventureId == adventureId).ToListAsync();

			foreach (var timelineEvent in timelineEvents) timelineEvent.EndDay = newDay;
		}

		await Db.SaveChangesAsync();

		return true;
	}


	public async Task<IOutcome> TryEndAdventure(int adventureId)
	{
		var adventure = await Db.Adventures.FirstOrDefaultAsync(x => x.Id == adventureId);

		if (adventure == null)
			return Outcomes.Failure().WithMessage($"Adventure {adventureId} does not exist");

		if (!adventure.IsStarted)
			return Outcomes.Failure().WithMessage("Adventure has not started");

		if (adventure.IsComplete)
			return Outcomes.Failure().WithMessage("Adventure is already complete");

		adventure.IsComplete = true;

		if (!adventure.HasFixedDuration)
		{
			var timelineItems = await Db.CharacterTimelines.Where(x => x.AdventureId == adventureId).ToListAsync();

			foreach (var item in timelineItems) item.EndDay = adventure.StartDay + adventure.Duration - 1;
		}

		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task<IOutcome<Adventure>> TryAddCharacterToAdventure(int adventureId, int characterId,
		int? manualStartDay = null)
	{
		// Get the adventure dates
		var adventure = await Db.Adventures.FirstOrDefaultAsync(x => x.Id == adventureId);
		var character = await Db.Characters.FirstOrDefaultAsync(x => x.Id == characterId);

		if (adventure == null)
			return Outcomes.Failure<Adventure>().WithMessage($"Adventure {adventureId} does not exist");

		if (character == null)
			return Outcomes.Failure<Adventure>().WithMessage($"Character {characterId} does not exist");

		if (adventure.IsComplete)
			return Outcomes.Failure<Adventure>().WithMessage("Adventure is already complete");


		// We're just adding the character to a potential adventure, so we don't need to check if the adventure is started
		if (!adventure.IsStarted)
		{
			// Connect the character and adventure
			var advChar = new AdventureCharacter()
			{
				AdventureId = adventureId,
				CharacterId = characterId
			};

			Db.AdventureCharacters.Add(advChar);
			await Db.SaveChangesAsync();
		}
		else
		{
			// Check the character is free within those dates
			var timelineOutcome = await timelineService.AddToCharacterTimeline(adventure, characterId, manualStartDay);

			if (timelineOutcome.Failure)
				return Outcomes.Failure<Adventure>().WithMessagesFrom(timelineOutcome);

			// Connect the character and adventure
			var advChar = new AdventureCharacter()
			{
				AdventureId = adventureId,
				CharacterId = characterId
			};

			Db.AdventureCharacters.Add(advChar);
			await Db.SaveChangesAsync();
		}

		var updatedAdventure = await GetAdventure(adventureId);
		return Outcomes.Success<Adventure>(updatedAdventure);
	}

	public async Task<bool> RemoveCharacterFromAdventure(int adventureId, int characterId)
	{
		var advChar = await Db.AdventureCharacters.FirstOrDefaultAsync(x => x.CharacterId == characterId &&
		                                                                    x.AdventureId == adventureId);
		if (advChar == null)
			return false;

		Db.AdventureCharacters.Remove(advChar);

		// Remove adventure from CharacterTimeline
		var adventure = await GetAdventure(adventureId);
		await timelineService.RemoveFromCharacterTimeline(adventure, characterId);

		await Db.SaveChangesAsync();

		return true;
	}

	#endregion


	#region Helpers

	public async Task<int> GetMaxStartDay()
	{
		var adventures = await Db.Adventures.Select(x => x.StartDay).ToListAsync();

		if (!adventures.Any())
			return 0;

		return adventures.Max();
	}

	#endregion
}