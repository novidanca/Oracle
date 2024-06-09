#region using

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oracle.App.Components.Shared.Dialogs;
using Oracle.App.Components.Shared.Timeline.Models;
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
	[Parameter] public TimelineDayStyleSpec StyleSpec { get; set; } = new();

	private Stopwatch stopwatch = new();
	private long elapsedMs;

	protected override void OnParametersSet()
	{
		stopwatch.Restart();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		stopwatch.Stop();
		elapsedMs = stopwatch.ElapsedMilliseconds;
	}


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

	public static List<ConditionVm> GetOverlappingConditions(List<ConditionVm> conditionsVms, int startDay, int endDay)
	{
		return conditionsVms.Where(s => s.StartDate <= endDay && (s.EndDate == null || s.EndDate >= startDay)).ToList();
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
			case "addCondition":
				await AddConditionButton_Clicked(date, characterId);
				break;
			case "deleteCondition":
				// Add your code here for the "deleteStatus" case
				break;
			case "editActivity":
				// Add your code here for the "editActivity" case
				break;
			case "manageConditions":
				// Add your code here for the "editStatus" case
				break;
			case "addNote":
				await AddNoteButton_Clicked(date, characterId);
				break;
			default:
				break;
		}
	}

	private async Task OnNoteMenuClicked(NoteVm note, string commandName, int day = 0)
	{
		switch (commandName)
		{
			case "editNote":
				// Add your code here for the "editNote" case
				break;
			case "deleteNote":
				// Add your code here for the "deleteNote" case
				break;
			case "endNote":
				// Add your code here for the "endNote" case
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
			await OnStateChanged.InvokeAsync();
			Snackbar.Add("New activity added!", Severity.Success);
		}
	}

	private async Task AddConditionButton_Clicked(int date, int characterId)
	{
		var parameters = new DialogParameters
		{
			{ "CharacterId", characterId },
			{ "StartDate", date },
			{ "EndDate", date + 7 }
		};

		var dialog = await DialogService.ShowAsync<NewConditionDialog>("Add new condition", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await OnStateChanged.InvokeAsync();
			Snackbar.Add("New condition added!", Severity.Success);
		}
	}


	private async Task AddNoteButton_Clicked(int date, int characterId)
	{
		var parameters = new DialogParameters
		{
			{ "CharacterId", characterId },
			{ "StartDate", date },
			{ "EndDate", date + 7 }
		};

		var dialog = await DialogService.ShowAsync<Timeline_AddNoteDialog>("Add new note", parameters);
		var result = await dialog.Result;

		if (!result.Canceled)
		{
			await OnStateChanged.InvokeAsync();
			Snackbar.Add("New note added!", Severity.Success);
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


	private string GetConditionDescriptions(List<ConditionVm> conditions)
	{
		return conditions.Any() ? string.Join(", ", conditions.Select(x => x.Description)) : "";
	}

	private int GetNoteSpanWidth(NoteVm note, int currentDay)
	{
		if (note is { EndDate: not null } && note.EndDate == currentDay) return StyleSpec.DayWidthPixels;
		return StyleSpec.DayWidthPixels + StyleSpec.MarginPixels;
	}

	private int GetNoteSpanMargin(NoteVm note, int currentDay)
	{
		if (note is { EndDate: not null } && note.EndDate == currentDay) return StyleSpec.MarginPixels;
		return 0;
	}

	public static string GetHexColorFromDescription(string description)
	{
		// Step 1: Hash the string
		var hash = GetStableHash(description);

		// Step 2: Convert hash to a hex color
		var hexColor = HashToColor(hash);

		return hexColor;
	}

	private static int GetStableHash(string input)
	{
		// Use a hash function to generate a stable integer hash from the string
		using (var md5 = MD5.Create())
		{
			var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			var hash = BitConverter.ToInt32(hashBytes, 0);
			return hash;
		}
	}

	private static string HashToColor(int hash)
	{
		// Ensure the hash is positive
		hash = Math.Abs(hash);

		// Generate RGB values from the hash
		var r = (hash & 0xFF0000) >> 16;
		var g = (hash & 0x00FF00) >> 8;
		var b = hash & 0x0000FF;

		// Convert RGB to hex
		return $"#{r:X2}{g:X2}{b:X2}";
	}

	#endregion
}