using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using Prakrishta.Data.Bulk.Benchmarks.ComparisonBenchmarks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Prakrishta.Data.Bulk.Benchmark.ComparisonBenchmarks
{
    public class RawSqlBulkCopy_Insert_Benchmarks : BulkBenchmarkBase
    {
        [Benchmark]
        public async Task Raw_SqlBulkCopy()
        {
            using var table = ToDataTable(Data);

            await using var conn = new SqlConnection(Harness.ConnectionString);
            await conn.OpenAsync();

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = Table,
                BulkCopyTimeout = 0
            };

            bulk.ColumnMappings.Add(nameof(TestEntity.Id), "Id");
            bulk.ColumnMappings.Add(nameof(TestEntity.Name), "Name");
            bulk.ColumnMappings.Add(nameof(TestEntity.Amount), "Amount");
            bulk.ColumnMappings.Add(nameof(TestEntity.CreatedOn), "CreatedOn");
            bulk.ColumnMappings.Add(nameof(TestEntity.OptionalValue), "OptionalValue");

            await bulk.WriteToServerAsync(table);
        }

        private static DataTable ToDataTable(IEnumerable<TestEntity> data)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("CreatedOn", typeof(DateTime));
            table.Columns.Add("OptionalValue", typeof(int));

            foreach (var e in data)
            {
                table.Rows.Add(e.Id, e.Name, e.Amount, e.CreatedOn, (object?)e.OptionalValue ?? DBNull.Value);
            }

            return table;
        }
    }

}
