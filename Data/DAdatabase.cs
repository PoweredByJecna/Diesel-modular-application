using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diesel_modular_application.Data
{
    public class DAdatabase : IdentityDbContext
    {
        public DAdatabase(DbContextOptions<DAdatabase> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Identity");

            builder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            // Stávající konfigurace pro Lokality a Odstavky
            builder.Entity<TableLokality>(entity =>
            {
                entity.ToTable("LokalityTable", schema: "Data");
            });
            builder.Entity<TableOdstavky>(entity =>
            {
                entity.ToTable("OdstavkyTable", schema: "Data");
            });
            builder.Entity<TableDieslovani>(entity=>
            {
                entity.ToTable("TableDieslovani",schema: "Data");
            });
             builder.Entity<TableFirma>(entity=>
            {
                entity.ToTable("TableFirma",schema: "Data");
            });
             builder.Entity<TableRegiony>(entity=>
            {
                entity.ToTable("TableRegiony",schema: "Data");
            });
             builder.Entity<TablePohotovosti>(entity=>
            {
                entity.ToTable("TablePohotovosti",schema: "Data");
            });
            builder.Entity<TableTechnici>(entity=>
            {
                entity.ToTable("TableTechnici", schema:"Data");
            });
             

            // Přidání vztahu mezi Odstavky a Lokality
            builder.Entity<TableOdstavky>()
                .HasOne(o => o.Lokality) // Navigační vlastnost v Odstavky
                .WithMany(l => l.OdstavkyList) // Kolekce odstávek v Lokality
                .HasForeignKey(o => o.LokalitaId) // Cizí klíč v Odstavky
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TableDieslovani>()
                .HasOne(o=>o.Odstavka)
                .WithMany(I=>I.DieslovaniList)
                .HasForeignKey(o=>o.IDodstavky)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TablePohotovosti>()
                .HasOne(o=>o.User)
                .WithMany()
                .HasForeignKey(o=>o.IdUser);
            
            builder.Entity<TableDieslovani>()
                .HasOne(o=>o.Firma)
                .WithMany(i=>i.DieslovaniList)
                .HasForeignKey(o=>o.FirmaId);

            builder.Entity<TableDieslovani>()
                .HasOne(o=>o.Technik)
                .WithMany(i=>i.DieslovaniList)
                .HasForeignKey(o=>o.IdTechnik);
                
            builder.Entity<TableTechnici>()
                .HasOne(o=>o.User)
                .WithMany()
                .HasForeignKey(o=>o.IdUser);

            builder.Entity<TableRegiony>()
                .HasOne(o=>o.Firma)
                .WithMany()
                .HasForeignKey(i=>i.FirmaID);

        

        }

        public DbSet<TableLokality> LokalityS {get; set;}
        public DbSet<TableOdstavky> OdstavkyS {get;set;}
        public DbSet<TableDieslovani> DieslovaniS {get;set;}
        public DbSet<TableFirma> FrimaS{get;set;}
        public DbSet<TableRegiony>ReginoS{get;set;}
        public DbSet<TablePohotovosti> Pohotovts{get;set;}
        public DbSet<TableTechnici> TechniS{get;set;}
        
    }
}
