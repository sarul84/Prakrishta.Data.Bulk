namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public sealed class FakeSqlExecutor
    {
        public List<string> ExecutedSql { get; } = new();

        public Task ExecuteAsync(string sql)
        {
            ExecutedSql.Add(sql);
            return Task.CompletedTask;
        }
    }
}
