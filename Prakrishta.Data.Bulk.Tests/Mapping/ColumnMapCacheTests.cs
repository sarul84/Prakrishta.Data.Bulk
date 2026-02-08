using FluentAssertions;
using Prakrishta.Data.Bulk.Mapping;

namespace Prakrishta.Data.Bulk.Tests.Mapping;

[TestClass]
public class ColumnMapCacheTests
{
    private sealed class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void Should_Return_ColumnMaps()
    {
        var maps = ColumnMapCache.Get(typeof(TestEntity));

        maps.Should().NotBeNull();
        maps.Should().HaveCount(2);
    }
}