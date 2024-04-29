#region using

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
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
		var testList = new List<int>();
		var outcome = await AdventureService.TryStartAdventure(AdventureId, 1, testList);

		if (outcome.Success)
			await Refresh();

		Snackbar.Add(outcome.Failure ? outcome.ToString() : "Adventure started", outcome.Failure ? Severity.Error : Severity.Success);
	}

	private async Task EndAdventure()
	{
		var outcome = await AdventureService.TryEndAdventure(AdventureId);

		if (outcome.Success)
			await Refresh();

		Snackbar.Add(outcome.Failure ? outcome.ToString() : "Adventure ended",
			outcome.Failure ? Severity.Error : Severity.Success);
	}


	private async Task AddCharacter()
	{
		var outcome = await AdventureService.TryAddCharacterToAdventure(AdventureId, 1);

		if (outcome.Success)
			await Refresh();

		Snackbar.Add(outcome.Failure ? outcome.ToString() : "Character added", outcome.Failure ? Severity.Error : Severity.Success);
	}
}