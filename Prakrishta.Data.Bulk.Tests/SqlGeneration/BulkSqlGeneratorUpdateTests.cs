using FluentAssertions;
using Prakrishta.Data.Bulk.Mapping;
using Prakrishta.Data.Bulk.SqlGeneration;

namespace Prakrishta.Data.Bulk.Tests.SqlGeneration
{
    [TestClass]
    public class BulkSqlGeneratorUpdateTests
    {
        [TestMethod]
        public void Should_Generate_Update_Sql()
        {
            var maps = new[]
            {
            new ColumnMap { ColumnName = "Id", IsKey = true },
            new ColumnMap { ColumnName = "Name" }
        };

            var sql = BulkSqlGenerator.GenerateUpdateSql("dbo.T", "dbo.S", maps);

            sql.Should().Contain("UPDATE T");
            sql.Should().Contain("T.[Name] = S.[Name]");
            sql.Should().Contain("T.[Id] = S.[Id]");
        }
    }
}
