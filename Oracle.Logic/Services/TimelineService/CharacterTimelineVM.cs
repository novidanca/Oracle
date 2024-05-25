namespace Oracle.Logic.Services.TimelineService;

public class CharacterTimelineVm
{
	public int CharacterId { get; set; }
	public List<TimelineDateVm> Timeline { get; set; } = [];
	public List<StatusVm> Statuses { get; set; } = [];
}

public class TimelineDateVm
{
	public int CharacterId { get; set; }
	public int StartDate { get; set; }
	public int? EndDate { get; set; }
	public TimelineEntityTypes Type { get; set; } = TimelineEntityTypes.None;
	public int? TimelineId { get; set; }
	public string? Description { get; set; }
	public string? EntityLink { get; set; }
	public bool HasEndDate => EndDate != null;
	public bool IsComplete { get; set; } = true;
}

public enum TimelineEntityTypes
{
	None,
	Adventure,
	Activity,
	BlockingStatus
}

public class StatusVm
{
	public int StatusId { get; set; }
	public int CharacterId { get; set; }
	public string Description { get; set; }
	public bool CanQuest { get; set; }
	public int StartDate { get; set; }
	public int? EndDate { get; set; }
}