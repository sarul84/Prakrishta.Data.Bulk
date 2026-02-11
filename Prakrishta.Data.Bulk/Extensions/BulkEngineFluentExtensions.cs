namespace Prakrishta.Data.Bulk.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Prakrishta.Data.Bulk.Abstractions;
    using Prakrishta.Data.Bulk.Builder;

    public static class BulkEngineFluentExtensions
    {
        public static BulkEntityBuilder<T> For<T>(this BulkEngine engine, IServiceProvider sp) where T : class
        {
            return new BulkEntityBuilder<T>(
                engine,
                (IBulkEngineInternal)engine,
                sp.GetRequiredService<IPartitionSwitchExecutor>(),
                sp.GetRequiredService<ISchemaResolver>());
        }
    }
}
