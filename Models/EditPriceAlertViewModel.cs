using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// View Model <see cref="EditPriceAlertViewModel"/> used to populate the form for editing or deleting
    /// Price Alerts.
    /// </summary>
    public class EditPriceAlertViewModel
    {
        // Fields.
        private string? note;

        /// <value>
        /// Type of notification the user wants to receive, either email or text.
        /// </value>
        public Notification NotificationType { get; set; }

        /// <value>
        /// The real estate site the property listing is on.
        /// </value>
        public TargetSite TargetSite { get; set; }

        /// <value>
        /// Link to the property the Price Alert is for.
        /// </value>
        public string? ListingLink { get; set; }

        /// <value>
        /// The minimum amount the price must change before a notification is sent.
        /// </value>
        public int PriceThreshold { get; set; }

        /// <value>
        /// Determines if the user should be notified if the property price increases.
        /// </value>
        public bool NotifyOnPriceIncrease { get; set; }

        /// <value>
        /// Determines if the user should be notified when the property is delisted.
        /// </value>
        public bool NotifyOnPropertyDelist { get; set; }

        /// <value>
        /// User note containing any details about the notification.
        /// </value>
        public string? Note
        {
            get { return this.note; }
            set { this.note = value ?? "Looks like you didn't leave a note :("; }
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

        /// <value>
        /// True if property has decreased since the notification was setup.
        /// </value>
        public bool? IsPriceDecrease
        {
            get
            {
                return this.Property == null ? null :
                    this.Property.LastScannedPrice < this.Property.FirstScannedPrice;
            }
        }

        /// <value>
        /// Property the price alert is for.
        /// </value>
        public PriceAlertProperty? Property { get; set; }

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
        /// Gets the NotifyOnPriceIncrease option in a displayable format.
        /// </summary>
        public string NotifyOnPriceIncreaseString
        {
            get
            {
                return this.NotifyOnPriceIncrease ? "Yes" : "No";
            }
        }

        /// <summary>
        /// Gets the NotifyOnPropertyDelist option in a displayable format.
        /// </summary>
        public string NotifyOnPropertyDelistString
        {
            get
            {
                return this.NotifyOnPropertyDelist ? "Yes" : "No";
            }
        }
    }
}
