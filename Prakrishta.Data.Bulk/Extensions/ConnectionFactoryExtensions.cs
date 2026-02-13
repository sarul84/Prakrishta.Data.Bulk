using Prakrishta.Data.Bulk.Abstractions;

namespace Prakrishta.Data.Bulk.Extensions
{
    public static class ConnectionFactoryExtensions
    {
        public static async Task WarmUpAsync(
            this IDbConnectionFactory factory,
            string connectionString)
        {
            using var conn = factory.Create(connectionString);
            await conn.OpenAsync();
            conn.Close();
        }
    }
}
