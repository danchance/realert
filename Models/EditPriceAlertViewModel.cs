using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class EditPriceAlertViewModel
    {

        public Notification NotificationType { get; set; }

        public TargetSite TargetSite { get; set; }

        public string? ListingLink { get; set; }

        public int PriceThreshold { get; set; }

        public bool NotifyOnPriceIncrease { get; set; }

        public bool NotifyOnPropertyDelist { get; set; }

        public string? Note { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        public string? DeleteCode { get; set; }

        public PriceAlertProperty? Property { get; set; }
    }
}
