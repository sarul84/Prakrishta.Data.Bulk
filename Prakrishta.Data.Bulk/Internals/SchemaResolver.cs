namespace Prakrishta.Data.Bulk.Internals
{
    using Prakrishta.Data.Bulk.Abstractions;

    internal sealed class SchemaResolver(IDbConnectionFactory factory, string connectionString) : ISchemaResolver
    {
        private readonly IDbConnectionFactory _factory = factory;
        private readonly string _connectionString = connectionString;

        public async Task<string> ResolveSchemaAsync(string tableName, CancellationToken cancellationToken)
        {
            await using var conn = _factory.Create(_connectionString);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            SELECT TABLE_SCHEMA 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = @name";

            var p = cmd.CreateParameter();
            p.ParameterName = "@name";
            p.Value = tableName;
            cmd.Parameters.Add(p);

            var schemas = new List<string>();

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
                schemas.Add(reader.GetString(0));

            if (schemas.Count == 0)
                throw new InvalidOperationException(
                    $"Table '{tableName}' not found in any schema.");

            if (schemas.Count > 1)
                throw new InvalidOperationException(
                    $"Table '{tableName}' exists in multiple schemas: {string.Join(", ", schemas)}. " +
                    "Specify schema explicitly using .InSchema(\"schema\").");

            return schemas[0];
        }
    }
}
