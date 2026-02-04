namespace Prakrishta.Data.Bulk.Tests.Strategies
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Prakrishta.Data.Bulk.Internals;
    using Prakrishta.Data.Bulk.Tests.TestHelpers;
    using System.Data;

    [TestClass]
    public class SqlBuilderExtensionsTests
    {
        [TestMethod]
        public void Should_Capture_Structured_Parameter_Metadata()
        {
            // Arrange
            var conn = new FakeSqlConnection();
            var cmd = (FakeSqlCommand)conn.CreateCommand();

            var table = new DataTable(); // TVP payload
            var typeName = "dbo.TestType";

            // Act
            var param = cmd.AddStructured("@Items", table, typeName);
            
            // Assert
            param.Should().BeOfType<FakeSqlParameter>();

            var fake = (FakeSqlParameter)param;

            fake.ParameterName.Should().Be("@Items");
            fake.Value.Should().Be(table);

            // ⭐ The important assertions
            fake.CapturedSqlDbType.Should().Be(SqlDbType.Structured);
            fake.CapturedTypeName.Should().Be(typeName);
        }
    }
}
