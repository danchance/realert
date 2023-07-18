using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class PriceAlertSetupViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Range(1000, 10000)]
        public uint PriceThreshold { get; set; }

        [Required (ErrorMessage = "Please enter the link to the propery listing.")]
        [StringLength(1000)]
        public string? ListingLink { get; set; }

        public bool NotifyOnPriceIncrease { get; set; }

        public bool NotifyOnPropertyDelist { get; set; }

        public string? NotificationType { get; set; }

        [StringLength(10000)]
        public string? Note { get; set; }
    }
}
