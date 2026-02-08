
namespace Prakrishta.Data.Bulk.Tests.Strategies
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Engine.Strategies;
    using Prakrishta.Data.Bulk.Mapping;
    using Prakrishta.Data.Bulk.Tests.TestHelpers;

    [TestClass]
    public class PartitionSwitchStrategyTests
    {
        private FakeBulkCopyFactory _bulkFactory = null!;
        private List<string> _executedSql = null!;
        private PartitionSwitchStrategy _strategy = null!;

        [TestInitialize]
        public void Setup()
        {
            _bulkFactory = new FakeBulkCopyFactory();
            _executedSql = new List<string>();

            Task ExecSql(string conn, string sql, CancellationToken ct)
            {
                _executedSql.Add(sql);
                return Task.CompletedTask;
            }

            void ValidateAlignment(string ts, string ss, string tf, string sf)
            {
                // happy path
            }

            _strategy = new PartitionSwitchStrategy(_bulkFactory, new FakeDbConnectionFactory());
        }

        //[TestMethod]
        //public async Task ExecuteAsync_CreatesStaging_BulkLoads_AndSwitches()
        //{
        //    var items = new List<object> { new TestEntity { Id = 1, Name = "A" } };
        //    var maps = ColumnMapCache.Get(typeof(TestEntity));

        //    var ctx = new BulkContext
        //    {
        //        TableName = "dbo.FactSales",
        //        EntityType = typeof(TestEntity),
        //        Items = items,
        //        ItemCount = items.Count
        //    };

        //    ctx.Properties["StagingTable"] = "dbo.FactSales_Staging_7";
        //    ctx.Properties["PartitionNumber"] = 7;
        //    ctx.Properties["ColumnMaps"] = maps;

        //    var result = await _strategy.ExecuteAsync(ctx, "FakeConn", default);

        //    Assert.AreEqual(items.Count, result);

        //    Assert.IsTrue(_executedSql.Any(sql => sql.Contains("SELECT TOP 0 * INTO")));
        //    Assert.IsTrue(_executedSql.Any(sql => sql.Contains("SWITCH PARTITION 7")));

        //    Assert.IsTrue(_bulkFactory.Instance.WasCalled);
        //    Assert.AreEqual("dbo.FactSales_Staging_7", _bulkFactory.Instance.DestinationTableName);
        //}

        [TestMethod]
        public async Task ExecuteAsync_Throws_WhenMissingStagingTable()
        {
            var ctx = new BulkContext
            {
                TableName = "dbo.FactSales",
                EntityType = typeof(TestEntity),
                Items = new List<object>(),
                ItemCount = 0
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _strategy.ExecuteAsync(ctx, "FakeConn", default));
        }

        private sealed class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}