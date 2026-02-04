using Prakrishta.Data.Bulk.Mapping;

namespace Prakrishta.Data.Bulk.SqlGeneration
{
    public static class BulkSqlGenerator
    {
        public static string GenerateUpdateSql(
            string tableName,
            string stagingTableName,
            ColumnMap[] maps)
        {
            var keyColumns = maps.Where(m => m.IsKey).ToArray();
            var nonKeyColumns = maps.Where(m => !m.IsKey).ToArray();

            if (keyColumns.Length == 0)
                throw new InvalidOperationException("At least one key column is required for UPDATE.");

            var setClauses = string.Join(", ",
                nonKeyColumns.Select(m => $"T.[{m.ColumnName}] = S.[{m.ColumnName}]"));

            var joinCondition = string.Join(" AND ",
                keyColumns.Select(m => $"T.[{m.ColumnName}] = S.[{m.ColumnName}]"));

            return $@"
                    UPDATE T
                    SET {setClauses}
                    FROM {tableName} AS T
                    JOIN {stagingTableName} AS S ON {joinCondition};";
        }

        public static string GenerateDeleteSql(
            string tableName,
            string stagingTableName,
            ColumnMap[] maps)
        {
            var keyColumns = maps.Where(m => m.IsKey).ToArray();

            if (keyColumns.Length == 0)
                throw new InvalidOperationException("At least one key column is required for DELETE.");

            var joinCondition = string.Join(" AND ",
                keyColumns.Select(m => $"T.[{m.ColumnName}] = S.[{m.ColumnName}]"));

            return $@"
                    DELETE T
                    FROM {tableName} AS T
                    JOIN {stagingTableName} AS S ON {joinCondition};";
        }
    }
}
