namespace Prakrishta.Data.Bulk.Tests.Strategies
{
    using FluentAssertions;
    using global::Prakrishta.Data.Bulk.Core;
    using global::Prakrishta.Data.Bulk.Enum;
    using global::Prakrishta.Data.Bulk.Strategies;
    using global::Prakrishta.Data.Bulk.Tests.TestHelpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TruncateAndReloadStrategyTests
    {
        [TestMethod]
        public async Task Should_Truncate_And_Reload()
        {
            var options = new BulkOptions();
            var fakeBulk = new FakeBulkCopyFactory();
            var fakeConn = new FakeDbConnectionFactory();

            var strategy = new TruncateAndReloadStrategy(options, fakeBulk, fakeConn);

            var ctx = BulkContextTestFactory.Create(BulkOperationKind.Insert);

            var result = await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

            result.Should().Be(2);

            fakeBulk.Instance.WasCalled.Should().BeTrue();
            fakeBulk.Instance.DestinationTableName.Should().Be("dbo.Test");

            ctx.Properties["LastExecutedSql"]
                .Should().Be("TRUNCATE TABLE dbo.Test;");
        }

        [TestMethod]
        public async Task Should_Return_Zero_When_List_Is_Empty()
        {
            var options = new BulkOptions();
            var fakeBulk = new FakeBulkCopyFactory();
            var fakeConn = new FakeDbConnectionFactory();

            var strategy = new TruncateAndReloadStrategy(options, fakeBulk, fakeConn);

            var ctx = BulkContextTestFactory.Create(BulkOperationKind.Insert, []);

            var result = await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

            result.Should().Be(0);
            fakeBulk.Instance.WasCalled.Should().BeFalse();
        }

        [TestMethod]
        public async Task Should_Map_Columns_Correctly()
        {
            var options = new BulkOptions();
            var fakeBulk = new FakeBulkCopyFactory();
            var fakeConn = new FakeDbConnectionFactory();

            var strategy = new TruncateAndReloadStrategy(options, fakeBulk, fakeConn);

            var ctx = BulkContextTestFactory.Create(BulkOperationKind.Insert);

            await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

            fakeBulk.Instance.ColumnMappings.Should().ContainEquivalentOf(("Id", "Id"));
            fakeBulk.Instance.ColumnMappings.Should().ContainEquivalentOf(("Name", "Name"));
        }
    }
}
