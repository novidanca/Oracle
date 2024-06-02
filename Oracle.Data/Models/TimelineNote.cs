#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class TimelineNote
{
	[Key] public int Id { get; set; }

	[Required] public string Note { get; set; }
	[Required] public int StartDate { get; set; }
	public int? EndDate { get; set; }

	public int? CharacterId { get; set; }
	public Character Character { get; set; }
}