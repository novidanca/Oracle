#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class CharacterCondition
{
	[Key] public int Id { get; set; }

	[Required] public string Description { get; set; }
	[Required] public int StartDay { get; set; }
	public int? EndDay { get; set; }

	[Required] public int CharacterId { get; set; }
	public Character Character { get; set; }
}