using Prakrishta.Data.Bulk;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Diagnostics;
using Prakrishta.Data.Bulk.Factories;
using Prakrishta.Data.Bulk.Pipeline;
using Prakrishta.Data.Bulk.Pipeline.Steps;
using Prakrishta.Data.Bulk.Pipeline.StrategySelector;
using Prakrishta.Data.Bulk.Strategies;

public static class BulkEngineFactory
{
    public static BulkEngine Create(string connectionString, int stagingThreshold)
    {
        var options = new BulkOptions
        {
            StagingThreshold = stagingThreshold
        };

        var selector = new BulkStrategySelector(options);

        var connectionFactory = new SqlConnectionFactory();
        var sqlBulkCopyFactory = new SqlBulkCopyFactory();

        var strategies = new IBulkStrategy[]
        {
            new StoredProcedureTvpStrategy(connectionFactory),
            new StagingTableStrategy(options, sqlBulkCopyFactory, connectionFactory),
            new TruncateAndReloadStrategy(options, sqlBulkCopyFactory, connectionFactory)
        };

        var steps = new IBulkPipelineStep[]
        {
            new MaterializationStep(),
            new StrategySelectionStep(selector, NullBulkDiagnosticsSink.Instance),
            new StrategyExecutionStep(strategies, connectionString)
        };

        var pipeline = new BulkPipelineEngine(steps);

        return new BulkEngine(connectionString, connectionFactory, pipeline);
    }

    public static BulkEngine CreateTruncateOnly(string connectionString)
    {
        var options = new BulkOptions();
        var connectionFactory = new SqlConnectionFactory();
        var bulkCopyFactory = new SqlBulkCopyFactory();

        var truncate = new TruncateAndReloadStrategy(
            options,
            bulkCopyFactory,
            connectionFactory
        );

        var steps = new IBulkPipelineStep[]
        {
        new MaterializationStep(),

        new DirectExecutionStep(truncate, connectionString)
        };

        var pipeline = new BulkPipelineEngine(steps);

        return new BulkEngine(connectionString, connectionFactory, pipeline);
    }

}