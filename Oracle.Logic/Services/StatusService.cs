#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class StatusService(OracleDbContext db) : ServiceBase(db)
{
	public async Task<IOutcome> AddStatus(int characterId, CharacterStatusOptions statusOptions)
	{
		var status = new CharacterStatus()
		{
			Description = statusOptions.Description,
			CanQuest = statusOptions.CanQuest,
			StartDay = statusOptions.StartDay,
			EndDay = statusOptions.EndDay,
			CharacterId = characterId
		};

		Db.CharacterStatuses.Add(status);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task RemoveStatus(int statusId)
	{
		var status = await Db.CharacterStatuses.FindAsync(statusId);

		if (status != null)
		{
			Db.CharacterStatuses.Remove(status);
			await Db.SaveChangesAsync();
		}
	}

	public async Task<List<CharacterStatus>> GetStatuses(int characterId, int startDay, int endDay)
	{
		return await Db.CharacterStatuses.Where(x => x.CharacterId == characterId
		                                             && ((x.StartDay >= startDay && x.StartDay <= endDay)
		                                                 || (x.EndDay >= startDay && x.EndDay <= endDay)
		                                                 || (x.StartDay < endDay && x.EndDay == null)))
			.Include(x => x.Character)
			.ToListAsync();
	}

	public async Task<List<CharacterStatus>> GetStatuses(List<int> characterIds, int startDay, int endDay)
	{
		return await Db.CharacterStatuses.Where(x => characterIds.Contains(x.CharacterId)
		                                             && ((x.StartDay >= startDay && x.StartDay <= endDay)
		                                                 || (x.EndDay >= startDay && x.EndDay <= endDay)
		                                                 || (x.StartDay < endDay && x.EndDay == null)))
			.Include(x => x.Character)
			.ToListAsync();
	}
}

public class CharacterStatusOptions(string description, int startDay, int? endDay = null, bool canQuest = false)
{
	public string Description = description;
	public int StartDay = startDay;
	public int? EndDay = endDay;
	public bool CanQuest = canQuest;
}