using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Tests.Core
{
    [TestClass]
    public class BulkOptionsTests
    {
        [TestMethod]
        public void Should_Default_To_StagingTable()
        {
            var options = new BulkOptions();
            options.PreferredStrategy.Should().Be(BulkStrategyKind.StagingTable);
        }
    }

}
