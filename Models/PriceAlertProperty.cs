using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class PriceAlertProperty
    {
        public int Id { get; set; }
        public string? PropertyName { get; set; }
        public int FirstScannedPrice { get; set; }
        public int LastScannedPrice { get; set; }
        public int NumberOfPriceChanges { get; set; }
        [DataType(DataType.Date)]
        public DateTime LastPriceChangeDate { get; set; }

        public int PriceAlertNotificationId { get; set; }
        public virtual PriceAlertNotification? Notification { get; set; }
    }
}
