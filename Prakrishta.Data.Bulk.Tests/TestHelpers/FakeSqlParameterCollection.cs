namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    using System.Collections;
    using System.Data;
    using System.Data.Common;

    public sealed class FakeSqlParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _list = new();

        public override int Count => _list.Count;
        public override object SyncRoot => new();

        public override int Add(object value)
        {
            var p = (DbParameter)value;

            if (p is FakeSqlParameter fake)
            {
                if (fake.Value is DataTable)
                {
                    fake.CapturedSqlDbType = SqlDbType.Structured;
                    fake.CapturedTypeName = fake.SourceColumn;
                }
            }

            _list.Add((DbParameter)value);
            return _list.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var v in values)
                _list.Add((DbParameter)v);
        }

        public override void Clear() => _list.Clear();

        public override bool Contains(object value) => _list.Contains((DbParameter)value);

        public override bool Contains(string value)
            => _list.Any(p => p.ParameterName == value);

        public override void CopyTo(Array array, int index)
            => _list.ToArray().CopyTo(array, index);

        public override IEnumerator GetEnumerator()
            => ((IEnumerable)_list).GetEnumerator(); // non-generic enumerator

        public override int IndexOf(object value)
            => _list.IndexOf((DbParameter)value);

        public override int IndexOf(string parameterName)
            => _list.FindIndex(p => p.ParameterName == parameterName);

        public override void Insert(int index, object value)
            => _list.Insert(index, (DbParameter)value);

        public override void Remove(object value)
            => _list.Remove((DbParameter)value);

        public override void RemoveAt(int index)
            => _list.RemoveAt(index);

        public override void RemoveAt(string parameterName)
        {
            var idx = IndexOf(parameterName);
            if (idx >= 0)
                _list.RemoveAt(idx);
        }

        protected override DbParameter GetParameter(int index)
            => _list[index];

        protected override DbParameter GetParameter(string parameterName)
            => _list.First(p => p.ParameterName == parameterName);

        protected override void SetParameter(int index, DbParameter value)
            => _list[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var idx = IndexOf(parameterName);
            if (idx >= 0)
                _list[idx] = value;
        }
    }
}
