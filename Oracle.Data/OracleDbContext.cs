#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data.Models;

#endregion

namespace Oracle.Data;

public partial class OracleDbContext(DbContextOptions<OracleDbContext> options, string path) : DbContext(options)
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite($"Data Source={path}");
	}

	public virtual DbSet<Character> Characters { get; set; }
	public virtual DbSet<Project> Projects { get; set; }
	public virtual DbSet<ProjectContributionType> ProjectContributionTypes { get; set; }
	public virtual DbSet<Activity> Activities { get; set; }
	public virtual DbSet<ActivityType> ActivityTypes { get; set; }
	public virtual DbSet<Adventure> Adventures { get; set; }
	public virtual DbSet<Adventure> CharacterAdventures { get; set; }
}