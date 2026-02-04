using System.Data;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public static class DataTableTestFactory
    {
        public static DataTable CreateSampleTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));

            table.Rows.Add(1, "A");
            table.Rows.Add(2, "B");

            return table;
        }
    }

}
