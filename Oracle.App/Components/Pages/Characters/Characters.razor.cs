#region using

using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Oracle.App.Components.Pages.Characters.Components;
using Oracle.Data.Models;

#endregion

namespace Oracle.App.Components.Pages.Characters;

public partial class Characters : OracleBasePage
{
	private List<Character> AllCharacters { get; set; } = new();

	//Refs
	private MudDialog NewCharacterDialog;

	protected override async Task OnInitializedAsync()
	{
		await Refresh();
	}

	protected override async Task Refresh()
	{
		AllCharacters = await Db.Characters.ToListAsync();
	}

	private async Task NewCharacterButton_Clicked()
	{
		var dialog = await DialogService.ShowAsync<NewCharacterDialog>();
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			var newName = result.Data.ToString();
			var character = new Character()
			{
				Name = newName
			};

			Db.Characters.Add(character);
			await Db.SaveChangesAsync();
			await Refresh();
		}
	}
}