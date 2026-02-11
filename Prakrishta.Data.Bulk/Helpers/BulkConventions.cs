namespace Prakrishta.Data.Bulk.Helpers
{
    public static class BulkConventions
    {
        public static string ResolveTableName<T>()
            => $"dbo.{typeof(T).Name}";

        public static string ResolveTvpName<T>()
            => $"dbo.{typeof(T).Name}Type";

        public static string ResolveProcedureName<T>(string operation)
            => $"dbo.{typeof(T).Name}_{operation}";
    }
}
