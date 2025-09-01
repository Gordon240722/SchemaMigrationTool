using FluentMigrator;
using SchemaMigrationTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaMigrationTool.Migrations._20250901
{
    public class Age : Migration
    {
        private string _schema;
        public override void Down()
        {
        }

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
            Alter.Table("User").InSchema(_schema).AddColumn("Age").AsInt32().NotNullable().WithDefaultValue(0);
        }
    }
}
