using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchemaMigrationTool.Data
{
    public class DesignTimeTenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        /*
         * for CLI tools like 'dotnet ef migrations add <Name>' or 'dotnet ef database update'
         */
        TenantDbContext IDesignTimeDbContextFactory<TenantDbContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>(); 
            // 設定連接字串，這裡使用範例連接字串，請根據實際情況修改
            var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TenantSampleDb;Trusted_Connection=True;"; 
            optionsBuilder.UseSqlServer(connectionString); // 預設 schema，可以根據需要修改
            return new TenantDbContext(optionsBuilder.Options, "tenantTest"); 
        } 
    }
}
