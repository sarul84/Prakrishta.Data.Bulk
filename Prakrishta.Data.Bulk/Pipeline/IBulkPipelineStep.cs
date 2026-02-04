using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Pipeline
{
    public interface IBulkPipelineStep
    {
        string Name { get; }
        Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken);
    }

}
