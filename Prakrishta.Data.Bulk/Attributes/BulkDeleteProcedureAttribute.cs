namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BulkDeleteProcedureAttribute : Attribute
    {
        public string ProcedureName { get; }
        public BulkDeleteProcedureAttribute(string name) => ProcedureName = name;
    }
}
