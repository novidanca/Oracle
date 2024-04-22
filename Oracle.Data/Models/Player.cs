#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Player
{
	[Key] public int Id { get; set; }
	public required string Name { get; set; }
	public ICollection<Character> Characters { get; set; }
}