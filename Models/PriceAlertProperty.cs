using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// Model <see cref="PriceAlertProperty"/> holds details of a property for a Price Alert.
    /// </summary>
    public class PriceAlertProperty
    {
        // Fields.
        private int lastScannedPrice;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceAlertProperty"/> class.
        /// Set initial values for the NumberOfPriceChanges timestamp and the LastPriceChangeDate.
        /// </summary>
        public PriceAlertProperty()
        {
            this.NumberOfPriceChanges = 0;
            this.LastPriceChangeDate = DateTime.Today;
        }

        // Properties.

        /// <value>
        /// Unique Id.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Name/address of the property.
        /// </value>
        public string? PropertyName { get; set; }

        /// <value>
        /// Price of the property when the notification was first setup.
        /// </value>
        public int FirstScannedPrice { get; set; }

        /// <value>
        /// Price of the property as of the last scan.
        /// </value>
        public int LastScannedPrice
        {
            get
            {
                return this.lastScannedPrice;
            }

            set
            {
                this.LastPriceChangeDate = DateTime.Today;
                if (value != this.FirstScannedPrice)
                {
                    this.NumberOfPriceChanges++;
                }

                this.lastScannedPrice = value;
            }
        }

        /// <value>
        /// Number of times the property price has increased/decreased.
        /// </value>
        public int NumberOfPriceChanges { get; private set; }

        /// <value>
        /// Date of the last price change.
        /// </value>
        [DataType(DataType.Date)]
        public DateTime LastPriceChangeDate { get; private set; }

        /// <value>
        /// Associated Notification.
        /// </value>
        public int PriceAlertNotificationId { get; set; }

        /// <value>
        /// Associated Notification.
        /// </value>
        public virtual PriceAlertNotification? Notification { get; set; }
    }
}
