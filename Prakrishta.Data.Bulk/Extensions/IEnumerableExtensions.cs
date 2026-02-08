namespace Prakrishta.Data.Bulk.Extensions
{
    using Prakrishta.Data.Bulk.Internals;
    using Prakrishta.Data.Bulk.Mapping;
    using System.Data;

    public static class IEnumerableExtensions
    {
        public static DataTable ToDataTable(this IEnumerable<object> items, ColumnMap[] maps)
        {
            var table = new DataTable();

            foreach (var map in maps)
            {
                var type = map.ColumnType;
                bool isNullable = type.IsNullableValueType();

                if (isNullable)
                {
                    type = type.GetUnderlyingType()!;
                }

                var column = new DataColumn(map.ColumnName, type);

                column.AllowDBNull = isNullable || !type.IsValueType;

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
