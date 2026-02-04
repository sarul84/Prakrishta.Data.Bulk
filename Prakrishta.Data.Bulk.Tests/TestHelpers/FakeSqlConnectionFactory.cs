using Prakrishta.Data.Bulk.Abstractions;
using System.Data.Common;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public sealed class FakeDbConnectionFactory : IDbConnectionFactory
    {
        public FakeSqlConnection Instance { get; } = new();

        public DbConnection Create(string connectionString)
            => Instance;
    }
}
