namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;

    public static class BulkContextTestFactory
    {
        public static BulkContext Create(
            BulkOperationKind kind,
            IEnumerable<TestEntity>? items = null)
        {
            var list = items != null
                ? items.Cast<object>().ToList()
                : DefaultItems().Cast<object>().ToList();

            var ctx = new BulkContext
            {
                OperationKind = kind,
                TableName = "dbo.Test",
                EntityType = typeof(TestEntity),
                Items = list
            };

            ctx.Properties["List"] = list;
            ctx.Properties["ColumnMaps"] = ColumnMapTestFactory.Create();

            return ctx;
        }

        private static IEnumerable<TestEntity> DefaultItems()
            => new[]
            {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
            };
    }
}
