#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.Adventures;

public partial class AdventuresPage : OracleBasePage
{
	[Inject] private AdventureService AdventureService { get; set; } = null!;

	//Adventures
	private List<Adventure> RelevantAdventures { get; set; } = [];
	private List<Adventure> SearchResultAdventures { get; set; } = [];
	private Adventure? SelectedAdventure { get; set; }

	//Search
	private string SearchQuery { get; set; } = "";
	private int StartDay { get; set; } = 0;
	private int MaxStartDay { get; set; } = 0;
	private int EndDay { get; set; } = 0;
	private int QuerySize { get; set; } = 10;
	private int Skip { get; set; } = 0;

	private bool IsSearchFilterExpanded { get; set; } = false;

	protected override async Task Refresh()
	{
		RelevantAdventures = await AdventureService.GetRelevantAdventures();
		MaxStartDay = await AdventureService.GetMaxStartDay();
	}

	private async Task NewAdventureButton_Clicked()
	{
		var dialog = await DialogService.ShowAsync<NewAdventureDialog>("Add new adventure");
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			var newName = result.Data.ToString();
			await Refresh();
			Snackbar.Add($"{newName} created!", Severity.Success);
		}
	}

	private async Task OnSearchFiltersApplied()
	{
		IsSearchFilterExpanded = false;
		await OnSearchQueryChanged(SearchQuery);
	}

	private async Task OnSearchQueryChanged(string value)
	{
		SearchQuery = value;

		if (!string.IsNullOrEmpty(SearchQuery))
			SearchResultAdventures =
				await AdventureService.SearchAdventures(SearchQuery, StartDay, EndDay, QuerySize, Skip);
	}

	private void ViewAdventure_Clicked(int adventureId)
	{
		NavManager.NavigateTo($"/adventureDetail/{adventureId}");
	}
}