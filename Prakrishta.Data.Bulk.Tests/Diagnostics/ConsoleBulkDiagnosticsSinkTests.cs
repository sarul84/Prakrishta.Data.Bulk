using FluentAssertions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Diagnostics;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Tests.Diagnostics
{
    [TestClass]
    public class ConsoleBulkDiagnosticsSinkTests
    {
        [TestMethod]
        public void Should_Not_Throw_On_All_Methods()
        {
            var sink = new ConsoleBulkDiagnosticsSink();
            var ctx = new BulkContext
            {
                TableName = "dbo.Test",
                OperationKind = BulkOperationKind.Update
            };

            sink.Invoking(s => s.OnStrategySelected(ctx)).Should().NotThrow();
            sink.Invoking(s => s.OnStepStarted(ctx, "X")).Should().NotThrow();
            sink.Invoking(s => s.OnStepCompleted(ctx, "X", TimeSpan.Zero)).Should().NotThrow();
            sink.Invoking(s => s.OnError(ctx, new Exception("err"))).Should().NotThrow();
        }
    }
}
