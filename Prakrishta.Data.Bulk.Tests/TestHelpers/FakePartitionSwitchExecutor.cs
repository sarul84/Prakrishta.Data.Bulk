using Prakrishta.Data.Bulk.Abstractions;
using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    internal sealed class FakePartitionSwitchExecutor : IPartitionSwitchExecutor
    {
        public IEnumerable<object>? LastItems { get; private set; }
        public string? LastTableName { get; private set; }
        public Action<PartitionSwitchOptions>? LastOptions { get; private set; }

        public Task<int> ReplacePartitionAsync<T>(
            BulkEngine engine,
            IEnumerable<T> items,
            string tableName,
            Action<PartitionSwitchOptions> configure,
            CancellationToken cancellationToken)
        {
            LastItems = items.Cast<object>().ToList();
            LastTableName = tableName;
            LastOptions = configure;
            return Task.FromResult(999);
        }
    }
}
