using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// View Model <see cref="NewPropertyAlertSetupViewModel"/> used to add a Price Alert.
    /// </summary>
    public class PriceAlertSetupViewModel
    {
        /// <value>
        /// Users preferred name to use when contacting them.
        /// </value>
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100)]
        public string? Name { get; set; }

        /// <value>
        /// Email address, populated if NotificationType = Email.
        /// </value>
        [RequiredWhenNotificationType(Notification.Email)]
        [StringLength(100)]
        public string? Email { get; set; }

        /// <value>
        /// Phone number, populated if NotificationType = Text.
        /// </value>
        [RequiredWhenNotificationType(Notification.Text)]
        [StringLength(11)]
        public string? PhoneNumber { get; set; }

        /// <value>
        /// The minimum amount the price must change before a notification is sent.
        /// </value>
        [Range(1000, 10000)]
        public int PriceThreshold { get; set; }

        /// <value>
        /// Link to the property listing the alert is for.
        /// </value>
        [Required(ErrorMessage = "Please enter the link to the propery listing.")]
        [StringLength(1000)]
        public string? ListingLink { get; set; }

        /// <value>
        /// Determines if the user should be notified if the property price increases.
        /// </value>
        public bool NotifyOnPriceIncrease { get; set; }

        /// <value>
        /// Determines if the user should be notified when the property is delisted.
        /// </value>
        public bool NotifyOnPropertyDelist { get; set; }

        /// <value>
        /// Type of notification to send, either email or text.
        /// </value>
        public Notification NotificationType { get; set; }

        /// <value>
        /// User note containing any details about the notification.
        /// </value>
        [StringLength(10000)]
        public string? Note { get; set; }
    }

    /// <summary>
    /// Custom required validation attribute for contact information fields: PhoneNumber and Email.
    /// - If 'Email' is selected as the notification type the Email address will be required.
    /// - If 'Text' is selected as the notification type the PhoneNumber field will be reqiured.
    /// </summary>
    public class RequiredWhenNotificationTypeAttribute : ValidationAttribute
    {
        private readonly Notification notificationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredWhenNotificationTypeAttribute"/> class.
        /// Sets the contact information (email or phone number) to validate.
        /// </summary>
        /// <param name="notification">Type of notification to validate.</param>
        public RequiredWhenNotificationTypeAttribute(Notification notification)
        {
            this.notificationType = notification;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var priceAlert = (PriceAlertSetupViewModel)validationContext.ObjectInstance;

            // Validation for the PhoneNumber field.
            if (this.notificationType == Notification.Text)
            {
                if (priceAlert.NotificationType != Notification.Text)
                {
                    return ValidationResult.Success;
                }

                var phoneStr = value as string;
                return string.IsNullOrWhiteSpace(phoneStr) ? new ValidationResult("Please enter your phone number.") : ValidationResult.Success;
            }

            // Validation for the Email field.
            if (this.notificationType == Notification.Email)
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
}
