﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.Data;

#nullable disable

namespace Oracle.Data.Migrations
{
    [DbContext(typeof(OracleDbContext))]
    [Migration("20240518172943_AddProjectToActivity")]
    partial class AddProjectToActivity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("Oracle.Data.Models.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActivityTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Date")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ActivityTypeId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("Oracle.Data.Models.ActivityType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectContributionAmount")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProjectContributionTypeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectContributionTypeId");

                    b.ToTable("ActivityTypes");
                });

            modelBuilder.Entity("Oracle.Data.Models.Adventure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsStarted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("StartDay")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Adventures");
                });

            modelBuilder.Entity("Oracle.Data.Models.AdventureCharacter", b =>
                {
                    b.Property<int>("AdventureId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.HasKey("AdventureId", "CharacterId");

                    b.HasIndex("CharacterId");

                    b.ToTable("AdventureCharacters");
                });

            modelBuilder.Entity("Oracle.Data.Models.CampaignSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CampaignName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CampaignSettings");
                });

            modelBuilder.Entity("Oracle.Data.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Oracle.Data.Models.CharacterStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanQuest")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("EndDay")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StartDay")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("ProjectId");

                    b.ToTable("CharacterStatuses");
                });

            modelBuilder.Entity("Oracle.Data.Models.CharacterTimeline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ActivityId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AdventureId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CharacterStatusId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EndDay")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StartDay")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("AdventureId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("CharacterStatusId");

                    b.ToTable("CharacterTimelines");
                });

            modelBuilder.Entity("Oracle.Data.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Oracle.Data.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Goal")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwningCharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProjectContributionTypeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwningCharacterId");

                    b.HasIndex("ProjectContributionTypeId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Oracle.Data.Models.ProjectContributionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ProjectContributionTypes");
                });

            modelBuilder.Entity("Oracle.Data.Models.Activity", b =>
                {
                    b.HasOne("Oracle.Data.Models.ActivityType", "ActivityType")
                        .WithMany()
                        .HasForeignKey("ActivityTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Oracle.Data.Models.Character", "Character")
                        .WithMany("Activities")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Oracle.Data.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.Navigation("ActivityType");

                    b.Navigation("Character");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Oracle.Data.Models.ActivityType", b =>
                {
                    b.HasOne("Oracle.Data.Models.ProjectContributionType", "ProjectContributionType")
                        .WithMany()
                        .HasForeignKey("ProjectContributionTypeId");

                    b.Navigation("ProjectContributionType");
                });

            modelBuilder.Entity("Oracle.Data.Models.AdventureCharacter", b =>
                {
                    b.HasOne("Oracle.Data.Models.Adventure", "Adventure")
                        .WithMany("AdventureCharacters")
                        .HasForeignKey("AdventureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Oracle.Data.Models.Character", "Character")
                        .WithMany("AdventureCharacters")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventure");

                    b.Navigation("Character");
                });

            modelBuilder.Entity("Oracle.Data.Models.Character", b =>
                {
                    b.HasOne("Oracle.Data.Models.Player", "Player")
                        .WithMany("Characters")
                        .HasForeignKey("PlayerId");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Oracle.Data.Models.CharacterStatus", b =>
                {
                    b.HasOne("Oracle.Data.Models.Character", "Character")
                        .WithMany("Statuses")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Oracle.Data.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.Navigation("Character");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Oracle.Data.Models.CharacterTimeline", b =>
                {
                    b.HasOne("Oracle.Data.Models.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId");

                    b.HasOne("Oracle.Data.Models.Adventure", "Adventure")
                        .WithMany()
                        .HasForeignKey("AdventureId");

                    b.HasOne("Oracle.Data.Models.Character", "Character")
                        .WithMany("Timeline")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Oracle.Data.Models.CharacterStatus", "Status")
                        .WithMany()
                        .HasForeignKey("CharacterStatusId");

                    b.Navigation("Activity");

                    b.Navigation("Adventure");

                    b.Navigation("Character");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Oracle.Data.Models.Project", b =>
                {
                    b.HasOne("Oracle.Data.Models.Character", "OwningCharacter")
                        .WithMany("Projects")
                        .HasForeignKey("OwningCharacterId");

                    b.HasOne("Oracle.Data.Models.ProjectContributionType", "ProjectContributionType")
                        .WithMany()
                        .HasForeignKey("ProjectContributionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OwningCharacter");

                    b.Navigation("ProjectContributionType");
                });

            modelBuilder.Entity("Oracle.Data.Models.Adventure", b =>
                {
                    b.Navigation("AdventureCharacters");
                });

            modelBuilder.Entity("Oracle.Data.Models.Character", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("AdventureCharacters");

                    b.Navigation("Projects");

                    b.Navigation("Statuses");

                    b.Navigation("Timeline");
                });

            modelBuilder.Entity("Oracle.Data.Models.Player", b =>
                {
                    b.Navigation("Characters");
                });
#pragma warning restore 612, 618
        }
    }
}
