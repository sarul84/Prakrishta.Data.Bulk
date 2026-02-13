namespace Prakrishta.Data.Bulk.Strategies
{
    using Prakrishta.Data.Bulk.Abstractions;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Extensions;
    using Prakrishta.Data.Bulk.Factories;
    using Prakrishta.Data.Bulk.Mapping;
    using Prakrishta.Data.Bulk.SqlGeneration;
    using System.Data.Common;

    public sealed class StagingTableStrategy(BulkOptions options, IBulkCopyFactory bulkCopyFactory, IDbConnectionFactory connectionFactory) : IBulkStrategy
    {
        public BulkStrategyKind Kind => BulkStrategyKind.StagingTable;

        private readonly BulkOptions _options = options;
        private readonly IBulkCopyFactory _bulkCopyFactory = bulkCopyFactory;
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

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

                // INSERT PATH
                if (context.OperationKind == BulkOperationKind.Insert)
                {
                    var table = list.ToDataTable(maps);

                    await using var bulk = _bulkCopyFactory.Create(conn);
                    bulk.DestinationTableName = context.TableName;

                    foreach (var map in maps)
                        bulk.ColumnMappings.Add((map.ColumnName, map.ColumnName));

                    await bulk.WriteToServerAsync(table, cancellationToken);

                    return list.Count;
                }

                // UPDATE / DELETE PATH
                var stagingTable = _options.StagingTableNameFactory(context.TableName);

                await EnsureStagingTableAsync(conn, context.TableName, stagingTable, cancellationToken);

                var stagingData = list.ToDataTable(maps);

                await using (var bulk = _bulkCopyFactory.Create(conn))
                {
                    bulk.DestinationTableName = stagingTable;

                    foreach (var map in maps)
                        bulk.ColumnMappings.Add((map.ColumnName, map.ColumnName));

                    await bulk.WriteToServerAsync(stagingData, cancellationToken);
                }

                var sql = context.OperationKind switch
                {
                    BulkOperationKind.Update => BulkSqlGenerator.GenerateUpdateSql(context.TableName, stagingTable, maps),
                    BulkOperationKind.Delete => BulkSqlGenerator.GenerateDeleteSql(context.TableName, stagingTable, maps),
                    _ => throw new NotSupportedException($"Operation {context.OperationKind} not supported by staging strategy.")
                };

                context.Properties["LastExecutedSql"] = sql;

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);

                await using var cleanup = conn.CreateCommand();
                cleanup.CommandText = $"TRUNCATE TABLE {stagingTable};";
                await cleanup.ExecuteNonQueryAsync(cancellationToken);

                return affected;
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

        private static async Task EnsureStagingTableAsync(
            DbConnection conn,
            string targetTable,
            string stagingTable,
            CancellationToken cancellationToken)
        {
            var sql = $@"
                IF OBJECT_ID('{stagingTable}', 'U') IS NULL
                BEGIN
                    SELECT TOP 0 * INTO {stagingTable} FROM {targetTable};
                END
                ELSE
                BEGIN
                    TRUNCATE TABLE {stagingTable};
                END";

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
