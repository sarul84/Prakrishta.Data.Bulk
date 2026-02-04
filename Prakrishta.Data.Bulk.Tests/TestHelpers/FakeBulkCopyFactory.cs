using Microsoft.Data.SqlClient;
using Prakrishta.Data.Bulk.Abstractions;
using System.Data.Common;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public sealed class FakeBulkCopyFactory : IBulkCopyFactory
    {
        public FakeBulkCopy Instance { get; } = new();

        public IBulkCopy Create(DbConnection connection)
            => Instance;
    }
}
