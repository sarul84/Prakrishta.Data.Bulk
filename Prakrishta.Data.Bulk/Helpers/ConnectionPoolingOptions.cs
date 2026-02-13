namespace Prakrishta.Data.Bulk.Helpers
{
    public sealed class ConnectionPoolingOptions
    {
        public TimeSpan? MaxConnectionAge { get; set; }
        public int MaxPoolSize { get; set; } = 100;
    }
}
