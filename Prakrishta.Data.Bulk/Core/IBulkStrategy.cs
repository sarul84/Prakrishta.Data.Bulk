namespace Prakrishta.Data.Bulk.Core
{
    using Prakrishta.Data.Bulk.Enum;

    public interface IBulkStrategy
    {
        BulkStrategyKind Kind { get; }

        Task<int> ExecuteAsync(
            BulkContext context,
            string connectionString,
            CancellationToken cancellationToken);
    }
}
