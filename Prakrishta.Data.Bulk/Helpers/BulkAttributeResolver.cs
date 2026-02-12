namespace Prakrishta.Data.Bulk.Helpers
{
    using Prakrishta.Data.Bulk.Attributes;
    using System.Reflection;

    internal static class BulkAttributeResolver
    {
        public static string? GetSchema<T>()
        {
            return typeof(T).GetCustomAttribute<BulkSchemaAttribute>()?.Schema;
        }

        public static string? GetTableName<T>()
        {
            return typeof(T).GetCustomAttribute<BulkTableAttribute>()?.TableName;
        }

        public static string? GetTvpName<T>()
        {
            return typeof(T).GetCustomAttribute<BulkTvpAttribute>()?.TvpName;
        }

        public static string? GetInsertProcedure<T>()
        {
            return typeof(T).GetCustomAttribute<BulkInsertProcedureAttribute>()?.ProcedureName;
        }

        public static string? GetUpdateProcedure<T>()
        {
            return typeof(T).GetCustomAttribute<BulkUpdateProcedureAttribute>()?.ProcedureName;
        }

        public static string? GetDeleteProcedure<T>()
        {
            return typeof(T).GetCustomAttribute<BulkDeleteProcedureAttribute>()?.ProcedureName;
        }
    }
}
