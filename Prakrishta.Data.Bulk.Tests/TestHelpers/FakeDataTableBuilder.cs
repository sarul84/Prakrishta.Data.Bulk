using Prakrishta.Data.Bulk.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public static class FakeDataTableBuilder
    {
        public static DataTable Build(IEnumerable<object> items, ColumnMap[] maps)
        {
            var table = new DataTable();
            foreach (var m in maps)
                table.Columns.Add(m.ColumnName, m.ColumnType);

            foreach (var item in items)
            {
                var row = table.NewRow();
                foreach (var m in maps)
                    row[m.ColumnName] = m.Getter(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }
    }

}
