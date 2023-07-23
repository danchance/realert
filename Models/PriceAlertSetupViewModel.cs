using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /*
     * Custom required validation attribute for contact information fields:
     * PhoneNumber and Email.
     *  - If the user selects 'Email' as the notification type then the Email 
     *    address will be required.
     *  - If they select 'Text' the PhoneNumber field will be reqiured.
     */
    public class RequiredWhenNotificationTypeAttribute : ValidationAttribute
    {
        // Set the contact information field we are validating.
        public RequiredWhenNotificationTypeAttribute(Notification notification) {
            _notificationType = notification;
        }

        private readonly Notification _notificationType;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var priceAlert = (PriceAlertSetupViewModel)validationContext.ObjectInstance;

            // Validation for the PhoneNumber field.
            if (_notificationType == Notification.Text)
            {
                if (priceAlert.NotificationType != Notification.Text)
                {
                    return ValidationResult.Success;
                }
                var phoneStr = value as string;
                return string.IsNullOrWhiteSpace(phoneStr) ? new ValidationResult("Please enter your phone number.") : ValidationResult.Success;
            }

            // Validation for the Email field.
            if (_notificationType == Notification.Email)
            {
                if (priceAlert.NotificationType != Notification.Email)
                {
                    return ValidationResult.Success;
                }
                var emailStr = value as string;
                return string.IsNullOrWhiteSpace(emailStr) ? new ValidationResult("Please enter your email address.") : ValidationResult.Success;
            }

            // Invalid field (should not get here!).
            return new ValidationResult("Invalid notification type");
        }
    }

    /*
     * View model for the form used for setting up new property price alerts.
     */
    public class PriceAlertSetupViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100)]
        public string? Name { get; set; }

        [RequiredWhenNotificationType(Notification.Email)]
        [StringLength(100)]
        public string? Email { get; set; }

        [RequiredWhenNotificationType(Notification.Text)]
        [StringLength(11)]
        public string? PhoneNumber { get; set; }

        [Range(1000, 10000)]
        public int PriceThreshold { get; set; }

        [Required (ErrorMessage = "Please enter the link to the propery listing.")]
        [StringLength(1000)]
        public string? ListingLink { get; set; }

        public bool NotifyOnPriceIncrease { get; set; }

        public bool NotifyOnPropertyDelist { get; set; }

        public Notification NotificationType { get; set; }

        [StringLength(10000)]
        public string? Note { get; set; }
    }
}
