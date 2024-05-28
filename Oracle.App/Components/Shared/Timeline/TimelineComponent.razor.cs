#region using

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.Data.Models;
using Oracle.Logic.Services;
using Oracle.Logic.Services.TimelineService;
using Color = MudBlazor.Color;

#endregion

namespace Oracle.App.Components.Shared.Timeline;

public partial class TimelineComponent
{
	[Inject] public ActivityService ActivityService { get; set; } = null!;
	[Inject] public TimelineService TimelineService { get; set; } = null!;
	[Inject] protected IDialogService DialogService { get; set; } = null!;
	[Inject] protected ISnackbar Snackbar { get; set; } = null!;
	[Inject] protected NavigationManager NavManager { get; set; } = null!;

	[Parameter] [EditorRequired] public List<CharacterTimelineVm> CharacterTimelines { get; set; } = [];
	[Parameter] [EditorRequired] public List<Character> Characters { get; set; } = [];
	[Parameter] [EditorRequired] public int StartDay { get; set; }
	[Parameter] [EditorRequired] public int EndDay { get; set; }
	[Parameter] [EditorRequired] public EventCallback OnStateChanged { get; set; }

	[Parameter] public int TimelineDayWidthPixels { get; set; } = 50;
	[Parameter] public int TimelineDayHeightPixels { get; set; } = 30;
	[Parameter] public int TimelineDayMarginPixels { get; set; } = 1;

	#region TimelineDisplayHelpers

	public static int GetTimelineDayPixelWidth(int timelineDayPixels, int numDays, int timelineDayMarginPixels)
	{
		var numMarginPixels = numDays <= 1 ? 0 : timelineDayMarginPixels * (numDays - 1);
		var numPixels = timelineDayPixels * numDays;
		numPixels += numMarginPixels;
		return numPixels;
	}

	public static int GetCountOfIntegers(int start, int end)
	{
		var lowerBound = Math.Min(start, end);
		var upperBound = Math.Max(start, end);

		return upperBound - lowerBound + 1;
	}

	public static int GetTimelineEventDuration(TimelineDateVm timelineEvent, int endDay)
	{
		if (timelineEvent is { HasEndDate: true, EndDate: not null } && timelineEvent.EndDate <= endDay)
			return GetCountOfIntegers(timelineEvent.StartDate, timelineEvent.EndDate.Value);

		return GetCountOfIntegers(timelineEvent.StartDate, endDay);
	}

	public static List<StatusVm> GetOverlappingStatusVms(List<StatusVm> statusVms, int startDay, int endDay)
	{
		return statusVms.Where(s => s.StartDate <= endDay && s.EndDate >= startDay).ToList();
	}

	#endregion


	#region MenuInteraction

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
					await OnStateChanged.InvokeAsync();
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

		var dialog = await DialogService.ShowAsync<NewActivityDialog>("Add new activity", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await OnStateChanged.InvokeAsync();
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

		var dialog = await DialogService.ShowAsync<NewStatusDialog>("Add new status", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await OnStateChanged.InvokeAsync();
			Snackbar.Add("New status added!", Severity.Success);
		}
	}

	#endregion


	#region UiHelpers

	protected async Task<bool> Confirm(string title, string text, string buttonText = "Yes",
		Color buttonColor = Color.Primary)
	{
		var parameters = new DialogParameters()
		{
			{ "Title", title },
			{ "Text", text },
			{ "ButtonText", buttonText },
			{ "ButtonColor", buttonColor }
		};

		var dialog = await DialogService.ShowAsync<ConfirmDialog>(title, parameters);
		var result = await dialog.Result;

		return !result.Canceled;
	}

	#endregion
}