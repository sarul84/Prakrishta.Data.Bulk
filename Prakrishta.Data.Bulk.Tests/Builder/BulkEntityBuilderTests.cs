using Prakrishta.Data.Bulk.Builder;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Tests.TestHelpers;

namespace Prakrishta.Data.Bulk.Tests.Builder;

[TestClass]
public class BulkEntityBuilderTests
{
    private FakeBulkEngine _fakeEngine = null!;
    private FakePartitionSwitchExecutor _fakeSwitch = null!;
    private FakeSchemaResolver _fakeResolver = null!;
    private BulkEntityBuilder<TestEntity> _builder = null!;
    private List<TestEntity> _items = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeEngine = new FakeBulkEngine();
        _fakeSwitch = new FakePartitionSwitchExecutor();
        _fakeResolver = new FakeSchemaResolver("dbo"); // default schema

        _builder = new BulkEntityBuilder<TestEntity>(
            engine: new BulkEngine("conn", null!, null!),
            internalEngine: _fakeEngine,
            switchExecutor: _fakeSwitch,
            schemaResolver: _fakeResolver);

        _items = new List<TestEntity>
        {
            new() { Id = 1, Name = "A" },
            new() { Id = 2, Name = "B" }
        };
    }

    // ---------------------------------------------------------
    // INSERT
    // ---------------------------------------------------------

    [TestMethod]
    public async Task Insert_UsesConventions()
    {
        await _builder.InsertAsync(_items);

        Assert.AreEqual("dbo.TestEntity", _fakeEngine.LastTableName);
        Assert.AreEqual("dbo.TestEntityType", _fakeEngine.LastTvpName);
        Assert.AreEqual("dbo.TestEntity_Insert", _fakeEngine.LastProcedureName);
    }

    [TestMethod]
    public async Task Insert_UsesOverrides()
    {
        await _builder
            .ToTable("dbo.Custom")
            .UsingTvp("dbo.CustomType")
            .UsingStoredProcedure("dbo.Custom_Insert")
            .InsertAsync(_items);

        Assert.AreEqual("dbo.Custom", _fakeEngine.LastTableName);
        Assert.AreEqual("dbo.CustomType", _fakeEngine.LastTvpName);
        Assert.AreEqual("dbo.Custom_Insert", _fakeEngine.LastProcedureName);
    }

    // ---------------------------------------------------------
    // UPDATE
    // ---------------------------------------------------------

    [TestMethod]
    public async Task Update_UsesConventions()
    {
        await _builder.UpdateAsync(_items);

        Assert.AreEqual("dbo.TestEntity", _fakeEngine.LastTableName);
        Assert.AreEqual("dbo.TestEntityType", _fakeEngine.LastTvpName);
        Assert.AreEqual("dbo.TestEntity_Update", _fakeEngine.LastProcedureName);
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    [TestMethod]
    public async Task Delete_UsesConventions()
    {
        await _builder.DeleteAsync(_items);

        Assert.AreEqual("dbo.TestEntity", _fakeEngine.LastTableName);
        Assert.AreEqual("dbo.TestEntityType", _fakeEngine.LastTvpName);
        Assert.AreEqual("dbo.TestEntity_Delete", _fakeEngine.LastProcedureName);
    }

    // ---------------------------------------------------------
    // PARTITION SWITCH
    // ---------------------------------------------------------

    [TestMethod]
    public async Task ReplacePartition_UsesConventions_AndCapturesOptions()
    {
        await _builder.ReplacePartitionAsync(_items, opts =>
        {
            opts.UseStagingTable("dbo.Stage_7").ForPartition(7);
        });

        Assert.AreEqual("dbo.TestEntity", _fakeSwitch.LastTableName);

        var options = new PartitionSwitchOptions();
        _fakeSwitch.LastOptions!(options);

        Assert.AreEqual("dbo.Stage_7", options.StagingTable);
        Assert.AreEqual(7, options.PartitionNumber);
    }

    [TestMethod]
    public async Task ReplacePartition_UsesTableOverride()
    {
        await _builder
            .ToTable("dbo.FactSales")
            .ReplacePartitionAsync(_items, opts =>
            {
                opts.UseStagingTable("dbo.Stage_7").ForPartition(7);
            });

        Assert.AreEqual("dbo.FactSales", _fakeSwitch.LastTableName);
    }

    // ---------------------------------------------------------
    // SCHEMA OVERRIDES
    // ---------------------------------------------------------

    [TestMethod]
    public async Task Insert_UsesConfiguredSchema()
    {
        await _builder
            .InSchema("sales")
            .InsertAsync(_items);

        Assert.AreEqual("sales.TestEntity", _fakeEngine.LastTableName);
        Assert.AreEqual("sales.TestEntityType", _fakeEngine.LastTvpName);
        Assert.AreEqual("sales.TestEntity_Insert", _fakeEngine.LastProcedureName);
    }

    // ---------------------------------------------------------
    // AUTOMATIC SCHEMA DISCOVERY
    // ---------------------------------------------------------

    [TestMethod]
    public async Task Insert_UsesDiscoveredSchema_WhenNotProvided()
    {
        var resolver = new FakeSchemaResolver("sales");

        var builder = new BulkEntityBuilder<TestEntity>(
            engine: new BulkEngine("conn", null!, null!),
            internalEngine: _fakeEngine,
            switchExecutor: _fakeSwitch,
            schemaResolver: resolver);

        await builder.InsertAsync(_items);

        Assert.AreEqual("sales.TestEntity", _fakeEngine.LastTableName);
    }
}

public sealed class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}