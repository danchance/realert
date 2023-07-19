﻿using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class NewPropertyAlertSetupViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [StringLength(11)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please give this notification a name")]
        [StringLength(100)]
        public string? NotificationName { get; set; }

        [Required]
        public string? NotificationFrequency { get; set; }

        [Required]
        public string? PropertyType { get; set; }

        [Required]
        public string? Location { get; set; }

        [Range(0, 40)]
        public float SearchRadius { get; set; }

        [Range(0, 20000000)]
        public uint MinPrice { get; set; }

        [Range(0, 20000000)]
        public uint MaxPrice { get; set;}

        [Required]
        [RegularExpression("^$|0|1|2|3|4|5", ErrorMessage = "Valid values are: ' ', 0, 1, 2, 3, 4, 5 ")]
        public string? MinBeds { get; set; }

        [Required]
        [RegularExpression("^$|0|1|2|3|4|5", ErrorMessage = "Valid values are: ' ', 0, 1, 2, 3, 4, 5 ")]
        public string? MaxBeds { get; set;}
    }
}
