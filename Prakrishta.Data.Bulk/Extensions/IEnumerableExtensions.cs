using Prakrishta.Data.Bulk.Mapping;
using System.Data;

namespace Prakrishta.Data.Bulk.Extensions
{
    public static class IEnumerableExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            var table = new DataTable();
            var columns = ColumnMapCacheOld.Get<T>();

            foreach (var col in columns)
            {
                var type = Nullable.GetUnderlyingType(col.Type) ?? col.Type;
                table.Columns.Add(col.Name, type);
            }

            foreach (var item in items)
            {
                var row = table.NewRow();

                for (int i = 0; i < columns.Length; i++)
                {
                    var value = columns[i].Getter(item!);
                    row[i] = value ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable ToDataTableSpan<T>(this IEnumerable<T> items)
        {
            var table = new DataTable();
            var columns = ColumnMapCacheOld.Get<T>();

            foreach (var col in columns)
            {
                var type = Nullable.GetUnderlyingType(col.Type) ?? col.Type;
                table.Columns.Add(col.Name, type);
            }

            var buffer = new object?[columns.Length];

            foreach (var item in items)
            {
                var span = buffer.AsSpan();

                for (int i = 0; i < columns.Length; i++)
                    span[i] = columns[i].Getter(item!) ?? DBNull.Value;

                table.Rows.Add(buffer.Clone()!); // clone to avoid reuse overwrite
            }

            return table;
        }

        public static DataTable ToDataTable(this IEnumerable<object> items, ColumnMap[] maps)
        {
            var table = new DataTable();

            foreach (var map in maps)
            {
                table.Columns.Add(map.ColumnName, map.ColumnType);
            }

            foreach (var item in items)
            {
                var row = table.NewRow();
                foreach (var map in maps)
                {
                    row[map.ColumnName] = map.Getter(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
