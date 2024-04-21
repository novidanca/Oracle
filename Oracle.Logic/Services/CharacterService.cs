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
		return await Db.Characters.ToListAsync();
	}
}