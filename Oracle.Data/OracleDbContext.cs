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

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Configure the many-to-many join table for Characters/Adventures
		modelBuilder.Entity<AdventureCharacter>()
			.HasKey(ac => new { ac.AdventureId, ac.CharacterId }); // Composite key

		modelBuilder.Entity<AdventureCharacter>()
			.HasOne(ac => ac.Adventure)
			.WithMany(a => a.AdventureCharacters)
			.HasForeignKey(ac => ac.AdventureId);

		modelBuilder.Entity<AdventureCharacter>()
			.HasOne(ac => ac.Character)
			.WithMany(c => c.AdventureCharacters)
			.HasForeignKey(ac => ac.CharacterId);
	}

	public virtual DbSet<Character> Characters { get; set; }
	public virtual DbSet<Project> Projects { get; set; }
	public virtual DbSet<ProjectContributionType> ProjectContributionTypes { get; set; }
	public virtual DbSet<Activity> Activities { get; set; }
	public virtual DbSet<ActivityType> ActivityTypes { get; set; }
	public virtual DbSet<Adventure> Adventures { get; set; }
	public virtual DbSet<AdventureCharacter> AdventureCharacters { get; set; }
	public virtual DbSet<CampaignSettings> CampaignSettings { get; set; }
	public virtual DbSet<Player> Players { get; set; }
	public virtual DbSet<CharacterStatus> CharacterStatuses { get; set; }
	public virtual DbSet<CharacterTimeline> CharacterTimelines { get; set; }
}