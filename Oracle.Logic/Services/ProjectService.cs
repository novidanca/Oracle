#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class ProjectService(OracleDbContext db) : ServiceBase(db)
{
    public async Task<List<Project>> GetActiveProjects(int characterId)
    {
        var activeProjects = db.Projects
            .Where(p => p.OwningCharacterId == characterId &&
                        p.Goal > p.ContributingActivities.Sum(a => a.ActivityType.ProjectContributionAmount))
            .AsSplitQuery()
            .AsNoTracking()
            .ToList();

        return activeProjects;
    }
}