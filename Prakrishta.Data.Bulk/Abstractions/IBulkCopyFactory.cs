namespace Prakrishta.Data.Bulk.Abstractions
{
    using System.Data.Common;
    public interface IBulkCopyFactory
    {
        IBulkCopy Create(DbConnection connection);
    }
}
