namespace Prakrishta.Data.Bulk
{
    using Prakrishta.Data.Bulk.Abstractions;
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;
    using Prakrishta.Data.Bulk.Pipeline;

    public sealed class BulkEngine: IBulkEngineInternal
    {
        private readonly IBulkPipeline _pipeline;

        public string ConnectionString { get; }
        public IDbConnectionFactory ConnectionFactory { get; }

        public BulkEngine(
            string connectionString, 
            IDbConnectionFactory connectionFactory,
            IBulkPipeline pipeline)
        {
            ConnectionString = connectionString;
            ConnectionFactory = connectionFactory;
            _pipeline = pipeline;
        }

        public Task<int> UpdateAsync<T>(
            IEnumerable<T> items,
            string tableName,
            string? tableTypeName = null,
            string? procedureName = null,
            CancellationToken cancellationToken = default)
        {
            var context = new BulkContext
            {
                OperationKind = BulkOperationKind.Update,
                TableName = tableName,
                TableTypeName = tableTypeName,
                ProcedureName = procedureName,
                EntityType = typeof(T),
                Items = items.Cast<object>().ToList()
            };

            return _pipeline.ExecuteAsync(context, cancellationToken);
        }

        public Task<int> DeleteAsync<T>(
            IEnumerable<T> items,
            string tableName,
            string? tableTypeName = null,
            string? procedureName = null,
            CancellationToken cancellationToken = default)
        {
            var context = new BulkContext
            {
                OperationKind = BulkOperationKind.Delete,
                TableName = tableName,
                TableTypeName = tableTypeName,
                ProcedureName = procedureName,
                EntityType = typeof(T),
                Items = items.Cast<object>().ToList()
            };

            return _pipeline.ExecuteAsync(context, cancellationToken);
        }

        public Task<int> InsertAsync<T>(
            IEnumerable<T> items,
            string tableName,
            string? tableTypeName = null,
            string? procedureName = null,
            CancellationToken cancellationToken = default)
        {
            var context = new BulkContext
            {
                OperationKind = BulkOperationKind.Insert,
                TableName = tableName,
                TableTypeName = tableTypeName,
                ProcedureName = procedureName,
                EntityType = typeof(T),
                Items = items.Cast<object>().ToList()
            };

            return _pipeline.ExecuteAsync(context, cancellationToken);
        }

        internal Task<int> ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
        {
            return _pipeline.ExecuteAsync(context, cancellationToken);
        }
    }
}