using FluentMigrator;
using SchemaMigrationTool.Data;

namespace SchemaMigrationTool.Migrations._20250826
{
    [Migration(20250826001, "2025-08-26 17:40")]
    public class CreateUsers : Migration
    {
        private string _schema;
        public override void Up()
        {
            if (ApplicationContext is MigrationTenantContext tenantContext) {
                _schema = tenantContext.Schema;
            }
            else {
                throw new Exception("Missing tenant schema in ApplicationContext");
            }
        
            // 建立 Users 資料表
            Create.Table("Users")
                .InSchema(_schema)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Username").AsString(50).NotNullable()
                .WithColumn("Email").AsString(100).NotNullable();
        }
        public override void Down()
        {
            // 刪除 Users 資料表
            Delete.Table("Users");
        }
    }
}
