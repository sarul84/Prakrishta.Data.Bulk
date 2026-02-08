namespace Prakrishta.Data.Bulk.Tests.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Extensions;
    using Prakrishta.Data.Bulk.Mapping;
    using Prakrishta.Data.Bulk.Pipeline;

    [TestClass]
    public class BulkEnginePartitionExtensionsTests
    {
        [TestMethod]
        public async Task ReplacePartitionAsync_BuildsContextCorrectly()
        {
            BulkContext? captured = null;

            var pipeline = new FakePipeline(ctx =>
            {
                captured = ctx;
                return Task.FromResult(ctx.ItemCount);
            });

            var engine = new BulkEngine(pipeline);

            var items = new[]
            {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
        };

            var result = await engine.ReplacePartitionAsync(
                items,
                "dbo.FactSales",
                opts => opts
                    .UseStagingTable("dbo.FactSales_Staging_7")
                    .ForPartition(7));

            Assert.AreEqual(2, result);
            Assert.IsNotNull(captured);

            Assert.AreEqual(BulkOperationKind.ReplacePartition, captured!.OperationKind);
            Assert.AreEqual(BulkStrategyKind.PartitionSwitch, captured.StrategyKind);
            Assert.AreEqual("dbo.FactSales", captured.TableName);

            Assert.AreEqual("dbo.FactSales_Staging_7", captured.Properties["StagingTable"]);
            Assert.AreEqual(7, captured.Properties["PartitionNumber"]);
            Assert.IsTrue(captured.Properties["ColumnMaps"] is ColumnMap[]);
        }

        private sealed class FakePipeline : IBulkPipeline
        {
            private readonly Func<BulkContext, Task<int>> _handler;

            public FakePipeline(Func<BulkContext, Task<int>> handler)
            {
                _handler = handler;
            }

            public Task<int> ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
                => _handler(context);
        }

        private sealed class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
