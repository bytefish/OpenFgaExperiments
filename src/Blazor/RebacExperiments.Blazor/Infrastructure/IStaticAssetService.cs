namespace RebacExperiments.Blazor.Infrastructure
{
    public interface IStaticAssetService
    {
        public Task<string?> GetAsync(string assetUrl, bool useCache = false);
    }
}
