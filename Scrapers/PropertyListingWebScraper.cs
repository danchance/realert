using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Realert.Interfaces;
using Realert.Models;

namespace Realert.Scrapers
{
    /// <summary>
    /// Class <c>PropertyListingWebScraper</c> scrapes data about a specific property listing from real estate
    /// web pages. Websites currently supported are: Rightmove and Purplebricks.
    /// </summary>
    public sealed class PropertyListingWebScraper : IWebScraper<PropertyListingWebScraper>
    {
        private PropertyListingWebScraper(string data, TargetSite site)
        {
            // Parse data based on the website it was scraped from.
            switch (site)
            {
                case TargetSite.Rightmove:
                    this.ParseRightmoveData(data);
                    break;
                case TargetSite.Purplebricks:
                    this.ParsePurplebricksData(data);
                    break;
            }
        }

        /// <value>
        /// Name/Address of the property.
        /// </value>
        public string? PropertyName { get; private set; }

        /// <value>
        /// Price of the property.
        /// </value>
        public int PropertyPrice { get; private set; }

        /// <summary>
        /// Method used to create a new <c>PropertyListingWebScraper</c> instance. Scrapes the supplied url making,
        /// data available on the Properties.
        /// </summary>
        /// <param name="url">Property url.</param>
        /// <param name="site">Site being searched.</param>
        /// <returns><c>PropertyListingWebScraper</c> instance.</returns>
        /// <exception cref="ArgumentException">site must be a supported.</exception>
        public static async Task<PropertyListingWebScraper> InitializeAsync(string url, TargetSite site)
        {
            // Throw error if an unsupported/invalid site is passed.
            if (site != TargetSite.Rightmove && site != TargetSite.Purplebricks)
            {
                throw new ArgumentException("Site is invalid/not supported.");
            }

            // Request data from the specified url.
            HttpClient client = new ();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Invalid URL");
            }

            string page = await response.Content.ReadAsStringAsync();

            return new PropertyListingWebScraper(page, site);
        }

        /// <summary>
        /// Method parses the HTML response for a Rightmove property listing and extracts the property
        /// name and price.
        /// </summary>
        /// <param name="html">Rightmove HTML data.</param>
        /// <exception cref="ArgumentException">No price found.</exception>
        private void ParseRightmoveData(string html)
        {
            HtmlDocument htmlDoc = new ();
            htmlDoc.LoadHtml(html);

            // XPath expressions for the nodes containing property information.
            const string propertyNameElement = "//h1[@itemprop='streetAddress']";
            const string propertyPriceElement = "//input[@name='propertyValue']";

            // Property Name.
            var propertyNameNode = htmlDoc.DocumentNode.SelectSingleNode(propertyNameElement);
            this.PropertyName = propertyNameNode.InnerText;

            // Property Price, throw an error if its not numeric.
            var propertyPriceNode = htmlDoc.DocumentNode.SelectSingleNode(propertyPriceElement);
            string price = propertyPriceNode.GetAttributeValue("value", "").Replace(",", "");
            if (int.TryParse(price, out int propertyPrice))
            {
                this.PropertyPrice = propertyPrice;
            }
            else
            {
                throw new ArgumentException("Cannot find price for this property");
            }
        }

        /// <summary>
        /// Method parses the JSON response for a Purplebricks property listing and extracts the property
        /// name and price.
        /// </summary>
        /// <param name="json">Purplebricks JSON data.</param>
        /// <exception cref="ArgumentException">No price found.</exception>
        private void ParsePurplebricksData(string json)
        {
            JToken token = JObject.Parse(json);

            // Property Name.
            this.PropertyName = (string?)token.SelectToken("address");

            // Property Price, throw an error if its not numeric.
            if (int.TryParse((string?)token.SelectToken("marketPrice"), out int propertyPrice))
            {
                this.PropertyPrice = propertyPrice;
            }
            else
            {
                throw new ArgumentException("Cannot find price for this property");
            }
        }
    }
}