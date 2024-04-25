#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Adventure
{
	[Key] public int Id { get; set; }

	[Required] public string Name { get; set; }

	public string Description { get; set; } = "";
	public int StartDay { get; set; }
	public int Duration { get; set; } = 1;
	public bool IsStarted { get; set; }
	public bool IsComplete { get; set; }
	public ICollection<AdventureCharacter> AdventureCharacters { get; } = [];
}