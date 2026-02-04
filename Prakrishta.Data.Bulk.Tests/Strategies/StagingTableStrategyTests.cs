namespace Prakrishta.Data.Bulk.Tests.Strategies;

using FluentAssertions;
using global::Prakrishta.Data.Bulk.Mapping;
using global::Prakrishta.Data.Bulk.SqlGeneration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Strategies;
using Prakrishta.Data.Bulk.Tests.TestHelpers;

[TestClass]
public class StagingTableStrategyTests
{
    [TestMethod]
    public async Task Insert_Should_Use_BulkCopy_Directly()
    {
        var options = new BulkOptions();
        var fakeFactory = new FakeBulkCopyFactory();
        var strategy = new StagingTableStrategy(options, fakeFactory, new FakeDbConnectionFactory());

        var ctx = BulkContextTestFactory.Create(BulkOperationKind.Insert);

        var result = await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

        result.Should().Be(2);
        fakeFactory.Instance.WasCalled.Should().BeTrue();
        fakeFactory.Instance.DestinationTableName.Should().Be("dbo.Test");
    }

    [TestMethod]
    public async Task Update_Should_Use_StagingTable_And_Generate_Update_Sql()
    {
        var options = new BulkOptions();
        var fakeFactory = new FakeBulkCopyFactory();
        var strategy = new StagingTableStrategy(options, fakeFactory, new FakeDbConnectionFactory());

        var ctx = BulkContextTestFactory.Create(BulkOperationKind.Update);

        // Execute
        await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

        // Validate bulk copy was used
        fakeFactory.Instance.WasCalled.Should().BeTrue();

        // Validate SQL generation
        var maps = (ColumnMap[])ctx.Properties["ColumnMaps"]!;
        var expectedSql = BulkSqlGenerator.GenerateUpdateSql("dbo.Test", "dbo.Test_Staging", maps);

        // Strategy stores SQL in context for testing
        ctx.Properties["LastExecutedSql"].Should().Be(expectedSql);
    }

    [TestMethod]
    public async Task Delete_Should_Use_StagingTable_And_Generate_Delete_Sql()
    {
        var options = new BulkOptions();
        var fakeFactory = new FakeBulkCopyFactory();
        var strategy = new StagingTableStrategy(options, fakeFactory, new FakeDbConnectionFactory());

        var ctx = BulkContextTestFactory.Create(BulkOperationKind.Delete);

        await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

        fakeFactory.Instance.WasCalled.Should().BeTrue();

        var maps = (ColumnMap[])ctx.Properties["ColumnMaps"]!;
        var expectedSql = BulkSqlGenerator.GenerateDeleteSql("dbo.Test", "dbo.Test_Staging", maps);

        ctx.Properties["LastExecutedSql"].Should().Be(expectedSql);
    }

    [TestMethod]
    public async Task Should_Return_Zero_When_List_Is_Empty()
    {
        var options = new BulkOptions();
        var fakeFactory = new FakeBulkCopyFactory();
        var strategy = new StagingTableStrategy(options, fakeFactory, new FakeDbConnectionFactory());

        var ctx = BulkContextTestFactory.Create(BulkOperationKind.Update, Enumerable.Empty<TestEntity>());

        var result = await strategy.ExecuteAsync(ctx, "FakeCS", CancellationToken.None);

        result.Should().Be(0);
        fakeFactory.Instance.WasCalled.Should().BeFalse();
    }
}