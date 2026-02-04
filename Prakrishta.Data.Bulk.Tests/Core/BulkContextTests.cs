using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Tests.Core
{
    [TestClass]
    public class BulkContextTests
    {
        [TestMethod]
        public void Should_Store_And_Retrieve_Properties()
        {
            var ctx = new BulkContext
            {
                OperationKind = BulkOperationKind.Update,
                TableName = "dbo.Test",
                EntityType = typeof(string),
                Items = new[] { "A", "B" }
            };

            ctx.Properties["X"] = 123;

            ctx.Properties["X"].Should().Be(123);
        }
    }
}
