namespace Prakrishta.Data.Bulk.Strategies
{
    using Prakrishta.Data.Bulk.Abstractions;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Extensions;
    using Prakrishta.Data.Bulk.Factories;
    using Prakrishta.Data.Bulk.Mapping;
    using System.Data.Common;

    public sealed class TruncateAndReloadStrategy(
        BulkOptions options,
        IBulkCopyFactory bulkCopyFactory,
        IDbConnectionFactory connectionFactory) : IBulkStrategy
    {
        private readonly BulkOptions _options = options;
        private readonly IBulkCopyFactory _bulkCopyFactory = bulkCopyFactory;
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public BulkStrategyKind Kind => BulkStrategyKind.TruncateAndReload;

        public async Task<int> ExecuteAsync(
            BulkContext context,
            string connectionString,
            CancellationToken cancellationToken)
        {
            var list = (IList<object>)context.Properties["List"]!;
            var maps = (ColumnMap[])context.Properties["ColumnMaps"]!;

            if (list.Count == 0)
                return 0;

            DbConnection conn = null;

            try
            {
                conn = _connectionFactory.Create(connectionString);
                await conn.OpenAsync(cancellationToken);

                // 1. TRUNCATE
                var truncateSql = $"TRUNCATE TABLE {context.TableName};";

                context.Properties["LastExecutedSql"] = truncateSql;

                await using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = truncateSql;
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                // 2. BULK INSERT
                var table = list.ToDataTable(maps);

                await using var bulk = _bulkCopyFactory.Create(conn);
                bulk.DestinationTableName = context.TableName;

                foreach (var map in maps)
                    bulk.ColumnMappings.Add((map.ColumnName, map.ColumnName));

                await bulk.WriteToServerAsync(table, cancellationToken);

                return list.Count;
            }
           finally
            {
                if (conn != null)
                {
                    if (_connectionFactory is PooledConnectionFactory pooled)
                        pooled.Return(conn);
                    else
                        await conn.DisposeAsync();
                }
            }
        }
    }
}
