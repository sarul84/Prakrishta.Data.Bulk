namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BulkTableAttribute : Attribute
    {
        public string TableName { get; }
        public BulkTableAttribute(string tableName) => TableName = tableName;
    }
}
