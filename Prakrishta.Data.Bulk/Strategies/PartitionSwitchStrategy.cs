using Microsoft.Data.SqlClient;
using Prakrishta.Data.Bulk.Abstractions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Extensions;
using Prakrishta.Data.Bulk.Helpers;
using Prakrishta.Data.Bulk.Mapping;

namespace Prakrishta.Data.Bulk.Engine.Strategies;

public sealed class PartitionSwitchStrategy : IBulkStrategy
{
    private readonly IBulkCopyFactory _bulkCopyFactory;
    private readonly IDbConnectionFactory _connectionFactory;

    public PartitionSwitchStrategy(IBulkCopyFactory bulkCopyFactory, IDbConnectionFactory connectionFactory)
    {
        _bulkCopyFactory = bulkCopyFactory;
        _connectionFactory = connectionFactory;
    }

    public BulkStrategyKind Kind => BulkStrategyKind.PartitionSwitch;

    public async Task<int> ExecuteAsync(
        BulkContext context,
        string connectionString,
        CancellationToken cancellationToken)
    {
        var targetTable = context.TableName;
        var stagingTable = GetRequired<string>(context, "StagingTable");
        var partitionNumber = GetRequired<int>(context, "PartitionNumber");
        var columnMaps = GetRequired<ColumnMap[]>(context, "ColumnMaps");
        var items = (IEnumerable<object>)context.Items;

        // 1. Create staging table
        await CreateStagingTableAsync(targetTable, stagingTable, connectionString, cancellationToken);

        // 2. Bulk load into staging table
        await BulkLoadAsync(stagingTable, items, columnMaps, connectionString, cancellationToken);

        // 3. Validate partition alignment
        await PartitionAlignmentValidator.ValidateAsync(
            targetTable,
            stagingTable,
            partitionNumber,
            connectionString,
            cancellationToken);

        // 4. Switch partition
        await SwitchPartitionAsync(stagingTable, targetTable, partitionNumber, connectionString, cancellationToken);

        return context.ItemCount;
    }

    private static T GetRequired<T>(BulkContext ctx, string key)
    {
        if (!ctx.Properties.TryGetValue(key, out var value) || value is not T typed)
            throw new InvalidOperationException($"BulkContext.Properties missing required key '{key}'.");
        return typed;
    }

    private async Task BulkLoadAsync(
        string stagingTable,
        IEnumerable<object> items,
        ColumnMap[] columnMaps,
        string connectionString,
        CancellationToken cancellationToken)
    {
        var table = items.ToDataTable(columnMaps);

        var conn = _connectionFactory.Create(connectionString);

        await using var bulk = _bulkCopyFactory.Create(conn);
        bulk.DestinationTableName = stagingTable;

        foreach (var map in columnMaps)
            bulk.ColumnMappings.Add((map.ColumnName, map.ColumnName));

        await bulk.WriteToServerAsync(table, cancellationToken);
    }

    private async Task CreateStagingTableAsync(
        string targetTable,
        string stagingTable,
        string connectionString,
        CancellationToken cancellationToken)
    {
        var sql = $@"
            IF OBJECT_ID('{stagingTable}', 'U') IS NOT NULL
                DROP TABLE {stagingTable};

            SELECT TOP 0 *
            INTO {stagingTable}
            FROM {targetTable};
            ";

        await ExecuteNonQueryAsync(connectionString, sql, cancellationToken);
    }

    private async Task SwitchPartitionAsync(
        string stagingTable,
        string targetTable,
        int partitionNumber,
        string connectionString,
        CancellationToken cancellationToken)
    {
        var sql = $@"
            ALTER TABLE {stagingTable}
            SWITCH PARTITION {partitionNumber}
            TO {targetTable} PARTITION {partitionNumber};
            ";

        await ExecuteNonQueryAsync(connectionString, sql, cancellationToken);
    }

    private static async Task ExecuteNonQueryAsync(
        string connectionString,
        string sql,
        CancellationToken cancellationToken)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}