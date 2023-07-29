using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /*
     * Information about a property for a Price Alert.
     */
    public class PriceAlertProperty
    {
        public int Id { get; set; }

        /*
         * Property name/address.
         */
        public string? PropertyName { get; set; }

        /*
         * Price when notification was first setup.
         */ 
        public int FirstScannedPrice { get; set; }

        /*
         * Price as of the last scan.
         */
        private int _lastScannedPrice;
        public int LastScannedPrice
        {
            get { return _lastScannedPrice;  }
            set 
            {
                LastPriceChangeDate = DateTime.Today;
                if (value != FirstScannedPrice) 
                {
                    NumberOfPriceChanges++;
                }
                _lastScannedPrice = value;
            }
        }

        /*
         * Number of times the price has increased/decreased.
         */
        public int NumberOfPriceChanges { get; private set; }

        /*
         * Date of the last price change.
         */
        [DataType(DataType.Date)]
        public DateTime LastPriceChangeDate { get; private set; }

        /*
         * Associated notification.
         */
        public int PriceAlertNotificationId { get; set; }
        public virtual PriceAlertNotification? Notification { get; set; }

        /*
         * Set initial values.
         */
        public PriceAlertProperty()
        {
            NumberOfPriceChanges = 0;
            LastPriceChangeDate = DateTime.Today;
        }
    }
}
