
using Microsoft.Extensions.Configuration;
using SchemaMigrationTool;
using dotenv.net;

// 讀取 appsettings.json 配置
//var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

DotEnv.Load(); // 先讀 .env，設成環境變數

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables(); // 支援環境變數覆蓋

IConfigurationRoot configuration = builder.Build();

var migrationRunner = new MigrationRunner(configuration);

migrationRunner.FluentMigrateAllTenants();