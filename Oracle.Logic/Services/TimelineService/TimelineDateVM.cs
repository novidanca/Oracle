

namespace Oracle.Logic.Services.TimelineService;
public class TimelineDateVm
{
	public int CharacterId { get; set; }
	public int Date { get; set; }
	public TimelineEntityTypes Type { get; set; } = TimelineEntityTypes.None;
	public int? TimelineId { get; set; }
	public string? Description { get; set; }
	public string? EntityLink { get; set; }
}

public enum TimelineEntityTypes
{
	None,
	Adventure,
	Activity,
	BlockingStatus
}