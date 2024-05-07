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
	public List<CharacterTimelineVm> Timeline { get; set; } = new();
	public static int TimelineDayWidthPixels = 40;
	public static int TimelineDayMarginPixels = 3;

	public int StartDay { get; set; } = 0;
	public int EndDay => StartDay + 20;

	public List<Character> Characters { get; set; } = new();
	public List<int> CharacterIds => Characters.Select(x => x.Id).ToList();

	protected override async Task Refresh()
	{
		Characters = await CharacterService.GetAllCharacters(new CharacterLoadOptions());
		Timeline = await TimelineService.GetTimelineForManyCharacters(CharacterIds, StartDay, EndDay);
		StateHasChanged();
	}

	private async Task AdjustDateRange(int adjustment)
	{
		var resultingStartDay = Math.Max(0, StartDay + adjustment);

		if (resultingStartDay == StartDay)
			return;

		StartDay = resultingStartDay;
		await Refresh();
	}

	public static string GetTimelineDayPixelWidth(int timelineDayPixels, int numDays, int timelineDayMarginPixels)
	{
		var numMarginPixels = numDays <= 1 ? 0 : timelineDayMarginPixels * (numDays - 1);
		var numPixels = timelineDayPixels * numDays;
		numPixels += numMarginPixels;
		return $"{numPixels}px";
	}
}