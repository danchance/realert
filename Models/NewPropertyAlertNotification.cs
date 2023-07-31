using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// Model <see cref="NewPropertyAlertNotification"/> holds details of all active New Property Alerts.
    /// </summary>
    public class NewPropertyAlertNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewPropertyAlertNotification"/> class.
        /// Set values for the CreatedAt timestamp and the DeleteCode.
        /// </summary>
        public NewPropertyAlertNotification()
        {
            this.CreatedAt = DateTime.Today;
            this.DeleteCode = GenerateCode();
        }

        /// <value>
        /// Unique Id.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Email address to send the notifications to.
        /// </value>
        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        /// <value>
        /// User friendly name for the notification.
        /// </value>
        [Required]
        [StringLength(100)]
        public string? NotificationName { get; set; }

        /// <value>
        /// Frequency the user wants to recieve notifications.
        /// </value>
        [RegularExpression("1|3|7")]
        public byte NotificationFrequency { get; set; }

        /// <value>
        /// Date the last scan was performed.
        /// </value>
        [DataType(DataType.Date)]
        public DateTime? LastScannedDate { get; set; }

        /// <value>
        /// The real estate site to search for properties on.
        /// </value>
        public TargetSite TargetSite { get; set; }

        /// <value>
        /// Search criteria: Type of property to search for.
        /// </value>
        public string? PropertyType { get; set; }

        /// <value>
        /// Search criteria: Location to search for properties.
        /// </value>
        [Required]
        public string? Location { get; set; }

        /// <value>
        /// Search criteria: Search radius from the location.
        /// </value>
        public float SearchRadius { get; set; }

        /// <value>
        /// Search criteria: Min property price.
        /// </value>
        public uint MinPrice { get; set; }

        /// <value>
        /// Search criteria: Max property price.
        /// </value>
        public uint MaxPrice { get; set; }

        /// <value>
        /// Search criteria: Min number of bedrooms.
        /// </value>
        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10")]
        public string? MinBeds { get; set; }

        /// <value>
        /// Search criteria: Max number of bedrooms.
        /// </value>
        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10")]
        public string? MaxBeds { get; set; }

        /// <value>
        /// Date the notification was setup.
        /// </value>
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; private set; }

        /// <value>
        /// Code used to verify that only the user receiving the emails can delete and/or edit
        /// the New Property notification. Needed as there are no user accounts.
        /// </value>
        [Required]
        public string DeleteCode { get; private set; }

        /// <summary>
        /// Get the url to use for searching for new properties that match the search criteria.
        /// </summary>
        /// <returns>Search url.</returns>
        public string GetUrl()
        {
            return this.TargetSite switch
            {
                TargetSite.Rightmove => this.BuildRightmoveUrl(),
                TargetSite.Purplebricks => this.BuildPurplebricksUrl(),
                _ => "",
            };
        }

        /// <summary>
        /// Method generates a "random" 10 digit alphanumeric code.
        /// </summary>
        /// <returns>10 character "random" string.</returns>
        private static string GenerateCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[10];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        /// Build the new property search url for Rightmove.
        /// </summary>
        /// <returns>Search url.</returns>
        private string BuildRightmoveUrl()
        {
            UriBuilder baseUri = new ("https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE");

            // Build query string.
            string query = $"locationIdentifier=OUTCODE%5E{this.Location}&radius={this.SearchRadius}&minBedrooms=" +
                $"{this.MinBeds}&maxBedrooms={this.MaxBeds}&displayPropertyType={this.PropertyType}";

            // If MinPrice or MaxPrice are 0, this means the user entered no value, i.e. no price
            // limit, so don't add a price query.
            if (this.MinPrice != 0)
            {
                query += $"&minPrice={this.MinPrice}";
            }

            if (this.MaxPrice != 0)
            {
                query += $"&maxPrice={this.MaxPrice}";
            }

            baseUri.Query += "&" + query;

            return baseUri.ToString();
        }

        /// <summary>
        /// Build the new property search url for Purplebricks.
        /// </summary>
        /// <returns>Search url.</returns>
        private string BuildPurplebricksUrl()
        {
            UriBuilder baseUri = new ("https://www.purplebricks.co.uk/search/property-for-sale/");

            // Build query string.
            string query = $"location={this.Location}&searchRadius={this.SearchRadius}&bedroomsFrom={this.MinBeds}&bedroomsTo={this.MaxBeds}";

            // If MinPrice or MaxPrice are 0, this means the user entered no value, i.e. no price
            // limit, so don't add a price query.
            if (this.MinPrice != 0)
            {
                query += $"&priceFrom={this.MinPrice}";
            }

            if (this.MaxPrice != 0)
            {
                query += $"&priceTo={this.MaxPrice}";
            }

            baseUri.Query += "&" + query;

            return baseUri.ToString();
        }
    }
}
