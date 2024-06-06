#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Layout;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data;
using Oracle.Logic.Services.CampaignSettings;
using Color = MudBlazor.Color;

#endregion

namespace Oracle.App.Components.Pages;

public partial class OracleBasePage : ComponentBase
{
	[CascadingParameter] public MainLayout? Layout { get; set; }
	[Inject] protected OracleDbContext Db { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected ISnackbar Snackbar { get; set; }
	[Inject] protected NavigationManager NavManager { get; set; }
	[Inject] private CampaignSettingsService SettingsService { get; set; } = null!;

	protected Data.Models.CampaignSettings CampaignSettings { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await GetSettings();
		UpdateLayout();

		await Refresh();
		StateHasChanged();
	}
	private void UpdateLayout()
	{
		if (Layout == null) return;
		Layout.AppTitle = CampaignSettings.CampaignName;
		Layout.EnableDrawer();
	}

	
	private async Task GetSettings()
	{
		CampaignSettings = await SettingsService.GetCampaignSettings();
	}

	protected virtual async Task Refresh()
	{
		return;
	}

	protected async Task<bool> Confirm(string title, string text, string buttonText = "Yes",
		Color buttonColor = Color.Primary)
	{
		var parameters = new DialogParameters()
		{
			{ "Title", title },
			{ "Text", text },
			{ "ButtonText", buttonText },
			{ "ButtonColor", buttonColor }
		};

		var dialog = await DialogService.ShowAsync<ConfirmDialog>(title, parameters);
		var result = await dialog.Result;

		return !result.Canceled;
	}
}