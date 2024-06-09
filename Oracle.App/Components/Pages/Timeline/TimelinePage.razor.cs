﻿#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data.Models;
using Oracle.Logic.Services;
using Oracle.Logic.Services.TimelineService;
using Color = MudBlazor.Color;

#endregion

namespace Oracle.App.Components.Pages.Timeline;

public partial class TimelinePage : OracleBasePage
{
	[Inject] private TimelineService TimelineService { get; set; } = null!;
	[Inject] private CharacterService CharacterService { get; set; } = null!;
	[Inject] private ActivityService ActivityService { get; set; } = null!;
	public List<CharacterTimelineVm> Timeline { get; set; } = new();
	public static int TimelineDayWidthPixels = 40;
	public static int TimelineDayMarginPixels = 3;

	public int StartDay { get; set; } = 0;
	public int EndDay => StartDay + 19;
	public int MaxEndDay { get; set; } = 365;

	public List<Character> Characters { get; set; } = new();
	public List<int> CharacterIds => Characters.Select(x => x.Id).ToList();

	protected override async Task OnInitializedAsync()
	{
		Characters = await CharacterService.GetAllCharacters(new CharacterLoadOptions());

		StartDay = await TimelineService.GetMaxTimelineDate();
		MaxEndDay = StartDay + 365;

		await Refresh();
	}

	protected override async Task Refresh()
	{
		Timeline = await TimelineService.GetTimelineForManyCharacters(CharacterIds, StartDay, EndDay);

		StateHasChanged();
	}

	#region DateAdjustment

	private async Task OnStartDateChanged(int value)
	{
		var adjustment = value - StartDay;

		await AdjustDateRange(adjustment);
	}

	private async Task AdjustDateRange(int adjustment)
	{
		var resultingStartDay = Math.Max(0, StartDay + adjustment);

		if (resultingStartDay == StartDay)
			return;

		StartDay = resultingStartDay;
		await Refresh();
	}

	#endregion


	public static int GetTimelineDayPixelWidth(int timelineDayPixels, int numDays, int timelineDayMarginPixels)
	{
		var numMarginPixels = numDays <= 1 ? 0 : timelineDayMarginPixels * (numDays - 1);
		var numPixels = timelineDayPixels * numDays;
		numPixels += numMarginPixels;
		return numPixels;
	}

	public static int GetTimelineEventDuration(TimelineDateVm timelineEvent, int endDay)
	{
		if (timelineEvent is { HasEndDate: true, EndDate: not null } && timelineEvent.EndDate <= endDay)
			return GetCountOfIntegers(timelineEvent.StartDate, timelineEvent.EndDate.Value);

		return GetCountOfIntegers(timelineEvent.StartDate, endDay);
	}

	public static int GetCountOfIntegers(int start, int end)
	{
		var lowerBound = Math.Min(start, end);
		var upperBound = Math.Max(start, end);

		return upperBound - lowerBound + 1;
	}

	#region UiEvents

	private async Task OnMenuCommandClicked(string commandName, int date, int characterId, int? timelineId = null,
		int? statusId = null)
	{
		int? entityId = null;

		if (timelineId != null && statusId == null)
			entityId = await TimelineService.GetConnectedEntityId(timelineId.Value);

		if (timelineId == null && statusId != null)
			entityId = statusId.Value;

		switch (commandName)
		{
			case "addActivity":
				await AddActivityButton_Clicked(date, characterId);
				break;
			case "deleteActivity":
				var confirmed = await Confirm("Delete Activity", "Really delete this activity?", "Delete", Color.Error);
				if (confirmed && entityId != null)
				{
					await ActivityService.RemoveActivity(entityId.Value);
					await Refresh();
				}

				break;
			case "addToAdventure":
				// Add your code here for the "addToAdventure" case
				break;
			case "addStatus":
				await AddStatusButton_Clicked(date, characterId);
				break;
			case "deleteStatus":
				// Add your code here for the "deleteStatus" case
				break;
			case "editActivity":
				// Add your code here for the "editActivity" case
				break;
			case "editStatus":
				// Add your code here for the "editStatus" case
				break;
			default:
				break;
		}
	}

	#endregion


	#region TimelineActions

	private async Task AddActivityButton_Clicked(int date, int characterId)
	{
		var parameters = new DialogParameters
		{
			{ "CharacterId", characterId },
			{ "Date", date }
		};

		var dialog = await DialogService.ShowAsync<Timeline_AddActivityDialog>("Add new activity", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await Refresh();
			Snackbar.Add("New activity added!", Severity.Success);
		}
	}

	private async Task AddStatusButton_Clicked(int date, int characterId)
	{
		var parameters = new DialogParameters
		{
			{ "CharacterId", characterId },
			{ "StartDate", date },
			{ "EndDate", date + 7 }
		};

		var dialog = await DialogService.ShowAsync<NewConditionDialog>("Add new status", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await Refresh();
			Snackbar.Add("New status added!", Severity.Success);
		}
	}

	#endregion
}