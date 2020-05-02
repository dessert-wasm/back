using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dessert.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<Account, AccountRole, long>
    {
        public ApplicationDbContext()
        {
        }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<AuthToken> AuthTokens { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ModuleModuleTagRelation> ModuleModuleTagRelations { get; set; }
        public virtual DbSet<ModuleModuleReplacementRelation> ModuleModuleReplacementRelations { get; set; }
        public virtual DbSet<ModuleTag> ModuleTags { get; set; }
        public virtual DbSet<ModuleReplacement> ModuleReplacements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account
            modelBuilder.Entity<Account>()
                .ToTable("account");

            // Auth token
            modelBuilder.Entity<AuthToken>()
                .ToTable("auth_token");

            // Module
            modelBuilder.Entity<Module>()
                .ToTable("module");

            // Module tag
            modelBuilder.Entity<ModuleTag>()
                .ToTable("module_tag");

            // Module to tag relation
            modelBuilder.Entity<ModuleModuleTagRelation>()
                .ToTable("module__module_tag");

            modelBuilder.Entity<ModuleModuleTagRelation>()
                .HasKey(pt => new { pt.ModuleId, pt.ModuleTagId });

            // Module replacement
            modelBuilder.Entity<ModuleReplacement>()
                .ToTable("module_replacement");

            // Module to ModuleReplacement relation
            modelBuilder.Entity<ModuleModuleReplacementRelation>()
                .ToTable("module__module_replacement");

            modelBuilder.Entity<ModuleModuleReplacementRelation>()
                .HasKey(pt => new { pt.ModuleId, ModduleReplacementId = pt.ModuleReplacementId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlite("Data Source=db.sqlite");
        }
    }
}
