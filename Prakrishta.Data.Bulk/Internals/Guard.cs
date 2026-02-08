namespace Prakrishta.Data.Bulk.Internals
{
    public static class Guard
    {
        public static void NotNull<T>(T value, string paramName)
            where T : class
        {
            if (value is null)
                throw new ArgumentNullException(paramName);
        }

        public static void NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", paramName);
        }

        public static void Positive(int value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName, "Value must be positive.");
        }
    }
}
