using HtmlAgilityPack;
using Realert.Models;

namespace Realert.Scrapers
{
    /*
     * Scrapes new property listings from real estate web pages.
     * Currently supports: Rightmove and Purplebricks
     */
    public class NewListingsWebScraper
    {
        /*
         * Number of results found.
         */
        public int ResultCount;

        /*
         * Private constructor. 
         * Populates public fields with property data collected by InitializeAsync call.
         */
        private NewListingsWebScraper(int results) 
        { 
            ResultCount = results;
        }

        /*
         * Creation method used to create a new NewListingsWebScraper instance.
         */
        public static async Task<NewListingsWebScraper> InitializeAsync(string initialUrl, TargetSite site)
        {
            int results;

            // Fetch all new properties for the specified site
            switch (site)
            {
                case TargetSite.Rightmove:
                    results = await GetRightmoveResults(initialUrl);
                    break;
                case TargetSite.Purplebricks:
                    results = await GetPurplebricksResults(initialUrl);
                    break;
                default:
                    throw new ArgumentException("Site is invalid/not supported.");
            }

            return new NewListingsWebScraper(results);
        }

        /*
         * Gets the number of property results that match the search url for Rightmove. 
         */
        private static async Task<int> GetRightmoveResults(string initialUrl)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");

            // Get the first page of results
            string page = await client.GetStringAsync(initialUrl);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(page);

            // XPath expression for the element containing the number of results.
            const string resultCountElement = "//span[contains(@class, 'searchHeader-resultCount')]";

            // Get the total number of results.
            var resultCountNode = htmlDoc.DocumentNode.SelectSingleNode(resultCountElement);
            if (int.TryParse(resultCountNode.InnerText, out int resultCount))
            {
                return resultCount;
            }

            // No total results value found, assume there are no results.
            return 0;
        }

        /*
         * Gets the number of property results that match the search url for Purplebricks. 
         */
        private static async Task<int> GetPurplebricksResults(string initialUrl)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");

            // Get the first page of results
            string page = await client.GetStringAsync(initialUrl);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(page);

            // XPath expression for the element containing the number of results.
            const string resultCountElement = "//span[@data-testid='search-results-number']";

            // Get the total number of results.
            var resultCountParent = htmlDoc.DocumentNode.SelectSingleNode(resultCountElement);
            var resultCountNode = resultCountParent.SelectSingleNode("./strong");
            if (int.TryParse(resultCountNode.InnerText, out int resultCount))
            {
                return resultCount;
            }

            // No total results value found, assume there are no results.
            return 0;
        }
    }
}
