using Prakrishta.Data.Bulk.Mapping;
using System.Data;

namespace Prakrishta.Data.Bulk.Extensions
{
    public static class IEnumerableExtensions
    {
        public static DataTable ToDataTable(this IEnumerable<object> items, ColumnMap[] maps)
        {
            var table = new DataTable();

            foreach (var map in maps)
            {
                var type = map.ColumnType;

                bool isNullableValueType =
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(Nullable<>);

                if (isNullableValueType)
                {
                    type = Nullable.GetUnderlyingType(type)!;
                }

                var column = new DataColumn(map.ColumnName, type);

                column.AllowDBNull = isNullableValueType || !type.IsValueType;

                table.Columns.Add(column);
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
