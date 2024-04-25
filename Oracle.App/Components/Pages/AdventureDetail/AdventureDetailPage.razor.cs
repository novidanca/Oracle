#region using

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.AdventureDetail;

public partial class AdventureDetailPage : OracleBasePage
{
	[Inject] private AdventureService AdventureService { get; set; } = null!;
	[Inject] private CharacterService CharacterService { get; set; } = null!;
	[Parameter] public int AdventureId { get; set; }


	private List<Character> AvailableCharacters { get; set; } = [];

	private Adventure? Adventure { get; set; }

	protected override async Task Refresh()
	{
		Adventure = await AdventureService.GetAdventure(AdventureId);
		AvailableCharacters =
			await CharacterService.GetAllAvailableCharacters(Adventure.StartDay, new CharacterLoadOptions(true));
	}

	private async Task UpdateAdventure()
	{
		Db.Entry(Adventure!).State = EntityState.Modified;
		await Db.SaveChangesAsync();
		await Refresh();
	}
}