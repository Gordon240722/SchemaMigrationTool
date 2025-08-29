using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaMigrationTool.Logger
{
    public static class MigrationLogger
    {
        public static void Log(Migration migration, string schema, long id, string name, string status, string? errorMessage = null) {
            migration.Insert.IntoTable("MigrationLog").InSchema(schema).Row(new
            {
                TenantName = schema,
                MigrationId = id,
                MigrationName = name,
                ExecutedAt = DateTime.UtcNow,
                Status = status,
                ErrorMessage = errorMessage
            });
        }
    }
}
