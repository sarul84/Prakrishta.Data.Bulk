using Prakrishta.Data.Bulk.Mapping;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public sealed class FakeSqlBulkCopyHelper
    {
        public bool WasCalled { get; private set; }

        public Task WriteAsync(
            string table,
            IEnumerable<object> items,
            IReadOnlyList<ColumnMap> maps)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }
}
