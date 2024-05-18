#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Pages.Projects.Components;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.Projects;

public partial class ProjectsPage : OracleBasePage
{
    [Inject] public ProjectService ProjectService { get; set; } = null!;
    [Inject] public CharacterService CharacterService { get; set; } = null!;

    public bool IsSearchFilterExpanded { get; set; }
    public string? SearchQuery { get; set; }
    public List<Character> Characters { get; set; } = [];
    public int SelectedCharacterId { get; set; }

    private List<Project> Projects { get; set; } = [];

    protected override async Task Refresh()
    {
        Projects = await ProjectService.GetActiveProjects();
        Characters = await CharacterService.GetAllCharacters();
    }

    private async Task NewProjectButton_Clicked()
    {
        // Open the NewProjectDialog and pass in the Characters as a parameter
        var dialog = await DialogService.ShowAsync<NewProjectDialog>("Add new project", new DialogParameters()
        {
            { "Characters", Characters }
        });

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Refresh();
            Snackbar.Add("New project added!", Severity.Success);
        }
    }

    private async Task OnSearchQueryChanged(string value)
    {
        SearchQuery = value;
        Projects = await ProjectService.SearchProjects(SearchQuery,
            SelectedCharacterId == 0 ? null : SelectedCharacterId);
    }
}