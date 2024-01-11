namespace RebacExperiments.Blazor.Infrastructure
{
    /// <summary>
    /// A Cached Item.
    /// </summary>
    /// <typeparam name="T">Type of the Value</typeparam>
    public class CacheItem<T>
    {
        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public required T? Value { get; set; }

        /// <summary>
        /// Gets or sets the Cache Expiration Date.
        /// </summary>
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
