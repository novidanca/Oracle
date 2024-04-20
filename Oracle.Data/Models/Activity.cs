#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Activity
{
	[Key] public int Id { get; set; }
	public required ActivityType ActivityType { get; set; }
	public required Character Character { get; set; }
	public int Date { get; set; }
}