#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class CharacterTimeline
{
	[Key] public int Id { get; set; }

	[Required] public int CharacterId { get; set; }
	public Character Character { get; set; }

	[Required] public int StartDay { get; set; }
	public int? EndDay { get; set; }


	public int? AdventureId { get; set; }
	public Adventure? Adventure { get; set; }


	public int? ActivityId { get; set; }
	public Activity? Activity { get; set; }

	public int? CharacterStatusId { get; set; }
	public CharacterStatus? Status { get; set; }
}