#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Adventure
{
	[Key] public int Id { get; set; }

	public required string Name { get; set; }

	public string Description { get; set; } = "";
	public int StartDay { get; set; }
	public int Duration { get; set; } = 1;
	public bool IsComplete { get; set; }
	public ICollection<Character> Characters { get; } = [];
}