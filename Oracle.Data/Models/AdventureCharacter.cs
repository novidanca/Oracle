#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class AdventureCharacter
{
	[Key] public int Id { get; set; }
	public Adventure Adventure { get; set; }
	public Character Character { get; set; }
	public required int CharacterId { get; set; }
	public required int AdventureId { get; set; }
}