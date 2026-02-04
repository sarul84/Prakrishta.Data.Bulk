namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using System.Data;
    using System.Data.Common;

    public sealed class FakeSqlParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; } = string.Empty;
        public override string SourceColumn { get; set; } = string.Empty;
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
        public SqlDbType? CapturedSqlDbType { get; set; }
        public string? CapturedTypeName { get; set; }

        public override void ResetDbType() { }
    }
}
