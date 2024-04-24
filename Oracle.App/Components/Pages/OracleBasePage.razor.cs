﻿#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Layout;
using Oracle.Data;

#endregion

namespace Oracle.App.Components.Pages;

public partial class OracleBasePage : ComponentBase
{
	[CascadingParameter] public MainLayout? Layout { get; set; }
	[Inject] protected OracleDbContext Db { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected ISnackbar Snackbar { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await Refresh();
		StateHasChanged();
	}

	protected virtual async Task Refresh()
	{
		return;
	}
}