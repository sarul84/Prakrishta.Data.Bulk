namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BulkColumnAttribute : Attribute
    {
        public string ColumnName { get; }
        public BulkColumnAttribute(string columnName) => ColumnName = columnName;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BulkIgnoreAttribute : Attribute { }
}
