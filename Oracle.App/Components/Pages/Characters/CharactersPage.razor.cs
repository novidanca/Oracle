#region using

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.Characters;

public partial class CharactersPage : OracleBasePage
{
	[Inject] private CharacterService CharacterService { get; set; } = null!;
	private List<Character> AllCharacters { get; set; } = new();

	protected override async Task Refresh()
	{
		AllCharacters = await CharacterService.GetAllCharacters();
	}

	private async Task NewCharacterButton_Clicked()
	{
		var parameters = new DialogParameters<NewCharacterDialog>();
		var players = await Db.Players.ToListAsync();

		parameters.Add(x => x.Players, players);

		var dialog = await DialogService.ShowAsync<NewCharacterDialog>("Add new character", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			var newName = result.Data.ToString();
			await Refresh();
			Snackbar.Add($"{newName} created!", Severity.Success);
		}
	}


	private async Task OnCharacterDeleteButton_Clicked(Character character)
	{
		var name = character.Name;
		await CharacterService.DeleteCharacter(character);
		await Refresh();
		Snackbar.Add($"{name} was deleted", Severity.Info);
	}
}