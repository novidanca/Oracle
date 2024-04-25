#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class ActivityType
{
	[Key] public int Id { get; set; }
	[Required] public string Name { get; set; }

	public int? ProjectContributionTypeId { get; set; }
	public ProjectContributionType? ProjectContributionType { get; set; }
	public int ProjectContributionAmount { get; set; }
}