using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class PriceAlertSetupViewModel
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        [Range(1000, 100000)]
        public uint PriceThreshold { get; set; }

        [Required]
        [StringLength(1000)]
        public string? ListingLink { get; set; }

        public bool NotifyOnPriceIncrease { get; set; }

        public bool NotifyOnPropertyDelist { get; set; }

        [Required]
        public string? NotificationType { get; set; }

        [StringLength(10000)]
        public string? Note { get; set; }
    }
}
