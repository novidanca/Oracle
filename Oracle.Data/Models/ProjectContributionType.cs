#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class ProjectContributionType
{
	[Key] public int Id { get; set; }
	[Required] public string Name { get; set; }
}