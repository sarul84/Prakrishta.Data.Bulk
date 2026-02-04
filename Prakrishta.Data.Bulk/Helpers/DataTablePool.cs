using Microsoft.Extensions.ObjectPool;
using Prakrishta.Data.Bulk.Mapping;
using System.Collections.Concurrent;
using System.Data;

namespace Prakrishta.Data.Bulk.Helpers
{
    public sealed class DataTablePool
    {
        private readonly ConcurrentDictionary<Type, ObjectPool<DataTable>> _pools = new();

        public DataTable Rent<T>()
        {
            var type = typeof(T);
            var pool = _pools.GetOrAdd(type, _ => CreatePoolFor<T>());
            return pool.Get();
        }

        public void Return<T>(DataTable table)
        {
            table.Clear();
            _pools[typeof(T)].Return(table);
        }

        private static ObjectPool<DataTable> CreatePoolFor<T>()
        {
            var policy = new DataTablePooledObjectPolicy(typeof(T));
            return new DefaultObjectPool<DataTable>(policy);
        }
    }

    public sealed class DataTablePooledObjectPolicy : PooledObjectPolicy<DataTable>
    {
        private readonly Type _type;

        public DataTablePooledObjectPolicy(Type type) => _type = type;

        public override DataTable Create()
        {
            var table = new DataTable();
            var columns = ColumnMapCacheOld.Get(_type);

            foreach (var col in columns)
            {
                var type = Nullable.GetUnderlyingType(col.Type) ?? col.Type;
                table.Columns.Add(col.Name, type);
            }

            return table;
        }

        public override bool Return(DataTable obj)
        {
            obj.Clear();
            return true;
        }
    }
}
