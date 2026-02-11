using Prakrishta.Data.Bulk.Abstractions;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public sealed class FakeSchemaResolver : ISchemaResolver
    {
        private readonly string _schemaToReturn;

        public FakeSchemaResolver(string schema)
        {
            _schemaToReturn = schema;
        }

        public Task<string> ResolveSchemaAsync(string tableName, CancellationToken ct)
            => Task.FromResult(_schemaToReturn);
    }
}
