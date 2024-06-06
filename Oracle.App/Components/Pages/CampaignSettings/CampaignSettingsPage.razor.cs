using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Oracle.Data.Models;
using Oracle.Logic.Services;

namespace Oracle.App.Components.Pages.CampaignSettings;
public partial class CampaignSettingsPage : OracleBasePage
{
	[Inject] private ActivityService ActivityService { get; set; } = null!;

	public List<Player> Players { get; set; } = new();
	public List<ActivityType> ActivityTypes { get; set; } = new();
	public List<ProjectContributionType> ProjectContributionTypes { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		Players = await Db.Players.ToListAsync();
		ActivityTypes = await ActivityService.GetActivityTypes();
		ProjectContributionTypes = await Db.ProjectContributionTypes.ToListAsync();
	}
}
