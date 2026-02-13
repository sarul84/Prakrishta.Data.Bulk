namespace Prakrishta.Data.Bulk.Factories
{
    using Prakrishta.Data.Bulk.Abstractions;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Data.Common;
    using System.Threading;

    public sealed class PooledConnectionFactory(IDbConnectionFactory inner, int maxPoolSize) : IDbConnectionFactory
    {
        private readonly IDbConnectionFactory _inner = inner;
        private readonly ConcurrentBag<DbConnection> _pool = new();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(maxPoolSize, maxPoolSize);

        public DbConnection Create(string connectionString)
        {
            if (_pool.TryTake(out var conn))
            {
                if (conn.State == ConnectionState.Closed)
                    return conn;

                conn.Dispose();
            }

            return _inner.Create(connectionString);
        }

        public async Task<DbConnection> CreateAsync(string connectionString)
        {
            await _semaphore.WaitAsync();

            try
            {
                return Create(connectionString);
            }
            catch
            {
                _semaphore.Release();
                throw;
            }
        }

        public void Return(DbConnection connection)
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            catch
            {
                connection.Dispose();
                return;
            }

            _pool.Add(connection);
            _semaphore.Release();
        }
    }
}
