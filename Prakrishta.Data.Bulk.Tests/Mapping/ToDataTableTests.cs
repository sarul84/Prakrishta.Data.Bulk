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

    private sealed class NullableEntity
    {
        public int Id { get; set; }
        public int? OptionalInt { get; set; }
        public decimal? OptionalDecimal { get; set; }
        public DateTime? OptionalDate { get; set; }
        public string? OptionalString { get; set; }
    }

    private static ColumnMap[] BuildMaps()
    {
        return typeof(NullableEntity)
            .GetProperties()
            .Select(p => new ColumnMap
            {
                ColumnName = p.Name,
                ColumnType = p.PropertyType,
                Getter = obj => p.GetValue(obj),
                IsKey = p.Name == "Id"
            })
            .ToArray();
    }

    [TestMethod]
    public void ToDataTable_Should_Unwrap_Nullable_Types_And_Set_AllowDBNull()
    {
        // Arrange
        var items = new List<NullableEntity>
        {
            new NullableEntity
            {
                Id = 1,
                OptionalInt = null,
                OptionalDecimal = 12.5m,
                OptionalDate = null,
                OptionalString = null
            }
        };

        var maps = BuildMaps();

        // Act
        var table = items.ToDataTable(maps);

        // Assert schema
        Assert.AreEqual(typeof(int), table.Columns["Id"]?.DataType);
        Assert.AreEqual(typeof(int), table.Columns["OptionalInt"]?.DataType);
        Assert.AreEqual(typeof(decimal), table.Columns["OptionalDecimal"]?.DataType);
        Assert.AreEqual(typeof(DateTime), table.Columns["OptionalDate"]?.DataType);
        Assert.AreEqual(typeof(string), table.Columns["OptionalString"]?.DataType);

        Assert.IsFalse(table.Columns["Id"]?.AllowDBNull);
        Assert.IsTrue(table.Columns["OptionalInt"]?.AllowDBNull);
        Assert.IsTrue(table.Columns["OptionalDecimal"].AllowDBNull);
        Assert.IsTrue(table.Columns["OptionalDate"]?.AllowDBNull);
        Assert.IsTrue(table.Columns["OptionalString"].AllowDBNull);

        // Assert row values
        var row = table.Rows[0];

        Assert.AreEqual(1, row["Id"]);
        Assert.AreEqual(DBNull.Value, row["OptionalInt"]);
        Assert.AreEqual(12.5m, row["OptionalDecimal"]);
        Assert.AreEqual(DBNull.Value, row["OptionalDate"]);
        Assert.AreEqual(DBNull.Value, row["OptionalString"]);
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