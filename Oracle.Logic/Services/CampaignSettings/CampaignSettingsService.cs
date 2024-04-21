#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Core.DatabaseManagement;
using Oracle.Data;

#endregion

namespace Oracle.Logic.Services.CampaignSettings;

public class CampaignSettingsService(OracleDbContext db, IDbPathService pathService) : ServiceBase(db)
{
	public async Task<Data.Models.CampaignSettings> GetCampaignSettings()
	{
		var settings = await Db.CampaignSettings.FirstOrDefaultAsync();
		if (settings != null) return settings;


		settings = new Data.Models.CampaignSettings()
		{
			CampaignName = "MyCampaign"
		};

		Db.CampaignSettings.Add(settings);
		await Db.SaveChangesAsync();

		return settings;
	}
}