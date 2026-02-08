namespace Prakrishta.Data.Bulk.Extensions
{
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Mapping;

    public static class BulkEnginePartitionExtensions
    {
        public static Task<int> ReplacePartitionAsync<T>(
            this BulkEngine engine,
            IEnumerable<T> items,
            string tableName,
            Action<PartitionSwitchOptions> configure,
            CancellationToken cancellationToken = default)
        {
            if (engine is null)
                throw new ArgumentNullException(nameof(engine));

            if (items is null)
                throw new ArgumentNullException(nameof(items));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name is required.", nameof(tableName));

            var options = new PartitionSwitchOptions();
            configure(options);

            var list = items.Cast<object>().ToList();

            var context = new BulkContext
            {
                OperationKind = BulkOperationKind.ReplacePartition,
                StrategyKind = BulkStrategyKind.PartitionSwitch,
                TableName = tableName,
                EntityType = typeof(T),
                Items = list,
                ItemCount = list.Count
            };

            context.Properties["ColumnMaps"] = ColumnMapCache.Get(typeof(T));

            options.ApplyTo(context);

            return engine.ExecuteAsync(context, cancellationToken);
        }
    }
}
