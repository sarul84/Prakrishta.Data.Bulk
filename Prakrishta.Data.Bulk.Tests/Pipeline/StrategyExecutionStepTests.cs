using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Pipeline.Steps;

namespace Prakrishta.Data.Bulk.Tests.Pipeline
{
    [TestClass]
    public class StrategyExecutionStepTests
    {
        private sealed class FakeStrategy : IBulkStrategy
        {
            public BulkStrategyKind Kind => BulkStrategyKind.StoredProcedureTvp;
            public Task<int> ExecuteAsync(BulkContext context, string cs, CancellationToken ct)
                => Task.FromResult(42);
        }

        [TestMethod]
        public async Task Should_Execute_Strategy_And_Set_AffectedRows()
        {
            var strategy = new FakeStrategy();
            var step = new StrategyExecutionStep(new[] { strategy }, "FakeCS");

            var ctx = new BulkContext
            {
                StrategyKind = BulkStrategyKind.StoredProcedureTvp
            };

            await step.ExecuteAsync(ctx, CancellationToken.None);

            ctx.Properties["AffectedRows"].Should().Be(42);
        }
    }
}
