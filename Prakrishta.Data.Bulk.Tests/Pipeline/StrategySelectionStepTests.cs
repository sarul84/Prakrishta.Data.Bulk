using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Diagnostics;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Pipeline.Steps;
using Prakrishta.Data.Bulk.Pipeline.StrategySelector;

namespace Prakrishta.Data.Bulk.Tests.Pipeline
{
    [TestClass]
    public class StrategySelectionStepTests
    {
        [TestMethod]
        public async Task Should_Set_Strategy_On_Context()
        {
            var options = new BulkOptions();
            var selector = new BulkStrategySelector(options);
            var diagnostics = NullBulkDiagnosticsSink.Instance;

            var step = new StrategySelectionStep(selector, diagnostics);

            var ctx = new BulkContext
            {
                ItemCount = 10
            };

            await step.ExecuteAsync(ctx, CancellationToken.None);

            ctx.StrategyKind.Should().Be(BulkStrategyKind.StoredProcedureTvp);
        }
    }
}
