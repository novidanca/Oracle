#region using

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.AdventureDetail;

public partial class AdventureDetailPage : OracleBasePage
{
	[Inject] private AdventureService AdventureService { get; set; } = null!;
	[Inject] private CharacterService CharacterService { get; set; } = null!;
	[Parameter] public int AdventureId { get; set; }

	private Adventure? Adventure { get; set; }

	protected override async Task Refresh()
	{
		Adventure = await AdventureService.GetAdventure(AdventureId);
	}

	private async Task UpdateAdventure()
	{
		Db.Entry(Adventure!).State = EntityState.Modified;
		await Db.SaveChangesAsync();
		await Refresh();
	}

	private async Task StartAdventure()
	{
		if (Adventure == null)
			return;

		var characterIds = Adventure.AdventureCharacters.Select(x => x.CharacterId).ToList();
		var outcome = await AdventureService.TryStartAdventure(AdventureId, characterIds);

		if (outcome.Success)
			await Refresh();

		Snackbar.Add(outcome.Failure ? outcome.ToString() : "Adventure started",
			outcome.Failure ? Severity.Error : Severity.Success);
	}

	private async Task EndAdventure()
	{
		var outcome = await AdventureService.TryEndAdventure(AdventureId);

		if (outcome.Success)
			await Refresh();

		Snackbar.Add(outcome.Failure ? outcome.ToString() : "Adventure ended",
			outcome.Failure ? Severity.Error : Severity.Success);
	}

	private async Task AddDay()
	{
		var outcome = await AdventureService.TryAddAdventureDay(AdventureId);

		if (outcome)
			await Refresh();

		Snackbar.Add(outcome ? "Day added" : "Failed to add day", outcome ? Severity.Success : Severity.Error);
	}

	private async Task AddCharacter()
	{
		var parameters = new DialogParameters
		{
			{ "AdventureId", AdventureId },
			{ "AdventureStarted", Adventure?.IsStarted }
		};

		var dialog = await DialogService.ShowAsync<AddCharacterToAdventureDialog>("Add new activity", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await Refresh();
			Snackbar.Add("Character added!", Severity.Success);
		}
		else
		{
			Snackbar.Add("Could not add character to adventure", Severity.Error);
		}
	}

	private async Task RemoveCharacter(int characterId)
	{
		var outcome = await AdventureService.RemoveCharacterFromAdventure(AdventureId, characterId);

		if (outcome)
			await Refresh();

		Snackbar.Add(outcome ? "Character removed" : "Failed to remove character",
			outcome ? Severity.Success : Severity.Error);
	}
}