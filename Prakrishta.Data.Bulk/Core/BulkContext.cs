using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Core
{
    public sealed class BulkContext
    {
        public BulkOperationKind OperationKind { get; init; }
        public BulkStrategyKind StrategyKind { get; set; }

        public string TableName { get; init; } = default!;
        public string? TableTypeName { get; init; }
        public string? ProcedureName { get; init; }

        public int ItemCount { get; set; }
        public Type EntityType { get; init; } = default!;
        public object Items { get; init; } = default!;

        public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
    }
}
