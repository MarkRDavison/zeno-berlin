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
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FullPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsBackup")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ShareId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("SharingOptionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("StorageOptionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("ShareId");

                    b.HasIndex("SharingOptionsId");

                    b.HasIndex("StorageOptionsId");

                    b.HasIndex("UserId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("SharingOptionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("StorageOptionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SharingOptionsId");

                    b.HasIndex("StorageOptionsId");

                    b.HasIndex("UserId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Share", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.SharingOptions", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Public")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SharingOptions");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StorageOptions", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("Compressed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RetentionAmount")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("StorageOptions");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.UserOptions", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("MaxCapacity")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserOptions");
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

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Document", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.berlin.shared.models.Entities.Share", "Share")
                        .WithMany()
                        .HasForeignKey("ShareId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mark.davison.berlin.shared.models.Entities.SharingOptions", "SharingOptions")
                        .WithMany()
                        .HasForeignKey("SharingOptionsId");

                    b.HasOne("mark.davison.berlin.shared.models.Entities.StorageOptions", "StorageOptions")
                        .WithMany()
                        .HasForeignKey("StorageOptionsId");

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Share");

                    b.Navigation("SharingOptions");

                    b.Navigation("StorageOptions");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Location", b =>
                {
                    b.HasOne("mark.davison.berlin.shared.models.Entities.SharingOptions", "SharingOptions")
                        .WithMany()
                        .HasForeignKey("SharingOptionsId");

                    b.HasOne("mark.davison.berlin.shared.models.Entities.StorageOptions", "StorageOptions")
                        .WithMany()
                        .HasForeignKey("StorageOptionsId");

                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SharingOptions");

                    b.Navigation("StorageOptions");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.Share", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.SharingOptions", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.StorageOptions", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("mark.davison.berlin.shared.models.Entities.UserOptions", b =>
                {
                    b.HasOne("mark.davison.common.server.abstractions.Identification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
