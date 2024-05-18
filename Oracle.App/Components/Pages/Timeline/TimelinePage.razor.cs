#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Pages.Timeline.Components;
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
	public int EndDay => StartDay + 19;

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

	public static int GetTimelineDayPixelWidth(int timelineDayPixels, int numDays, int timelineDayMarginPixels)
	{
		var numMarginPixels = numDays <= 1 ? 0 : timelineDayMarginPixels * (numDays - 1);
		var numPixels = timelineDayPixels * numDays;
		numPixels += numMarginPixels;
		return numPixels;
	}

	public static int GetTimelineEventDuration(TimelineDateVm timelineEvent, int endDay)
	{
		if (timelineEvent is { IsComplete: true, EndDate: not null } && timelineEvent.EndDate <= endDay)
			return GetCountOfIntegers(timelineEvent.StartDate, timelineEvent.EndDate.Value);

		return GetCountOfIntegers(timelineEvent.StartDate, endDay);
	}

	public static int GetCountOfIntegers(int start, int end)
	{
		var lowerBound = Math.Min(start, end);
		var upperBound = Math.Max(start, end);

		return upperBound - lowerBound + 1;
	}


	#region UI Events

	private async Task AddActivityButton_Clicked(int date, int characterId)
	{
		var parameters = new DialogParameters
		{
			{ "CharacterId", characterId },
			{ "Date", date }
		};

		var dialog = await DialogService.ShowAsync<NewActivityDialog>("Add new activity", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await Refresh();
			Snackbar.Add("New activity added!", Severity.Success);
		}
	}

	#endregion
}