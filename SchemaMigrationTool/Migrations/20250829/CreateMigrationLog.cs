using FluentMigrator;
using SchemaMigrationTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaMigrationTool.Migrations._20250829
{
    [Migration(20250829002, "2025-08-29 14:30")]
    public class CreateMigrationLog : Migration
    {
        private string _schema;
        public override void Up()
        {
            if (ApplicationContext is MigrationTenantContext tenantContext)
            {
                _schema = tenantContext.Schema;
            }
            else
            {
                throw new Exception("Missing tenant schema in ApplicationContext");
            }
            Create.Table("MigrationLog")
                .InSchema(_schema)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("TenantName").AsString(100).NotNullable()
                .WithColumn("MigrationId").AsInt64().NotNullable()
                .WithColumn("MigrationName").AsString(255).NotNullable()
                .WithColumn("ExecutedAt").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("Status").AsString(20).NotNullable()
                .WithColumn("ErrorMessage").AsString(int.MaxValue).Nullable();
        }
        public override void Down()
        {
            if (ApplicationContext is MigrationTenantContext tenantContext)
            {
                _schema = tenantContext.Schema;
            }
            else
            {
                throw new Exception("Missing tenant schema in ApplicationContext");
            }
            Delete.Table("MigrationLog");
        }
    }
}
