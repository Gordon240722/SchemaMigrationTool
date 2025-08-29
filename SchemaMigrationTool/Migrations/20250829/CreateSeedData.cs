using FluentMigrator;
using SchemaMigrationTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaMigrationTool.Migrations._20250829
{
    [Migration(20250829001, "2025-08-29 14:00")]
    public class CreateSeedData : Migration
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
            Insert.IntoTable("Users").InSchema(_schema).Row(
                new
                {
                    Username = "admin",
                    Email = "admin@example.com"
                }
            );
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
            Delete.FromTable("Users").InSchema(_schema).Row(
                new
                {
                    Username = "admin",
                    Email = "admin@example.com"
                }
            );
        }
    }
}
