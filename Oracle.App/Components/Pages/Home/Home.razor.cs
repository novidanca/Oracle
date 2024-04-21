#region using

using Microsoft.AspNetCore.Components;
using Oracle.Data.Models;
using Oracle.Logic.Services.CampaignSettings;

#endregion

namespace Oracle.App.Components.Pages.Home;

public partial class Home : OracleBasePage
{
	[Inject] private CampaignSettingsService SettingsService { get; set; }

	private CampaignSettings CampaignSettings;

	#region Init

	protected override async Task OnInitializedAsync()
	{
		await GetSettings();
		UpdateLayout();

		await Refresh();
	}

	private async Task GetSettings()
	{
		CampaignSettings = await SettingsService.GetCampaignSettings();
	}

	private void UpdateLayout()
	{
		if (Layout == null) return;
		Layout.AppTitle = CampaignSettings.CampaignName;
		Layout.EnableDrawer();
	}

	#endregion

	protected override async Task Refresh()
	{
	}
}