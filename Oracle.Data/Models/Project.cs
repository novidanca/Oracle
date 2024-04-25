#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Project
{
	[Key] public int Id { get; set; }
	[Required] public string Name { get; set; }

	[Required] public int ProjectContributionTypeId { get; set; }
	public ProjectContributionType ProjectContributionType { get; set; }

	[Required] public int Goal { get; set; }
	public bool IsComplete { get; set; }

	public int? OwningCharacterId { get; set; }
	public Character? OwningCharacter { get; set; }
}