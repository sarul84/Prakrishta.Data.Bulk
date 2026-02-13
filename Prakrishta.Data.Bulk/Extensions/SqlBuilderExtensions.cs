namespace Prakrishta.Data.Bulk.Extensions
{
    using Microsoft.Data.SqlClient;
    using System.Data;
    using System.Data.Common;

    public static class SqlBuilderExtensions
    {
        public static DbParameter AddParam(
            this DbCommand cmd,
            string name,
            object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;

            cmd.Parameters.Add(p);
            return p;
        }

        public static DbParameter AddStructured(
            this DbCommand cmd,
            string name,
            object value,
            string typeName)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            p.SourceColumn = typeName;

            if (p is SqlParameter sqlParam)
            {
                sqlParam.SqlDbType = SqlDbType.Structured;
                sqlParam.TypeName = typeName;
            }

            cmd.Parameters.Add(p);
            return p;
        }
    }

}
