using Microsoft.EntityFrameworkCore;
using SchemaMigrationTool.Models;

namespace SchemaMigrationTool.Data
{
    public class TenantDbContext : DbContext
    {
        private readonly string _schema;

        public TenantDbContext(DbContextOptions<TenantDbContext> options, string schema) : base(options)
        {
            _schema = schema;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 將資料表綁定到指定 schema 下
            modelBuilder.Entity<Tenant>().ToTable("Tenants", _schema);
            modelBuilder.Entity<User>().ToTable("Users", _schema);
        }
    }
}
