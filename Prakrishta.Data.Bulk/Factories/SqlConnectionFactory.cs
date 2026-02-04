namespace Prakrishta.Data.Bulk.Factories
{
    using Microsoft.Data.SqlClient;
    using Prakrishta.Data.Bulk.Abstractions;
    using System.Data.Common;
    public sealed class SqlConnectionFactory : IDbConnectionFactory
    {
        public DbConnection Create(string connectionString)
            => new SqlConnection(connectionString);
    }
}
