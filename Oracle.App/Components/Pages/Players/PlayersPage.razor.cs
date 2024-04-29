using Microsoft.AspNetCore.Components;
using Oracle.Data.Models;
using Oracle.Logic.Services.TimelineService;

namespace Oracle.App.Components.Pages.Players;

public partial class PlayersPage : OracleBasePage
{
	[Inject] private TimelineService TimelineService { get; set; } = null!;
	public List<TimelineDateVm> Timeline { get; set; } = new();
	protected override async Task Refresh()
	{
		Timeline = await TimelineService.GetTimelineForCharacter(1, 0, 100);
	}
}