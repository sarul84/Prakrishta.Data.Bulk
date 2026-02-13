using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace Prakrishta.Data.Bulk.Extensions;

using Prakrishta.Data.Bulk.Abstractions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Engine.Strategies;
using Prakrishta.Data.Bulk.Enum;
using Prakrishta.Data.Bulk.Factories;
using Prakrishta.Data.Bulk.Helpers;
using Prakrishta.Data.Bulk.Internals;
using Prakrishta.Data.Bulk.Pipeline;
using Prakrishta.Data.Bulk.Pipeline.StrategySelector;
using Prakrishta.Data.Bulk.Strategies;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBulkEngine(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration,
        Action<BulkOptions>? configure = null)
    {
        var options = new BulkOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        // Factories
        services.AddSingleton<IBulkCopyFactory, SqlBulkCopyFactory>();
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        services.Configure<ConnectionPoolingOptions>(configuration.GetSection("BulkEngine:ConnectionPooling"));

        services.AddSingleton<IDbConnectionFactory>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<ConnectionPoolingOptions>>().Value;
            var baseFactory = new SqlConnectionFactory();
            return new PooledConnectionFactory(baseFactory, opts.MaxPoolSize);
        });

        // Register schema resolver
        services.AddSingleton<ISchemaResolver>(sp =>
        {
            var factory = sp.GetRequiredService<IDbConnectionFactory>();
            return new SchemaResolver(factory, connectionString);
        });

        // Strategy selector
        services.AddSingleton<BulkStrategySelector>();

        // Strategies
        services.AddSingleton<IBulkStrategy>(sp =>
            new StoredProcedureTvpStrategy(
                sp.GetRequiredService<IDbConnectionFactory>()));

        services.AddSingleton<IBulkStrategy>(sp =>
            new StagingTableStrategy(
                sp.GetRequiredService<BulkOptions>(),
                sp.GetRequiredService<IBulkCopyFactory>(),
                sp.GetRequiredService<IDbConnectionFactory>()));

        services.AddSingleton<IBulkStrategy>(sp =>
            new TruncateAndReloadStrategy(
                sp.GetRequiredService<BulkOptions>(),
                sp.GetRequiredService<IBulkCopyFactory>(),
                sp.GetRequiredService<IDbConnectionFactory>()));

        services.AddSingleton<IBulkStrategy>(sp =>
            new PartitionSwitchStrategy(
                sp.GetRequiredService<IBulkCopyFactory>(),
                sp.GetRequiredService<IDbConnectionFactory>()));

        // Strategy dictionary
        services.AddSingleton<IDictionary<BulkStrategyKind, IBulkStrategy>>(sp =>
        {
            var strategies = sp.GetServices<IBulkStrategy>();
            return strategies.ToDictionary(s => s.Kind, s => s);
        });

        // Pipeline
        services.AddSingleton<IBulkPipeline, BulkPipelineEngine>();

        // Register BulkEngine
        services.AddSingleton<BulkEngine>(sp =>
        {
            var factory = sp.GetRequiredService<IDbConnectionFactory>();
            var pipeline = sp.GetRequiredService<IBulkPipeline>();
            return new BulkEngine(connectionString, factory, pipeline);
        });


        return services;
    }

    public static IServiceCollection AddBulkStrategy<T>(this IServiceCollection services)
        where T : class, IBulkStrategy
    {
        services.AddSingleton<IBulkStrategy, T>();
        return services;
    }
}
