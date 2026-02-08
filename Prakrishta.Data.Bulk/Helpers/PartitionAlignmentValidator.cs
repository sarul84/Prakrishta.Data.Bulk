namespace Prakrishta.Data.Bulk.Helpers
{
    using Microsoft.Data.SqlClient;

    public static class PartitionAlignmentValidator
    {
        public static async Task ValidateAsync(
            string targetTable,
            string stagingTable,
            int partitionNumber,
            string connectionString,
            CancellationToken cancellationToken)
        {
            var sql = $@"
                SELECT 
                    target_ps.name AS TargetPartitionScheme,
                    staging_ps.name AS StagingPartitionScheme,
                    target_pf.name AS TargetPartitionFunction,
                    staging_pf.name AS StagingPartitionFunction
                FROM sys.tables target
                JOIN sys.indexes target_i ON target.object_id = target_i.object_id AND target_i.index_id < 2
                JOIN sys.partition_schemes target_ps ON target_i.data_space_id = target_ps.data_space_id
                JOIN sys.partition_functions target_pf ON target_ps.function_id = target_pf.function_id
                JOIN sys.tables staging ON staging.name = PARSENAME('{stagingTable}', 1)
                JOIN sys.indexes staging_i ON staging.object_id = staging_i.object_id AND staging_i.index_id < 2
                JOIN sys.partition_schemes staging_ps ON staging_i.data_space_id = staging_ps.data_space_id
                JOIN sys.partition_functions staging_pf ON staging_ps.function_id = staging_pf.function_id
                WHERE target.name = PARSENAME('{targetTable}', 1);
                ";

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (!reader.Read())
                throw new InvalidOperationException("Unable to validate partition alignment.");

            var targetScheme = reader.GetString(0);
            var stagingScheme = reader.GetString(1);
            var targetFunction = reader.GetString(2);
            var stagingFunction = reader.GetString(3);

            if (targetScheme != stagingScheme || targetFunction != stagingFunction)
            {
                throw new InvalidOperationException(
                    $"Partition alignment mismatch. " +
                    $"Target uses ({targetScheme}/{targetFunction}), " +
                    $"Staging uses ({stagingScheme}/{stagingFunction}).");
            }
        }
    }
}
