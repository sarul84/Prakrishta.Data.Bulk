namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using System.Data;
    using System.Data.Common;

    public sealed class FakeSqlCommand : DbCommand
    {
        private readonly FakeSqlConnection _conn;

        public FakeSqlCommand(FakeSqlConnection conn)
        {
            _conn = conn;
        }

        public override string CommandText { get; set; } = string.Empty;
        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; } = CommandType.Text;

        protected override DbConnection DbConnection
        {
            get => _conn;
            set { }
        }

        protected override DbTransaction DbTransaction { get; set; } = null!;
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbParameterCollection DbParameterCollection { get; }
            = new FakeSqlParameterCollection();

        public override void Cancel() { }

        public override int ExecuteNonQuery()
        {
            _conn.ExecutedSql.Add(CommandText);
            return 42; // deterministic fake result
        }

        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            _conn.ExecutedSql.Add(CommandText);
            return Task.FromResult(42);
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => new FakeSqlDataReader();

        public override object ExecuteScalar()
        {
            _conn.ExecutedSql.Add(CommandText);
            return 42;
        }

        public override void Prepare() { }

        protected override DbParameter CreateDbParameter()
            => new FakeSqlParameter();

    }
}
