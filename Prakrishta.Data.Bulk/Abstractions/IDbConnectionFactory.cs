namespace Prakrishta.Data.Bulk.Abstractions
{
    using System.Data.Common;

    public interface IDbConnectionFactory
    {
        DbConnection Create(string connectionString);
    }
}
