using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /*
     * Model for the New Property Alert Nofication table which stores all active
     * email notifcations for new property alerts.
     */
    public class NewPropertyAlertNotification
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        /*
         * Name the user gave the notification.
         */
        [Required]
        [StringLength(100)]
        public string? NotificationName { get; set; }

        /*
         * How frequently a scan will occur to check for new properties.
         */
        [RegularExpression("1|3|7")]
        public byte NotificationFrequency { get; set; }
        [DataType(DataType.Date)]
        public DateTime LastScannedDate { get; set; }

        /*
         * The site to search for properties on
         */
        public TargetSite TargetSite { get; set; }

        /*
         * Property search parameters that define the type of property to search
         * for.
         */
        public string? PropertyType { get; set; }
        [Required]
        public string? Location { get; set; }
        public float SearchRadius { get; set; }
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; }
        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10")]
        public string? MinBeds { get; set; }
        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10")]
        public string? MaxBeds { get; set; }

        /*
         * Date notification was first setup.
         */
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; private set; }

        /*
         * As there are no user accounts, DeleteCode is used to confirm the user 
         * deleting the notification is the one who is receiving the emails.
         */
        [Required]
        public string DeleteCode { get; private set; }

        /*
         * Set values for the CreatedAt timestamp and the DeleteCode.
         */
        public NewPropertyAlertNotification() 
        {
            CreatedAt = DateTime.Today;
            DeleteCode = GenerateCode();
        }

        /*
         * Function generates a "random" 10 digit alphanumeric code for the 
         * DeleteCode field. 
         */
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

        /*
         * Get the url used to search for new properties.
         */
        public string GetUrl()
        {
            switch (TargetSite)
            {
                case TargetSite.Rightmove:
                    return BuildRightmoveUrl();
                case TargetSite.Purplebricks:
                    return BuildPurplebricksUrl();
            }

            return "";
        }

        /*
         * Build new property search url for Rightmove.
         */
        private string BuildRightmoveUrl()
        {
            UriBuilder baseUri = new("https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE");

            // Build query string.
            string query = $"locationIdentifier=OUTCODE%5E{Location}&radius={SearchRadius}&minBedrooms={MinBeds}&maxBedrooms={MaxBeds}&displayPropertyType={PropertyType}";

            // If MinPrice or MaxPrice are 0, this means the user entered no value, i.e. no price
            // limit, so don't add a price query.
            if(MinPrice != 0)
            {
                query += $"&minPrice={MinPrice}";
            }
            if (MaxPrice != 0)
            {
                query += $"&maxPrice={MaxPrice}";
            }
            baseUri.Query += "&" + query ;

            return baseUri.ToString();
        }

        /*
         * Build new property serach url for Purplebricks.
         */
        private string BuildPurplebricksUrl()
        {
            UriBuilder baseUri = new("https://www.purplebricks.co.uk/search/property-for-sale/");

            // Build query string.
            string query = $"location={Location}&searchRadius={SearchRadius}&bedroomsFrom={MinBeds}&bedroomsTo={MaxBeds}";

            // If MinPrice or MaxPrice are 0, this means the user entered no value, i.e. no price
            // limit, so don't add a price query.
            if (MinPrice != 0)
            {
                query += $"&priceFrom={MinPrice}";
            }
            if (MaxPrice != 0)
            {
                query += $"&priceTo={MaxPrice}";
            }
            baseUri.Query += "&" + query ;

            return baseUri.ToString();
        }
    }
}
