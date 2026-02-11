using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Abstractions
{
    internal interface IPartitionSwitchExecutor
    {
        Task<int> ReplacePartitionAsync<T>(
            BulkEngine engine,
            IEnumerable<T> items,
            string tableName,
            Action<PartitionSwitchOptions> configure,
            CancellationToken cancellationToken);
    }
}
