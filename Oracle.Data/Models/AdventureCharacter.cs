#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class AdventureCharacter
{
	[Key] public int Id { get; set; }

	[Required] public required int AdventureId { get; set; }
	public Adventure Adventure { get; set; }

	[Required] public required int CharacterId { get; set; }
	public Character Character { get; set; }
}