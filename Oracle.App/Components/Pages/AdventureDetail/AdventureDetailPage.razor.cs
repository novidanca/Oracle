#region using

using Microsoft.AspNetCore.Components;
using Oracle.Data.Models;
using Oracle.Logic.Services;

#endregion

namespace Oracle.App.Components.Pages.AdventureDetail;

public partial class AdventureDetailPage : OracleBasePage
{
	[Inject] private AdventureService AdventureService { get; set; } = null;
	[Parameter] public int AdventureId { get; set; }

	private Adventure? Adventure { get; set; }

	protected override async Task OnInitializedAsync()
	{
		Adventure = await AdventureService.GetAdventure(AdventureId);
	}
}