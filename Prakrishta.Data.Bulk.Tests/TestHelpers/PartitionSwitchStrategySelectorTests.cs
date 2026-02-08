namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Pipeline.StrategySelector;

    [TestClass]
    public class PartitionSwitchStrategySelectorTests
    {
        private readonly BulkStrategySelector _selector = new(new BulkOptions());

        [TestMethod]
        public void Select_ShouldReturnPartitionSwitch_WhenExplicitlySet()
        {
            var ctx = new BulkContext
            {
                StrategyKind = BulkStrategyKind.PartitionSwitch
            };

            var result = _selector.Select(ctx);

            Assert.AreEqual(BulkStrategyKind.PartitionSwitch, result);
        }

        [TestMethod]
        public void Select_ShouldReturnPartitionSwitch_WhenOperationIsReplacePartition()
        {
            var ctx = new BulkContext
            {
                OperationKind = BulkOperationKind.ReplacePartition
            };

            var result = _selector.Select(ctx);

            Assert.AreEqual(BulkStrategyKind.PartitionSwitch, result);
        }

        [TestMethod]
        public void Select_ShouldReturnExistingStrategy_WhenNotPartitionSwitch()
        {
            var ctx = new BulkContext
            {
                StrategyKind = BulkStrategyKind.StoredProcedureTvp
            };

            var result = _selector.Select(ctx);

            Assert.AreEqual(BulkStrategyKind.StoredProcedureTvp, result);
        }
    }
}
