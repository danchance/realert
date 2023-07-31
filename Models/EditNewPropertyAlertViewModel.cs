using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Realert.Models
{
    /// <summary>
    /// View Model <see cref="EditNewPropertyAlertViewModel"/> used to populate the form for editing or deleting
    /// New Property Alerts.
    /// </summary>
    public class EditNewPropertyAlertViewModel
    {
        // Fields.
        private string? minBeds;
        private string? maxBeds;

        // Properties.

        /// <value>
        /// User friendly name for the notification.
        /// </value>
        public string? NotificationName { get; set; }

        /// <value>
        /// Frequency the user wants to recieve notifications.
        /// </value>
        public byte NotificationFrequency { get; set; }

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
        public string? MinBeds
        {
            get { return this.minBeds; }
            set { this.minBeds = value ?? "No min"; }
        }

        /// <value>
        /// Search criteria: Max number of bedrooms.
        /// </value>
        public string? MaxBeds
        {
            get { return this.maxBeds; }
            set { this.maxBeds = value ?? "No max"; }
        }

        /// <value>
        /// Date the notification was setup.
        /// </value>
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        /// <value>
        /// Code used to verify that only the user receiving the emails can delete and/or edit
        /// the New Property notification. Needed as there are no user accounts.
        /// </value>
        public string? DeleteCode { get; set; }

        /// <summary>
        /// Gets the Tailwind background color class associated with Rightmove or Purplebricks.
        /// </summary>
        public string TargetSiteColorClass
        {
            get
            {
                return this.TargetSite == TargetSite.Rightmove ? "bg-green-400" : "bg-purple-700";
            }
        }

        /// <summary>
        /// Gets the property type in a displayable format.
        /// </summary>
        public string FormattedPropertyType
        {
            get
            {
                var propertyDictionary = new Dictionary<string, string>
                {
                    { "houses", "Houses" },
                    { "flats", "Flats" },
                    { "commercial", "Commercial Property" },
                    { "land", "Land" },
                    { "bungalows", "Bungalows" },
                    { "other", "Other Property" },
                };
                return this.PropertyType == null ? "Properties" : propertyDictionary[this.PropertyType];
            }
        }
    }
}
