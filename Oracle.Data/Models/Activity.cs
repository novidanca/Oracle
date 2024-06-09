#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Activity
{
	[Key] public int Id { get; set; }

	[Required] public int ActivityTypeId { get; set; }
	public ActivityType ActivityType { get; set; }

	[Required] public int CharacterId { get; set; }
	public Character Character { get; set; }

	[Required] public int StartDate { get; set; }
	[Required] public int EndDate { get; set; }

	public int? ProjectId { get; set; }
	public Project? Project { get; set; }
}