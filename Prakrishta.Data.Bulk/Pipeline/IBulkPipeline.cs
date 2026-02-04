using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Pipeline
{
    public interface IBulkPipeline
    {
        Task<int> ExecuteAsync(BulkContext context, CancellationToken cancellationToken);
    }
}
