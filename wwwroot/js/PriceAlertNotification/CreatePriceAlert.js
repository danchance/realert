/**
 * New Price Alert form fields.
 */
const notificationTypeInput = document.querySelector('#notification-type');
const emailContainer = document.querySelector('#email-container');
const phoneContainer = document.querySelector('#phone-container');

/**
 * Define notifcation type values.
 * Note: These values must match the enum values in the model.
 */
const notificationType = {
    email: '0',
    phone: '1',
};

/**
 * Function is called when the notification type dropdown list value is changed,
 * and is used to toggle between the phone number input and the email address input.
 */
const notificationTypeChanged = () => {
    // Text notification type selected.
    if (notificationTypeInput.value === notificationType["phone"]) {
        phoneContainer.classList.remove('hidden');
        emailContainer.classList.add('hidden');
        return;
    }
    // Email notification type selected.
    phoneContainer.classList.add('hidden');
    emailContainer.classList.remove('hidden');
}

// Set the correct contact details input (email or phone) when the page is loaded.
notificationTypeChanged();

/**
* Add a page load event listener.
* Display the correct contact details input field (email or phone) when the page
* is loaded.
*/
addEventListener('load', () => notificationTypeChanged());