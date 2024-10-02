using Diesel_modular_application.Models;
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
            builder.Entity<LokalityTable>(entity =>
            {
                entity.ToTable("LokalityTable", schema: "Data");
            });
            builder.Entity<OdstavkyTable>(entity =>
            {
                entity.ToTable("OdstavkyTable", schema: "Data");
            });

            // Přidání vztahu mezi Odstavky a Lokality
            builder.Entity<OdstavkyTable>()
                .HasOne(o => o.Lokality) // Navigační vlastnost v Odstavky
                .WithMany(l => l.OdstavkyList) // Kolekce odstávek v Lokality
                .HasForeignKey(o => o.LokalitaId); // Cizí klíč v Odstavky
        }

        public DbSet<LokalityTable> LokalityS {get; set;}
        public DbSet<OdstavkyTable> OdstavkyS {get;set;}
    }
}
