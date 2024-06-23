#region using

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.CampaignSettings;

public partial class CampaignSettingsPage : OracleBasePage
{
    [Inject] private ActivityService ActivityService { get; set; } = null!;

    public List<Player> Players { get; set; } = new();
    public List<ActivityType> ActivityTypes { get; set; } = new();
    public List<ProjectContributionType> ProjectContributionTypes { get; set; } = new();

    protected override async Task Refresh()
    {
        Players = await Db.Players.ToListAsync();
        ActivityTypes = await ActivityService.GetActivityTypes();
        ProjectContributionTypes = await Db.ProjectContributionTypes.ToListAsync();
    }

    #region UI Events

    private async Task NewActivityTypeButton_Clicked()
    {
        var dialog = await DialogService.ShowAsync<NewActivityTypeDialog>("Add new activity type");
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var newName = result.Data.ToString();
            await Refresh();
            Snackbar.Add($"{newName} created!", Severity.Success);
        }
    }

    private async Task EditActivityTypeButton_Clicked(int selectedActivityTypeId)
    {
        var parameters = new DialogParameters
        {
            { "ActivityTypeId", selectedActivityTypeId }
        };

        var dialog = await DialogService.ShowAsync<NewActivityTypeDialog>("Edit activity type", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var newName = result.Data.ToString();
            await Refresh();
            Snackbar.Add($"{newName} updated!", Severity.Success);
        }
    }

    private async Task NewPlayerButton_Clicked()
    {
        var dialog = await DialogService.ShowAsync<NewPlayerDialog>("Add new player");
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var newName = result.Data.ToString();
            await Refresh();
            Snackbar.Add($"{newName} created!", Severity.Success);
        }
    }

    #endregion
}