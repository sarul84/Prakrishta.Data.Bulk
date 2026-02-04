using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Strategies;
using Prakrishta.Data.Bulk.Tests.TestHelpers;

namespace Prakrishta.Data.Bulk.Tests.Strategies
{
    [TestClass]
    public class StoredProcedureTvpStrategyTests
    {
        [TestMethod]
        public async Task Insert_Should_Throw_When_TableTypeName_Missing()
        {
            var strategy = new StoredProcedureTvpStrategy(new FakeDbConnectionFactory());

            var ctx = new BulkContext
            {
                OperationKind = BulkOperationKind.Insert,
                TableName = "dbo.Test",
                Items = new List<object>()
            };

            await strategy.Invoking(s => s.ExecuteAsync(ctx, "Fake", CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
