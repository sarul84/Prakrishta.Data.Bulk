namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BulkSchemaAttribute : Attribute
    {
        public string Schema { get; }
        public BulkSchemaAttribute(string schema) => Schema = schema;
    }
}
