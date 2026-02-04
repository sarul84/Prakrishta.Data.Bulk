using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Pipeline;

namespace Prakrishta.Data.Bulk.Tests.Pipeline
{
    [TestClass]
    public class BulkPipelineEngineTests
    {
        private sealed class FakeStep : IBulkPipelineStep
        {
            public string Name => "Fake";
            public bool Executed { get; private set; }

            public Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public async Task Should_Execute_All_Steps()
        {
            var step = new FakeStep();
            var pipeline = new BulkPipelineEngine(new[] { step });

            var ctx = new BulkContext
            {
                OperationKind = BulkOperationKind.Update,
                TableName = "dbo.Test",
                EntityType = typeof(int),
                Items = new[] { 1 }
            };

            await pipeline.ExecuteAsync(ctx, CancellationToken.None);

            step.Executed.Should().BeTrue();
        }
    }
}
