using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// View Model <see cref="NewPropertyAlertSetupViewModel"/> used to add a New Propery Alert.
    /// </summary>
    public class NewPropertyAlertSetupViewModel
    {
        /// <value>
        /// Email address to send notifications to.
        /// </value>
        [Required(ErrorMessage = "Please enter your email address.")]
        [StringLength(100)]
        public string? Email { get; set; }

        /// <value>
        /// User friendly name for the notification.
        /// </value>
        [Required(ErrorMessage = "Please give this notification a name")]
        [StringLength(100)]
        public string? NotificationName { get; set; }

        /// <value>
        /// The real estate site to search for properties on.
        /// </value>
        public TargetSite TargetSite { get; set; }

        /// <value>
        /// Frequency the user wants to recieve notifications.
        /// </value>
        [Required]
        [RegularExpression("1|3|7", ErrorMessage = "Valid values are 1, 3 or 7 days")]
        public byte NotificationFrequency { get; set; }

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
        [Range(0, 40)]
        public float SearchRadius { get; set; }

        /// <value>
        /// Search criteria: Min property price.
        /// </value>
        [Range(0, 20000000)]
        public uint MinPrice { get; set; }

        /// <value>
        /// Search criteria: Max property price.
        /// </value>
        [Range(0, 20000000)]
        public uint MaxPrice { get; set; }

        /// <value>
        /// Search criteria: Min number of bedrooms.
        /// </value>
        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10", ErrorMessage = "Valid values are: ' ' and 0-10")]
        public string? MinBeds { get; set; }

        /// <value>
        /// Search criteria: Max number of bedrooms.
        /// </value>
        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10", ErrorMessage = "Valid values are: ' ' and 0-10")]
        public string? MaxBeds { get; set; }

        /// <value>
        /// Search url from the real estate website. Used to extract information from.
        /// </value>
        [Required]
        public string? SearchLink { get; set; }
    }
}
