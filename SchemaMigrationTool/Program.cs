
using Microsoft.Extensions.Configuration;
using SchemaMigrationTool;

var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

IConfigurationRoot configuration = builder.Build();

var migrationRunner = new MigrationRunner(configuration);

migrationRunner.FluentMigrateAllTenants();