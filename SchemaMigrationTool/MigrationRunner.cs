using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchemaMigrationTool.Data;
using SchemaMigrationTool.Models;

namespace SchemaMigrationTool
{
    public class MigrationRunner
    {
        private readonly IConfiguration _configuration;

        public MigrationRunner(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void MigrateAllTenants() { 
        
            var tenants = _configuration.GetSection("Tenants").Get<List<TenantSettings>>(); // 讀取租戶
            var connectionString = _configuration.GetConnectionString("DefaultConnection"); // 讀取連線字串

            if (tenants == null || tenants.Count == 0)
            {
                Console.WriteLine("沒有找到任何租戶設定，請檢查 appsettings.json 中的 Tenants 設定。");
                return;
            }
                
            foreach (var tenant in tenants)
            {
                try
                {
                    Console.WriteLine($"正在執行 Schema Migrate: {tenant.Name}（Schema: {tenant.Schema}）");
                    // 使用 DesignTimeTenantDbContextFactory 來建立 DbContext
                    var factory = new TenantDbContextFactory(connectionString);
                    using var context = factory.CreateDbContext(tenant.Schema);
                    // 執行遷移
                    context.Database.Migrate();
                    Console.WriteLine($"租戶 {tenant.Name} 的資料庫遷移完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"為租戶 {tenant.Name} 執行資料庫遷移時發生錯誤: {ex.Message}");
                }
            }
        }

        public void FluentMigrateAllTenants()
        {

            var tenants = _configuration.GetSection("Tenants").Get<List<TenantSettings>>(); // 讀取租戶
            var connectionString = _configuration.GetConnectionString("DefaultConnection"); // 讀取連線字串

            if (tenants == null || tenants.Count == 0)
            {
                Console.WriteLine("沒有找到任何租戶設定，請檢查 appsettings.json 中的 Tenants 設定。");
                return;
            }

            foreach (var tenant in tenants)
            {
                try
                {
                    Console.WriteLine($"正在執行 Schema Migrate: {tenant.Name}（Schema: {tenant.Schema}）");

                    var serviceProvider = new ServiceCollection()
                        .AddFluentMigratorCore()
                        .ConfigureRunner(rb => rb
                            .AddSqlServer()
                            .WithGlobalConnectionString(connectionString)
                            .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations()
                            .WithVersionTable(new TenantVersionTableMetaData(tenant.Schema))
                        )
                        .Configure<RunnerOptions>(opt => { 
                            opt.ApplicationContext = new MigrationTenantContext { Schema = tenant.Schema };
                        })
                        .AddLogging(lb => lb.AddFluentMigratorConsole())
                        .BuildServiceProvider(false);

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                        runner.MigrateUp();
                    }

                    Console.WriteLine($"租戶 {tenant.Name} 的資料庫遷移完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"為租戶 {tenant.Name} 執行資料庫遷移時發生錯誤: {ex.Message}");
                }
            }
        }
    }
}
