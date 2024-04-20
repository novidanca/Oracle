﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.Data;

#nullable disable

namespace Oracle.Data.Migrations
{
    [DbContext(typeof(OracleDbContext))]
    partial class OracleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.HasKey("Id");

                    b.HasIndex("ActivityTypeId");

                    b.HasIndex("CharacterId");

                    b.ToTable("Activities", (string)null);
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

                    b.ToTable("AcivityTypes", (string)null);
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("StartDay")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Adventure", (string)null);
                });

            modelBuilder.Entity("Oracle.Data.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AdventureId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AdventureId");

                    b.ToTable("Characters", (string)null);
                });

            modelBuilder.Entity("Oracle.Data.Models.CharacterAdventure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdventureId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AdventureId");

                    b.HasIndex("CharacterId");

                    b.ToTable("CharacterAdventure", (string)null);
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

                    b.ToTable("Projects", (string)null);
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

                    b.ToTable("ProjectContributionTypes", (string)null);
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

                    b.Navigation("ActivityType");

                    b.Navigation("Character");
                });

            modelBuilder.Entity("Oracle.Data.Models.ActivityType", b =>
                {
                    b.HasOne("Oracle.Data.Models.ProjectContributionType", "ProjectContributionType")
                        .WithMany()
                        .HasForeignKey("ProjectContributionTypeId");

                    b.Navigation("ProjectContributionType");
                });

            modelBuilder.Entity("Oracle.Data.Models.Character", b =>
                {
                    b.HasOne("Oracle.Data.Models.Adventure", null)
                        .WithMany("Characters")
                        .HasForeignKey("AdventureId");
                });

            modelBuilder.Entity("Oracle.Data.Models.CharacterAdventure", b =>
                {
                    b.HasOne("Oracle.Data.Models.Adventure", "Adventure")
                        .WithMany()
                        .HasForeignKey("AdventureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Oracle.Data.Models.Character", "Character")
                        .WithMany("Adventures")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventure");

                    b.Navigation("Character");
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
                    b.Navigation("Characters");
                });

            modelBuilder.Entity("Oracle.Data.Models.Character", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Adventures");

                    b.Navigation("Projects");
                });
#pragma warning restore 612, 618
        }
    }
}
