#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data;
using Oracle.Data.Models;

#endregion

namespace Oracle.Logic.Services;

public class ProjectService(OracleDbContext db) : ServiceBase(db)
{
    #region Adders

    public async Task<Project> AddProject(int? characterId, string projectName, int goal, int projectContributionTypeId)
    {
        var project = new Project
        {
            ProjectContributionTypeId = projectContributionTypeId,
            Name = projectName,
            Goal = goal,
            OwningCharacterId = characterId
        };

        Db.Projects.Add(project);
        await Db.SaveChangesAsync();

        return await LoadProject(project.Id);
    }

    #endregion

    #region Getters

    public async Task<List<Project>> GetActiveProjects(int characterId)
    {
        var activeProjectIds = await Db.Projects
            .Where(p => p.OwningCharacterId == characterId &&
                        p.Goal > p.ContributingActivities.Sum(a => a.ActivityType.ProjectContributionAmount))
            .AsSplitQuery()
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync();

        return await LoadProjects(activeProjectIds);
    }

    public async Task<List<Project>> GetActiveProjects()
    {
        var activeProjectIds = await Db.Projects
            .Where(p => p.Goal > p.ContributingActivities.Sum(a => a.ActivityType.ProjectContributionAmount))
            .AsSplitQuery()
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync();

        return await LoadProjects(activeProjectIds);
    }

    public async Task<List<Project>> SearchProjects(string query, int? characterId = null)
    {
        var projectQuery = Db.Projects.AsQueryable();

        if (characterId.HasValue) projectQuery = projectQuery.Where(p => p.OwningCharacterId == characterId.Value);

        var projectIds = await projectQuery
            .Where(p => p.Name.ToLower().Contains(query.ToLower()))
            .AsSplitQuery()
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync();

        return await LoadProjects(projectIds);
    }

    public async Task<List<ProjectContributionType>> GetProjectContributionTypes()
    {
        return await Db.ProjectContributionTypes.ToListAsync();
    }

    #endregion

    #region Loaders

    public async Task<Project> LoadProject(int projectId)
    {
        var project = await Db.Projects
            .Include(p => p.ContributingActivities)
            .ThenInclude(a => a.ActivityType)
            .Include(x => x.OwningCharacter)
            .Include(x => x.ProjectContributionType)
            .FirstAsync(p => p.Id == projectId);

        if (project is { IsComplete: false } && project.ContributingActivities.Any())
        {
            var contributionAmount = project.ContributingActivities.Sum(a => a.ActivityType.ProjectContributionAmount);
            if (contributionAmount >= project.Goal)
            {
                project.IsComplete = true;
                await Db.SaveChangesAsync();
            }
        }

        return project;
    }

    public async Task<List<Project>> LoadProjects(List<int> projectIds)
    {
        var projects = await Db.Projects
            .Include(p => p.ContributingActivities)
            .ThenInclude(a => a.ActivityType)
            .Include(x => x.OwningCharacter)
            .Include(x => x.ProjectContributionType)
            .Where(p => projectIds.Contains(p.Id))
            .ToListAsync();

        foreach (var project in projects)
            if (project is { IsComplete: false } && project.ContributingActivities.Any())
            {
                var contributionAmount =
                    project.ContributingActivities.Sum(a => a.ActivityType.ProjectContributionAmount);
                if (contributionAmount >= project.Goal) project.IsComplete = true;
            }

        await Db.SaveChangesAsync();
        return projects;
    }

    #endregion
}