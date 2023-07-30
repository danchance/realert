using HtmlAgilityPack;
using Realert.Interfaces;
using Realert.Models;

namespace Realert.Scrapers
{
    /// <summary>
    /// Class <c>NewListingsWebScraper</c> scrapes new property listings from real estate web pages.
    /// Websites currently supported are: Rightmove and Purplebricks.
    /// </summary>
    public sealed class NewListingsWebScraper : IWebScraper<NewListingsWebScraper>
    {
        private NewListingsWebScraper(int results)
        {
            this.ResultCount = results;
        }

        /// <value>
        /// Number of new property listings.
        /// </value>
        public int ResultCount { get; set; }

        /// <summary>
        /// Method used to create a new <c>NewListingsWebScraper</c> instance. Scrapes the supplied url making,
        /// data available on the Properties.
        /// </summary>
        /// <param name="url">Search url.</param>
        /// <param name="site">Site being searched.</param>
        /// <returns><c>NewListingsWebScraper</c> instance.</returns>
        /// <exception cref="ArgumentException">site must be a supported.</exception>
        public static async Task<NewListingsWebScraper> InitializeAsync(string url, TargetSite site)
        {
            // Fetch new property data for the specified site.
            var results = site switch
            {
                TargetSite.Rightmove => await GetRightmoveResults(url),
                TargetSite.Purplebricks => await GetPurplebricksResults(url),
                _ => throw new ArgumentException("Site is invalid/not supported."),
            };

            return new NewListingsWebScraper(results);
        }

        /// <summary>
        /// Method fetches new listing information for Rightmove.
        /// </summary>
        /// <param name="url">Rightmove search url.</param>
        /// <returns>Number of new listings.</returns>
        /// <exception cref="ArgumentException">Invalid Url error.</exception>
        private static async Task<int> GetRightmoveResults(string url)
        {
            HttpClient client = new ();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");

            // Get the first page of results.
            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Invalid URL");
            }

            string page = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDoc = new ();
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

        /// <summary>
        /// Method fetches new listing information for Purplebricks.
        /// </summary>
        /// <param name="url">Rightmove search url.</param>
        /// <returns>Number of new listings.</returns>
        /// <exception cref="ArgumentException">Invalid Url error.</exception>
        private static async Task<int> GetPurplebricksResults(string url)
        {
            HttpClient client = new ();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");

            // Get the first page of results.
            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Invalid URL");
            }

            string page = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDoc = new ();
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
