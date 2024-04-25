#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class CharacterService(OracleDbContext db) : ServiceBase(db)
{
	#region Getters

	public async Task<Character> GetCharacter(int characterId, CharacterLoadOptions? loadOptions = null)
	{
		var character = Db.Characters.Where(x => x.Id == characterId);

		if (loadOptions == null)
			return await character.FirstAsync();

		if (loadOptions.GetPlayer)
			character.Include(x => x.Player);
		if (loadOptions.GetActivities)
			character.Include(x => x.Activities);
		if (loadOptions.GetAdventures)
			character.Include(x => x.AdventureCharacters)
				.ThenInclude(x => x.Adventure);
		if (loadOptions.GetProjects)
			character.Include(x => x.Projects);

		return await character.AsSplitQuery().FirstAsync();
	}

	public async Task<List<Character>> GetCharacters(List<int> characterIds, CharacterLoadOptions? loadOptions = null)
	{
		var characters = Db.Characters.Where(x => characterIds.Contains(x.Id));

		if (loadOptions == null)
			return await characters.ToListAsync();

		if (loadOptions.GetPlayer)
			characters.Include(x => x.Player);
		if (loadOptions.GetActivities)
			characters.Include(x => x.Activities);
		if (loadOptions.GetAdventures)
			characters.Include(x => x.AdventureCharacters)
				.ThenInclude(x => x.Adventure);
		if (loadOptions.GetProjects)
			characters.Include(x => x.Projects);

		return await characters.AsSplitQuery().ToListAsync();
	}

	public async Task<List<Character>> GetAllCharacters(CharacterLoadOptions? loadOptions = null)
	{
		var ids = await Db.Characters.AsNoTracking().Select(x => x.Id).ToListAsync();
		return await GetCharacters(ids, loadOptions);
	}

	#endregion

	#region Create

	public async Task MakeNewCharacter(string name, Player? owningPlayer = null)
	{
		if (owningPlayer != null && Db.Entry(owningPlayer).State == EntityState.Detached)
			Db.Players.Attach(owningPlayer);

		var character = new Character()
		{
			Name = name,
			Player = owningPlayer
		};

		Db.Characters.Add(character);

		try
		{
			await Db.SaveChangesAsync();
		}
		catch (DbUpdateException ex)
		{
			// Log the error and the inner exception
			Console.WriteLine("An error occurred while updating the database: " + ex.Message);
			if (ex.InnerException != null) Console.WriteLine("Inner exception: " + ex.InnerException.Message);

			// Optionally rethrow the exception or handle it based on your use case
			throw;
		}
	}

	#endregion

	#region Modify

	public async Task DeleteCharacter(Character character)
	{
		if (Db.Entry(character).State == EntityState.Detached)
			Db.Characters.Attach(character);

		Db.Characters.Remove(character);
		await Db.SaveChangesAsync();
	}

	#endregion
}

public class CharacterLoadOptions(bool loadAll = false)
{
	public bool GetPlayer
	{
		get => loadAll || _getPlayer;
		set => _getPlayer = value;
	}

	private bool _getPlayer;

	public bool GetAdventures
	{
		get => loadAll || _getAdventures;
		set => _getAdventures = value;
	}

	private bool _getAdventures;

	public bool GetActivities
	{
		get => loadAll || _getActivities;
		set => _getActivities = value;
	}

	private bool _getActivities;

	public bool GetProjects
	{
		get => loadAll || _getProjects;
		set => _getProjects = value;
	}

	private bool _getProjects;
}