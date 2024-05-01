namespace Oracle.Logic.Services.TimelineService;

public class CharacterTimelineVm
{
	public int CharacterId { get; set; }
	public List<TimelineDateVm> Timeline { get; set; } = [];
}

public class TimelineDateVm
{
	public int CharacterId { get; set; }
	public int Date { get; set; }
	public TimelineEntityTypes Type { get; set; } = TimelineEntityTypes.None;
	public int? TimelineId { get; set; }
	public string? Description { get; set; }
	public string? EntityLink { get; set; }
	public bool IsComplete { get; set; } = true;
}

public enum TimelineEntityTypes
{
	None,
	Adventure,
	Activity,
	BlockingStatus
}