﻿// <auto-generated />
using System;
using Diesel_modular_application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    [DbContext(typeof(DAdatabase))]
    [Migration("20241006105118_UPDATEnow")]
    partial class UPDATEnow
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Identity")
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Diesel_modular_application.Models.TableDieslovani", b =>
                {
                    b.Property<int>("IdDieslovani")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDieslovani"));

                    b.Property<int>("FirmaId")
                        .HasColumnType("int");

                    b.Property<int>("IDodstavky")
                        .HasColumnType("int");

                    b.Property<int>("IdTechnik")
                        .HasColumnType("int");

                    b.Property<DateTime>("Odchod")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Vstup")
                        .HasColumnType("datetime2");

                    b.HasKey("IdDieslovani");

                    b.HasIndex("FirmaId");

                    b.HasIndex("IDodstavky");

                    b.HasIndex("IdTechnik");

                    b.ToTable("TableDieslovani", "Data");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableFirma", b =>
                {
                    b.Property<int>("IDFirmy")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IDFirmy"));

                    b.Property<string>("NázevFirmy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<int>("RegionyIdRegion")
                        .HasColumnType("int");

                    b.HasKey("IDFirmy");

                    b.HasIndex("RegionyIdRegion");

                    b.ToTable("TableFirma", "Data");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableLokality", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adresa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Baterie")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DA")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Klasifikace")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lokalita")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Zásuvka")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LokalityTable", "Data");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableOdstavky", b =>
                {
                    b.Property<int>("IdOdstavky")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdOdstavky"));

                    b.Property<string>("Distributor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Do")
                        .HasColumnType("datetime2");

                    b.Property<string>("Firma")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LokalitaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Od")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Odchod")
                        .HasColumnType("datetime2");

                    b.Property<string>("Popis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Vstup")
                        .HasColumnType("datetime2");

                    b.HasKey("IdOdstavky");

                    b.HasIndex("LokalitaId");

                    b.ToTable("OdstavkyTable", "Data");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TablePohotovosti", b =>
                {
                    b.Property<int>("IdPohotovst")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPohotovst"));

                    b.Property<int>("IdTechnik")
                        .HasColumnType("int");

                    b.Property<DateTime>("Konec")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Začátek")
                        .HasColumnType("datetime2");

                    b.HasKey("IdPohotovst");

                    b.HasIndex("IdTechnik");

                    b.ToTable("TablePohotovosti", "Data");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableRegiony", b =>
                {
                    b.Property<int>("IdRegion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRegion"));

                    b.Property<string>("NazevRegionu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdRegion");

                    b.ToTable("TableRegiony", "Data");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableTechnik", b =>
                {
                    b.Property<int>("IdTechnika")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTechnika"));

                    b.Property<int>("FirmaId")
                        .HasColumnType("int");

                    b.Property<string>("Jmeno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<int>("RegionyIdRegion")
                        .HasColumnType("int");

                    b.Property<string>("Tel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdTechnika");

                    b.HasIndex("FirmaId");

                    b.HasIndex("RegionyIdRegion");

                    b.ToTable("TableTechnik", "Data");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Role", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("User", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens", "Identity");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableDieslovani", b =>
                {
                    b.HasOne("Diesel_modular_application.Models.TableFirma", "Firma")
                        .WithMany()
                        .HasForeignKey("FirmaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Diesel_modular_application.Models.TableOdstavky", "Odstavka")
                        .WithMany()
                        .HasForeignKey("IDodstavky")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Diesel_modular_application.Models.TableTechnik", "Technik")
                        .WithMany()
                        .HasForeignKey("IdTechnik")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Firma");

                    b.Navigation("Odstavka");

                    b.Navigation("Technik");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableFirma", b =>
                {
                    b.HasOne("Diesel_modular_application.Models.TableRegiony", "Regiony")
                        .WithMany()
                        .HasForeignKey("RegionyIdRegion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Regiony");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableOdstavky", b =>
                {
                    b.HasOne("Diesel_modular_application.Models.TableLokality", "Lokality")
                        .WithMany("OdstavkyList")
                        .HasForeignKey("LokalitaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lokality");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TablePohotovosti", b =>
                {
                    b.HasOne("Diesel_modular_application.Models.TableTechnik", "Technik")
                        .WithMany()
                        .HasForeignKey("IdTechnik")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Technik");
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableTechnik", b =>
                {
                    b.HasOne("Diesel_modular_application.Models.TableFirma", "Firma")
                        .WithMany()
                        .HasForeignKey("FirmaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Diesel_modular_application.Models.TableRegiony", "Regiony")
                        .WithMany()
                        .HasForeignKey("RegionyIdRegion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Firma");

                    b.Navigation("Regiony");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Diesel_modular_application.Models.TableLokality", b =>
                {
                    b.Navigation("OdstavkyList");
                });
#pragma warning restore 612, 618
        }
    }
}
