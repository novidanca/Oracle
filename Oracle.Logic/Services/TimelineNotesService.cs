#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class TimelineNotesService(OracleDbContext db) : ServiceBase(db)
{
	public async Task<IOutcome> AddTimelineNote(NoteOptions noteOptions)
	{
		var note = new TimelineNote()
		{
			Note = noteOptions.Note,
			StartDate = noteOptions.StartDate,
			EndDate = noteOptions.EndDate,
			CharacterId = noteOptions.CharacterId
		};

		Db.TimelineNotes.Add(note);
		await Db.SaveChangesAsync();

		return Outcomes.Success();
	}

	public async Task RemoveTimelineNote(int noteId)
	{
		var note = await Db.TimelineNotes.FindAsync(noteId);

		if (note != null)
		{
			Db.TimelineNotes.Remove(note);
			await Db.SaveChangesAsync();
		}
	}

	public async Task<List<TimelineNote>> GetTimelineNotes(int characterId, int startDay, int endDay,
		bool trackEntities = true)
	{
		var query = Db.TimelineNotes.Where(x => x.CharacterId != null && x.CharacterId == characterId
		                                                              && ((x.StartDate >= startDay &&
		                                                                   x.StartDate <= endDay)
		                                                                  || (x.EndDate >= startDay &&
		                                                                      x.EndDate <= endDay)
		                                                                  || (x.StartDate < endDay &&
		                                                                      x.EndDate == null)))
			.Include(x => x.Character);

		if (!trackEntities) query.AsNoTracking();

		return await query.ToListAsync();
	}

	public async Task<List<TimelineNote>> GetTimelineNotes(List<int> characterIds, int startDay, int endDay,
		bool trackEntities = true)
	{
		var query = Db.TimelineNotes.Where(x => x.CharacterId != null && characterIds.Contains((int)x.CharacterId)
		                                                              && ((x.StartDate >= startDay &&
		                                                                   x.StartDate <= endDay)
		                                                                  || (x.EndDate >= startDay &&
		                                                                      x.EndDate <= endDay)
		                                                                  || (x.StartDate < endDay &&
		                                                                      x.EndDate == null)))
			.Include(x => x.Character);

		if (!trackEntities) query.AsNoTracking();

		return await query.ToListAsync();
	}

	public async Task<List<TimelineNote>> GetTimelineNotes(int startDay, int endDay)
	{
		return await Db.TimelineNotes.Where(x => x.CharacterId == null
		                                         && ((x.StartDate >= startDay &&
		                                              x.StartDate <= endDay)
		                                             || (x.EndDate >= startDay &&
		                                                 x.EndDate <= endDay)
		                                             || (x.StartDate < endDay &&
		                                                 x.EndDate == null)))
			.ToListAsync();
	}
}

public class NoteOptions(string note, int startDate, int? endDate = null, int? characterId = null)
{
	public string Note = note;
	public int? CharacterId = characterId;
	public int StartDate = startDate;
	public int? EndDate = endDate;
}