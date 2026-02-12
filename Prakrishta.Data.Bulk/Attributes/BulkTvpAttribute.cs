namespace Prakrishta.Data.Bulk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BulkTvpAttribute : Attribute
    {
        public string TvpName { get; }
        public BulkTvpAttribute(string tvpName) => TvpName = tvpName;
    }
}
