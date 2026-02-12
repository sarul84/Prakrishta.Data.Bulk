namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BulkUpdateProcedureAttribute : Attribute
    {
        public string ProcedureName { get; }
        public BulkUpdateProcedureAttribute(string name) => ProcedureName = name;
    }
}
