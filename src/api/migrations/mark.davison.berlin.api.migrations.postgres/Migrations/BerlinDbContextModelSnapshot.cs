﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using mark.davison.berlin.api.persistence;

#nullable disable

namespace mark.davison.berlin.api.migrations.postgres.Migrations
{
    [DbContext(typeof(BerlinDbContext))]
    partial class BerlinDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsUserSpecified")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentAuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("SiteId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ParentAuthorId");

                    b.HasIndex("SiteId");

                    b.HasIndex("UserId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Fandom", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ExternalName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsUserSpecified")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentFandomId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ParentFandomId");

                    b.HasIndex("UserId");

                    b.ToTable("Fandoms");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Job", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ContextUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("JobRequest")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("JobResponse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("JobType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PerformerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SelectedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SubmittedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ContextUserId");

                    b.HasIndex("UserId");

                    b.ToTable("Job");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Site", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LongName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Story", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Complete")
                        .HasColumnType("boolean");

                    b.Property<int?>("ConsumedChapters")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrentChapters")
                        .HasColumnType("integer");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Favourite")
                        .HasColumnType("boolean");

                    b.Property<DateOnly>("LastAuthored")
                        .HasColumnType("date");

                    b.Property<DateTime>("LastChecked")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid");

                    b.Property<int?>("TotalChapters")
                        .HasColumnType("integer");

                    b.Property<Guid>("UpdateTypeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.HasIndex("UpdateTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StoryAuthorLink", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("StoryId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("StoryId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryAuthorLinks");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StoryFandomLink", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FandomId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("StoryId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FandomId");

                    b.HasIndex("StoryId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryFandomLinks");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StoryUpdate", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("Complete")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrentChapters")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("LastAuthored")
                        .HasColumnType("date");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("StoryId")
                        .HasColumnType("uuid");

                    b.Property<int?>("TotalChapters")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryUpdates");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.UpdateType", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UpdateTypes");
                });

            modelBuilder.Entity("mark.davison.common.server.abstractions.Identification.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("Admin")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("First")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Last")
                        .IsRequired()
                        .HasMaxLength(62554)
                        .HasColumnType("character varying(62554)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Sub")
                        .HasColumnType("uuid");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Author", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Author", "ParentAuthor")
                        .WithMany()
                        .HasForeignKey("ParentAuthorId");

                    b.HasOne("mark.davison.berlin.shared.models.Entities.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId");

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentAuthor");

                    b.Navigation("Site");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Fandom", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Fandom", "ParentFandom")
                        .WithMany()
                        .HasForeignKey("ParentFandomId");

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentFandom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Job", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "ContextUser")
                        .WithMany()
                        .HasForeignKey("ContextUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContextUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Site", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Story", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.berlin.shared.models.Entities.UpdateType", "UpdateType")
                        .WithMany()
                        .HasForeignKey("UpdateTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Site");

                    b.Navigation("UpdateType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StoryAuthorLink", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.berlin.shared.models.Entities.Story", "Story")
                        .WithMany("StoryAuthorLinks")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Story");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StoryFandomLink", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Fandom", "Fandom")
                        .WithMany()
                        .HasForeignKey("FandomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.berlin.shared.models.Entities.Story", "Story")
                        .WithMany("StoryFandomLinks")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fandom");

                    b.Navigation("Story");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StoryUpdate", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Story", "Story")
                        .WithMany()
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Story");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.UpdateType", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Story", b =>
                {
                    b.Navigation("StoryAuthorLinks");

                    b.Navigation("StoryFandomLinks");
                });
#pragma warning restore 612, 618
        }
    }
}
