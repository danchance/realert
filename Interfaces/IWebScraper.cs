using Realert.Models;

namespace Realert.Interfaces
{
    public interface IWebScraper<T>
    {
        static abstract Task<T> InitializeAsync(string url, TargetSite site);
    }
}
