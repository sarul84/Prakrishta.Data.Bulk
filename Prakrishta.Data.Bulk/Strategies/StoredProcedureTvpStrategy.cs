namespace Prakrishta.Data.Bulk.Strategies
{
    using Prakrishta.Data.Bulk.Abstractions;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Extensions;
    using Prakrishta.Data.Bulk.Factories;
    using Prakrishta.Data.Bulk.Mapping;
    using System.Data;
    using System.Data.Common;

    public sealed class StoredProcedureTvpStrategy(IDbConnectionFactory connectionFactory) : IBulkStrategy
    {
        public BulkStrategyKind Kind => BulkStrategyKind.StoredProcedureTvp;

        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<int> ExecuteAsync(
            BulkContext context,
            string connectionString,
            CancellationToken cancellationToken)
        {
            if (context.TableTypeName is null)
                throw new InvalidOperationException("TableTypeName is required for TVP strategy.");
            if (context.ProcedureName is null)
                throw new InvalidOperationException("ProcedureName is required for TVP strategy.");

            var list = (IList<object>)context.Properties["List"]!;
            var maps = (ColumnMap[])context.Properties["ColumnMaps"]!;

            var table = list.ToDataTable(maps);

            DbConnection conn = null;

            try
            {
                conn = _connectionFactory.Create(connectionString);
                await conn.OpenAsync(cancellationToken);

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = context.ProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (cmd.Parameters is Microsoft.Data.SqlClient.SqlParameterCollection sqlParameters)
                {
                    sqlParameters.AddWithValue("@Items", table);
                }

                cmd.AddStructured("@Items", table, context.TableTypeName);

                return await cmd.ExecuteNonQueryAsync(cancellationToken);
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
