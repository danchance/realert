using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realert.Models;

namespace Realert.Scrapers.PriceAlertNotification
{
    /*
     * Scrapes information about a specific property listing from real estate
     * web pages.
     * Currently supports: Rightmove and Purplebricks.
     */
    public class PropertyWebScraper
    {

        /*
         * Property data.
         */
        public string? PropertyName { get; private set; }
        public string? PropertyPrice { get; private set; }

        /*
         * Private constructor. Class is created using InitializeAsync().
         * Parses the raw data and populates the property fields.
         */
        private PropertyWebScraper(string data, TargetSite site) {
            // Parse data based on the website is was scraped from.
            switch (site)
            {
                case TargetSite.Rightmove:
                    ParseRightmoveHTML(data);
                    break;
                case TargetSite.Purplebricks:
                    ParsePurplebricksJSON(data);
                    break;
            }
        }

        /*
         * Creation method used to create a new PropertyScraper object.
         */
        public static async Task<PropertyWebScraper> InitializeAsync(string url, TargetSite site)
        {
            // Throw error if an unsupported/invalid site is passed.
            if (site != TargetSite.Rightmove &&  site != TargetSite.Purplebricks)
            {
                throw new ArgumentException("Site is invalid/not supported.");
            }

            // Request data from the specified url.
            HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
            string response = await client.GetStringAsync(url);

            return new PropertyWebScraper(response, site);
        }

        /*
         * Function parses the HTML of a Rightmove property listing page, to extract
         * the property name and price.
         */
        private void ParseRightmoveHTML(string html)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            // XPath expressions for the nodes containing property information.
            const string propertyNameElement = "//h1[@itemprop='streetAddress']";
            const string propertyPriceElement = "//input[@name='propertyValue']";

            // Property Name.
            var propertyNameNode = htmlDoc.DocumentNode.SelectSingleNode(propertyNameElement);
            PropertyName = propertyNameNode.InnerText;

            // Property Price.
            var propertyPriceNode = htmlDoc.DocumentNode.SelectSingleNode(propertyPriceElement);
            PropertyPrice = propertyPriceNode.GetAttributeValue("value", "");
        }

        /*
         * Function parses the JSON of a Purplebricks property listing, to extract
         * the property name and price.
         */
        private void ParsePurplebricksJSON(string json)
        {
            // Parse the json string and extract the property name and price.
            JToken token = JObject.Parse(json);
            PropertyName = (string?)token.SelectToken("address");
            PropertyPrice = (string?)token.SelectToken("marketPrice");
        }
    }
}

