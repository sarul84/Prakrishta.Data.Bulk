using FluentAssertions;
using Prakrishta.Data.Bulk.Mapping;
using Prakrishta.Data.Bulk.SqlGeneration;

namespace Prakrishta.Data.Bulk.Tests.SqlGeneration
{
    [TestClass]
    public class BulkSqlGeneratorDeleteTests
    {
        [TestMethod]
        public void Should_Generate_Delete_Sql()
        {
            var maps = new[]
            {
            new ColumnMap { ColumnName = "Id", IsKey = true }
        };

            var sql = BulkSqlGenerator.GenerateDeleteSql("dbo.T", "dbo.S", maps);

            sql.Should().Contain("DELETE T");
            sql.Should().Contain("T.[Id] = S.[Id]");
        }
    }
}
