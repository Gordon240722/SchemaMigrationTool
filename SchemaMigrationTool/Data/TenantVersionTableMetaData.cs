using FluentMigrator.Runner.VersionTableInfo;

namespace SchemaMigrationTool.Data
{
    public class TenantVersionTableMetaData : IVersionTableMetaData
    {
        private readonly string _schema;
        public TenantVersionTableMetaData(string schema)
        {
            _schema = schema;
        }

        public object ApplicationContext => null;
        public bool OwnsSchema => false;
        public string SchemaName => _schema;
        public string TableName => "VersionInfo";
        public string ColumnName => "Version";
        public string DescriptionColumnName => "Description";
        public string UniqueIndexName => "UC_Version";
        public string AppliedOnColumnName => "AppliedOn";
        object IVersionTableMetaData.ApplicationContext { get => ApplicationContext; set => throw new NotImplementedException(); }

    }
}
