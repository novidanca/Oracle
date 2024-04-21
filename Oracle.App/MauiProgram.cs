#region using

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Oracle.Core.DatabaseManagement;
using Oracle.Core.ServiceManagement;
using Oracle.Data;

#endregion

namespace Oracle.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		Preferences.Set("DbInitialized", false);

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
		builder.Services.AddSingleton<IDbPathService, MauiDbPathService>();
		builder.Services.AddTransient<OracleDbContext>(serviceProvider =>
		{
			var dbPathService = serviceProvider.GetRequiredService<IDbPathService>();
			var dbPath = dbPathService.GetPath() ?? dbPathService.MakePath("MyCampaign").Result ?? "";

			if (dbPathService.IsDbInitialized()) dbPathService.SetPath(dbPath);

			return new OracleDbContext(new DbContextOptions<OracleDbContext>(), dbPath);
		});

		builder.Services.AddMudServices();
		builder.Services.AddAutoRegisteredServices();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}