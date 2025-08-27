using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaMigrationTool.Data
{
    public class TenantDbContextFactory
    {
        private readonly string _connectionString;

        public TenantDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TenantDbContext CreateDbContext(string schema)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new TenantDbContext(optionsBuilder.Options, schema);
        }
    }

}
