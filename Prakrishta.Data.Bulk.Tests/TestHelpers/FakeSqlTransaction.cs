namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using System.Data;
    using System.Data.Common;

    public sealed class FakeSqlTransaction : DbTransaction
    {
        private readonly FakeSqlConnection _conn;

        public FakeSqlTransaction(FakeSqlConnection conn)
        {
            _conn = conn;
        }

        public override IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;

        protected override DbConnection DbConnection => _conn;

        public override void Commit() { }
        public override void Rollback() { }
    }
}
