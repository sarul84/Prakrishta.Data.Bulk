namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BulkInsertProcedureAttribute : Attribute
    {
        public string ProcedureName { get; }
        public BulkInsertProcedureAttribute(string name) => ProcedureName = name;
    }

}
