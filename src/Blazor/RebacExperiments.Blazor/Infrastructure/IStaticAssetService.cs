namespace RebacExperiments.Blazor.Infrastructure
{
    /// <summary>
    /// Service for loading Static Assets.
    /// </summary>
    public interface IStaticAssetService
    {
        /// <summary>
        /// Gets a static asset by the <paramref name="assetUrl"/>. The Browser cache can be used 
        /// to prevent reloading the data. 
        /// </summary>
        /// <param name="assetUrl">Asset URL pointing to the data</param>
        /// <param name="useCache">Signals wether to use the Browser Cache</param>
        /// <returns>The asset as a String</returns>
        public Task<string?> GetAsync(string assetUrl, bool useCache = false);
    }
}
