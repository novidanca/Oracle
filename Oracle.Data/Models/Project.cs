#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class Project
{
	[Key] public int Id { get; set; }
	public required string Name { get; set; }
	public required ProjectContributionType ProjectContributionType { get; set; }
	public required int Goal { get; set; }
	public bool IsComplete { get; set; }
	public Character? OwningCharacter { get; set; }
}