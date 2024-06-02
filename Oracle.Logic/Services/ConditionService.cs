#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class ConditionService(OracleDbContext db) : ServiceBase(db)
{
	public async Task<IOutcome> AddCondition(int characterId, ConditionOptions statusOptions)
	{
		var status = new CharacterCondition()
		{
			Description = statusOptions.Description,
			StartDay = statusOptions.StartDay,
			EndDay = statusOptions.EndDay,
			CharacterId = characterId
		};

		Db.CharacterConditions.Add(status);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task RemoveCondition(int statusId)
	{
		var status = await Db.CharacterConditions.FindAsync(statusId);

		if (status != null)
		{
			Db.CharacterConditions.Remove(status);
			await Db.SaveChangesAsync();
		}
	}

	public async Task<List<CharacterCondition>> GetConditions(int characterId, int startDay, int endDay,
		bool trackEntities = true)
	{
		var query = Db.CharacterConditions.Where(x => x.CharacterId == characterId
		                                              && ((x.StartDay >= startDay && x.StartDay <= endDay)
		                                                  || (x.EndDay >= startDay && x.EndDay <= endDay)
		                                                  || (x.StartDay < endDay && x.EndDay == null)))
			.Include(x => x.Character);

		if (!trackEntities) query.AsNoTracking();

		return await query.ToListAsync();
	}

	public async Task<List<CharacterCondition>> GetConditions(List<int> characterIds, int startDay, int endDay,
		bool trackEntities = true)
	{
		var query = Db.CharacterConditions.Where(x => characterIds.Contains(x.CharacterId)
		                                              && ((x.StartDay >= startDay && x.StartDay <= endDay)
		                                                  || (x.EndDay >= startDay && x.EndDay <= endDay)
		                                                  || (x.StartDay < endDay && x.EndDay == null)))
			.Include(x => x.Character);

		if (!trackEntities) query.AsNoTracking();

		return await query.ToListAsync();
	}
}

public class ConditionOptions(string description, int startDay, int? endDay = null)
{
	public string Description = description;
	public int StartDay = startDay;
	public int? EndDay = endDay;
}