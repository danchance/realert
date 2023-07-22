const notificationTypeInput = document.querySelector('#notification-type');
const emailContainer = document.querySelector('#email-container');
const phoneContainer = document.querySelector('#phone-container');

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
    console.log(notificationTypeInput.value);
    console.log(notificationType['phone']);
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