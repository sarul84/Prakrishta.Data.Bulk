using Prakrishta.Data.Bulk.Mapping;
using System.Data;

namespace Prakrishta.Data.Bulk.Extensions
{
    public static class DataTableExtensions
    {
        public static List<T> ToPocoList<T>(this DataTable table) where T : new()
        {
            var maps = ColumnMapCacheOld.Get<T>();
            var list = new List<T>(table.Rows.Count);

            foreach (DataRow row in table.Rows)
            {
                var obj = new T();
                foreach (var map in maps)
                {
                    var value = row[map.Name];
                    if (value is DBNull) value = null;

                    map.Setter(obj, value);
                }
                list.Add(obj);
            }

            return list;
        }
    }
}
