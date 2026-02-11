using Prakrishta.Data.Bulk.Abstractions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Extensions;

namespace Prakrishta.Data.Bulk.Internals
{
    internal sealed class PartitionSwitchExecutor : IPartitionSwitchExecutor
    {
        public Task<int> ReplacePartitionAsync<T>(
            BulkEngine engine,
            IEnumerable<T> items,
            string tableName,
            Action<PartitionSwitchOptions> configure,
            CancellationToken cancellationToken)
            => BulkEnginePartitionExtensions.ReplacePartitionAsync(
                engine, items, tableName, configure, cancellationToken);
    }
}
