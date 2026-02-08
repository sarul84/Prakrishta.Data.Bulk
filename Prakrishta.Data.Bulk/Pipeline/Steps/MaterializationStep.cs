namespace Prakrishta.Data.Bulk.Pipeline.Steps
{
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Mapping;

    public sealed class MaterializationStep : IBulkPipelineStep
    {
        public string Name => "Materialization";

        public Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
        {
            var enumerable = (IEnumerable<object>)context.Items;
            var list = enumerable.ToList();

            context.Properties["List"] = list;

            var maps = ColumnMapCache.Get(context.EntityType);

            context.Properties["ColumnMaps"] = maps;

            context.ItemCount = list.Count;

            return Task.CompletedTask;
        }
    }
}
