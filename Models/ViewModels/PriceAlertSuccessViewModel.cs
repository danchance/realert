namespace Realert.Models.ViewModels
{
    /// <summary>
    /// View Model <see cref="PriceAlertSuccessViewModel"/> used to display details of
    /// the Price Alert when a successful action is performed (e.g. Added or Deleted).
    /// </summary>
    public class PriceAlertSuccessViewModel
    {
        /// <value>
        /// Either Email address or phone number depending on the contact method chosen
        /// when setting up the alert.
        /// </value>
        public string? ContactDetails { get; set; }

        /// <value>
        /// The property associated with the notification that has just been deleted.
        /// </value>
        public string? PropertyName { get; set; }
    }
}
