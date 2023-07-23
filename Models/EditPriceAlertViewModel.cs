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

        private string? _note;
        public string? Note 
        { 
            get { return _note; } 
            set { _note = value ?? "Looks like you didn't leave a note :("; }
        }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        public string? DeleteCode { get; set; }

        public bool? IsPriceDecrease
        {
            get 
            {
                return Property == null ? null :  
                    Property.LastScannedPrice < Property.FirstScannedPrice;
            }
        }

        public PriceAlertProperty? Property { get; set; }

        public string TargetSiteColorClass 
        { 
            get 
            { 
                return TargetSite == TargetSite.Rightmove ? "bg-green-400" : "bg-purple-700"; 
            } 
        }

        public string NotifyOnPriceIncreaseString
        {
            get
            {
                return NotifyOnPriceIncrease ? "Yes" : "No";
            }
        }

        public string NotifyOnPropertyDelistString
        {
            get
            {
                return NotifyOnPropertyDelist ? "Yes" : "No";
            }
        }
    }
}
