#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class CharacterAdventure
{
	[Key] public int Id { get; set; }
	public required Adventure Adventure { get; set; }
	public required Character Character { get; set; }
}