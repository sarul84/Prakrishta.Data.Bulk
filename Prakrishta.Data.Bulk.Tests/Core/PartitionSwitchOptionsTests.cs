namespace Prakrishta.Data.Bulk.Tests.Core
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;

    [TestClass]
    public class PartitionSwitchOptionsTests
    {
        [TestMethod]
        public void UseStagingTable_SetsValue()
        {
            var opts = new PartitionSwitchOptions()
                .UseStagingTable("dbo.Stage_7");

            Assert.AreEqual("dbo.Stage_7", opts.StagingTable);
        }

        [TestMethod]
        public void ForPartition_SetsValue()
        {
            var opts = new PartitionSwitchOptions()
                .ForPartition(7);

            Assert.AreEqual(7, opts.PartitionNumber);
        }

        [TestMethod]
        public void FluentConfiguration_Works()
        {
            var opts = new PartitionSwitchOptions()
                .UseStagingTable("dbo.Stage_7")
                .ForPartition(7);

            Assert.AreEqual("dbo.Stage_7", opts.StagingTable);
            Assert.AreEqual(7, opts.PartitionNumber);
        }
    }
}
