using Prakrishta.Data.Bulk.Mapping;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public static class ColumnMapTestFactory
    {
        public static ColumnMap[] Create()
            => new[]
            {
            new ColumnMap
            {
                ColumnName = "Id",
                ColumnType = typeof(int),
                Getter = o => ((TestEntity)o).Id,
                IsKey = true
            },
            new ColumnMap
            {
                ColumnName = "Name",
                ColumnType = typeof(string),
                Getter = o => ((TestEntity)o).Name
            }
            };
    }
}
