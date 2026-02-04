using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Pipeline.StrategySelector;

namespace Prakrishta.Data.Bulk.Tests.Core
{
    [TestClass]
    public class BulkStrategySelectorTests
    {
        [TestMethod]
        public void Should_Select_Staging_When_Above_Threshold()
        {
            var options = new BulkOptions { StagingThreshold = 100 };
            var selector = new BulkStrategySelector(options);

            var ctx = new BulkContext
            {
                ItemCount = 200
            };

            selector.Select(ctx).Should().Be(BulkStrategyKind.StagingTable);
        }

        [TestMethod]
        public void Should_Select_Tvp_When_Below_Threshold()
        {
            var options = new BulkOptions { StagingThreshold = 100 };
            var selector = new BulkStrategySelector(options);

            var ctx = new BulkContext
            {
                ItemCount = 50
            };

            selector.Select(ctx).Should().Be(BulkStrategyKind.StoredProcedureTvp);
        }
    }

}
