#region using

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

#endregion

namespace Oracle.Data;

public class OracleDbContextFactory : IDesignTimeDbContextFactory<OracleDbContext>
{
	public OracleDbContext CreateDbContext(string[] args)
	{
		var path = args.Length > 0 ? args[0] : "C:\\Dev\\Personal\\Oracle\\Oracle.Data\\designTimeDb.campaign";

		var optionsBuilder = new DbContextOptionsBuilder<OracleDbContext>();
		optionsBuilder.UseSqlite($"Data Source={path}");

		return new OracleDbContext(optionsBuilder.Options, path);
	}
}