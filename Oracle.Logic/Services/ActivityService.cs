#region using

using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class ActivityService(OracleDbContext db, TimelineService.TimelineService timelineService) : ServiceBase(db)
{
    public async Task<IOutcome> TryAddActivity(int characterId, int activityTypeId, int date, int? projectId)
    {
        // Check if the character is available on the given date
        var isAvailable = await timelineService.IsCharacterAvailable(characterId, date);

        if (isAvailable.Failure) return Outcomes.Failure().WithMessage("Character is not available on the given date.");

        // Add the activity to the database
        var activity = new Activity
        {
            CharacterId = characterId,
            ActivityTypeId = activityTypeId,
            Date = date,
            ProjectId = projectId
        };

        db.Activities.Add(activity);
        await db.SaveChangesAsync();

        // Add the activity to the timeline using TimelineService
        await timelineService.AddToCharacterTimeline(activity, characterId);

        return Outcomes.Success();
    }

    public async Task<IOutcome> RemoveActivity(int activityId)
    {
        // Find the activity in the database
        var activity = await db.Activities.FindAsync(activityId);

        if (activity == null) return Outcomes.Failure().WithMessage("Activity not found.");

        // Remove the activity from the character timeline using TimelineService
        await timelineService.RemoveFromCharacterTimeline(activity, activity.CharacterId);

        // Remove the activity from the database
        db.Activities.Remove(activity);
        await db.SaveChangesAsync();

        return Outcomes.Success();
    }

    public async Task<List<ActivityType>> GetActivityTypes()
    {
        return await Db.ActivityTypes.ToListAsync();
    }
}