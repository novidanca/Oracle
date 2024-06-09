#region using

using Ether.Outcomes;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services.TimelineService;

public class TimelineAddService(OracleDbContext db) : ServiceBase(db)
{
	#region Adventures

	public async Task<IOutcome> AddToCharacterTimeline(Adventure adventure, int characterId, int? manualStartDay = null)
	{
		var timeline = new CharacterTimeline()
		{
			AdventureId = adventure.Id,
			CharacterId = characterId,
			StartDay = GetAdventureStartDay(adventure, manualStartDay),
			EndDay = adventure.IsComplete || adventure.HasFixedDuration
				? adventure.StartDay + (adventure.Duration - 1)
				: null
		};

		Db.CharacterTimelines.Add(timeline);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public static int GetAdventureStartDay(Adventure adventure, int? manualStartDay)
	{
		if (manualStartDay == null) return adventure.StartDay;

		var startDay = manualStartDay.Value;
		var endDay = adventure.StartDay + adventure.Duration - 1;

		if (startDay >= adventure.StartDay && startDay <= endDay) return startDay;

		return adventure.StartDay;
	}

	#endregion

	#region Activities

	public async Task<IOutcome> AddToCharacterTimeline(Activity activity, int characterId)
	{
		var timeline = new CharacterTimeline()
		{
			ActivityId = activity.Id,
			CharacterId = characterId,
			StartDay = activity.StartDate,
			EndDay = activity.EndDate
		};

		Db.CharacterTimelines.Add(timeline);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	#endregion
}