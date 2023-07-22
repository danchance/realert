﻿using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class NewPropertyAlertSetupViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please give this notification a name")]
        [StringLength(100)]
        public string? NotificationName { get; set; }

        public TargetSite TargetSite { get; set; }

        [Required]
        [RegularExpression("1|3|7", ErrorMessage = "Valid values are 1, 3 or 7 days")]
        public byte NotificationFrequency { get; set; }

        public string? PropertyType { get; set; }

        [Required]
        public string? Location { get; set; }

        [Range(0, 40)]
        public float SearchRadius { get; set; }

        [Range(0, 20000000)]
        public uint MinPrice { get; set; }

        [Range(0, 20000000)]
        public uint MaxPrice { get; set;}

        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10", ErrorMessage = "Valid values are: ' ' and 0-10")]
        public string? MinBeds { get; set; }

        [RegularExpression("^$|0|1|2|3|4|5|6|7|8|9|10", ErrorMessage = "Valid values are: ' ' and 0-10")]
        public string? MaxBeds { get; set;}

        [Required]
        public string? SearchLink { get; set; }
    }
}
