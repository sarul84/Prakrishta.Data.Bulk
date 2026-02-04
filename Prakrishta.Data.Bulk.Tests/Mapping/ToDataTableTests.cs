namespace Prakrishta.Data.Bulk.Tests.Mapping;

using FluentAssertions;
using Prakrishta.Data.Bulk.Extensions;
using Prakrishta.Data.Bulk.Mapping;

[TestClass]
public class DataTableExtensionsTests
{
    private sealed class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void Should_Create_DataTable()
    {
        var maps = new[]
        {
            new ColumnMap { ColumnName = "Id", ColumnType = typeof(int), Getter = o => ((TestEntity)o).Id },
            new ColumnMap { ColumnName = "Name", ColumnType = typeof(string), Getter = o => ((TestEntity)o).Name }
        };

        var items = new[]
        {
            new TestEntity { Id = 1, Name = "A" }
        };

        var table = items.Cast<object>().ToDataTable(maps);

        table.Rows.Count.Should().Be(1);
        table.Columns.Count.Should().Be(2);
    }
}