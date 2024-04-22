#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class CharacterService(OracleDbContext db) : ServiceBase(db)
{
	public async Task<List<Character>> GetAllCharacters()
	{
		return await Db.Characters.Include(x => x.Player).ToListAsync();
	}

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

	public async Task DeleteCharacter(Character character)
	{
		if (Db.Entry(character).State == EntityState.Detached)
			Db.Characters.Attach(character);

		Db.Characters.Remove(character);
		await Db.SaveChangesAsync();
	}
}