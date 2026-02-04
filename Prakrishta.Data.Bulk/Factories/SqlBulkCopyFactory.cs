using Microsoft.Data.SqlClient;
using Prakrishta.Data.Bulk.Abstractions;
using System.Data.Common;

namespace Prakrishta.Data.Bulk.Factories
{
    public sealed class SqlBulkCopyFactory : IBulkCopyFactory
    {
        public IBulkCopy Create(DbConnection connection)
            => new SqlBulkCopyWrapper(new SqlBulkCopy(connection as SqlConnection));
    }
}
