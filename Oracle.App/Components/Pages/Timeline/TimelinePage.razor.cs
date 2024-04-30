#region using

using Microsoft.AspNetCore.Components;
using Oracle.Data.Models;
using Oracle.Logic.Services;
using Oracle.Logic.Services.TimelineService;

#endregion

namespace Oracle.App.Components.Pages.Timeline;

public partial class TimelinePage : OracleBasePage
{
	[Inject] private TimelineService TimelineService { get; set; } = null!;
	[Inject] private CharacterService CharacterService { get; set; } = null!;
	public List<TimelineDateVm> Timeline { get; set; } = new();
	public List<int> TimelineDays => Timeline.Select(x => x.Date).Distinct().ToList();

	public List<Character> Characters { get; set; } = new();
	public List<int> CharacterIds => Characters.Select(x => x.Id).ToList();

	protected override async Task Refresh()
	{
		Characters = await CharacterService.GetAllCharacters(new CharacterLoadOptions());
		Timeline = await TimelineService.GetTimelineForManyCharacters(CharacterIds, 0, 30);
		StateHasChanged();
	}
}