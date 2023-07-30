using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Realert.Interfaces;
using Realert.Models;

namespace Realert.Scrapers
{
    /*
     * Scrapes information about a specific property listing from real estate
     * web pages.
     * Currently supports: Rightmove and Purplebricks.
     */
    public class PropertyListingWebScraper : IWebScraper<PropertyListingWebScraper>
    {

        /*
         * Property data.
         */
        public string? PropertyName { get; private set; }
        public int PropertyPrice { get; private set; }

        /*
         * Private constructor. 
         * Parses the raw data and populates the property fields.
         */
        private PropertyListingWebScraper(string data, TargetSite site)
        {
            // Parse data based on the website it was scraped from.
            switch (site)
            {
                case TargetSite.Rightmove:
                    ParseRightmoveData(data);
                    break;
                case TargetSite.Purplebricks:
                    ParsePurplebricksData(data);
                    break;
            }
        }

        /*
         * Creation method used to create a new PropertyListingWebScraper instance.
         */
        public static async Task<PropertyListingWebScraper> InitializeAsync(string url, TargetSite site)
        {
            // Throw error if an unsupported/invalid site is passed.
            if (site != TargetSite.Rightmove && site != TargetSite.Purplebricks)
            {
                throw new ArgumentException("Site is invalid/not supported.");
            }

            // Request data from the specified url.
            HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Invalid URL");
            }
            string page = await response.Content.ReadAsStringAsync();

            return new PropertyListingWebScraper(page, site);
        }

        /*
         * Parses the HTML of a Rightmove property listing page, to extract the
         * property name and price.
         */
        private void ParseRightmoveData(string html)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            // XPath expressions for the nodes containing property information.
            const string propertyNameElement = "//h1[@itemprop='streetAddress']";
            const string propertyPriceElement = "//input[@name='propertyValue']";

            // Property Name.
            var propertyNameNode = htmlDoc.DocumentNode.SelectSingleNode(propertyNameElement);
            PropertyName = propertyNameNode.InnerText;

            // Property Price, throw an error if its not numeric.
            var propertyPriceNode = htmlDoc.DocumentNode.SelectSingleNode(propertyPriceElement);
            string price = propertyPriceNode.GetAttributeValue("value", "").Replace(",", "");
            if (int.TryParse(price, out int propertyPrice)) 
            {
                PropertyPrice = propertyPrice;
            } 
            else
            {
                throw new ArgumentException("Cannot find price for this property");
            }
        }

        /*
         * Parses the JSON of a Purplebricks property listing, to extract the
         * property name and price.
         */
        private void ParsePurplebricksData(string json)
        {
            JToken token = JObject.Parse(json);

            // Property Name.
            PropertyName = (string?)token.SelectToken("address");

            // Property Price, throw an error if its not numeric.
            if (int.TryParse((string?)token.SelectToken("marketPrice"), out int propertyPrice))
            {
                PropertyPrice = propertyPrice;
            }
            else
            {
                throw new ArgumentException("Cannot find price for this property");
            }
        }
    }
}

