namespace Realert.Models.ViewModels
{
    /// <summary>
    /// View Model <see cref="NewPropertyAlertSuccessViewModel"/> used to display details of
    /// the New Property Alert when a successful action is performed (e.g. Added or Deleted).
    /// </summary>
    public class NewPropertyAlertSuccessViewModel
    {
        /// <value>
        /// The name the user gave the notification.
        /// </value>
        public string? NotificationName { get; set; }

        /// <value>
        /// Email address notifications are sent to.
        /// </value>
        public string? Email { get; set; }
    }
}
