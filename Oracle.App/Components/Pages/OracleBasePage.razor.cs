#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Layout;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data;
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

	protected override async Task OnInitializedAsync()
	{
		await Refresh();
		StateHasChanged();
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