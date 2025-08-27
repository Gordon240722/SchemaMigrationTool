
using Microsoft.Extensions.Configuration;
using SchemaMigrationTool;

var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfigurationRoot configuration = builder.Build();

var migrationRunner = new MigrationRunner(configuration);

migrationRunner.FluentMigrateAllTenants();

//trigger workflow test