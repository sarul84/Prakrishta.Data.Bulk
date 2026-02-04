namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using System.Data;
    using System.Data.Common;

    public sealed class FakeSqlConnection : DbConnection
    {
        public List<string> ExecutedSql { get; } = new();
        public FakeSqlCommand? LastCommand { get; private set; }


        public override string ConnectionString { get; set; } = "Fake";
        public override string Database => "FakeDb";
        public override string DataSource => "FakeSource";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => ConnectionState.Open;

        public override void Open() { }
        public override Task OpenAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public override void Close() { }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            => new FakeSqlTransaction(this);

        protected override DbCommand CreateDbCommand()
        {
            var cmd = new FakeSqlCommand(this);
            LastCommand = cmd;
            return cmd;
        }


        public override void ChangeDatabase(string databaseName) { }
    }
}
