using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Pipeline;

namespace Prakrishta.Data.Bulk.Tests
{
    [TestClass]
    public class BulkEngineTests
    {
        private sealed class FakePipeline : IBulkPipeline
        {
            public Task<int> ExecuteAsync(BulkContext ctx, CancellationToken ct)
                => Task.FromResult(99);
        }

        [TestMethod]
        public async Task Should_Invoke_Pipeline()
        {
            var engine = new BulkEngine(new FakePipeline());

            var result = await engine.UpdateAsync(new[] { 1 }, "dbo.Test");

            result.Should().Be(99);
        }

        [TestMethod]
        public async Task Insert_Should_Invoke_Pipeline()
        {
            var pipeline = new FakePipeline();
            var engine = new BulkEngine(pipeline);

            var result = await engine.InsertAsync(new[] { 1 }, "dbo.Test");

            result.Should().Be(99);
        }

    }
}
