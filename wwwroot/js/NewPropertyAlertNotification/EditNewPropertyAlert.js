/**
 * Hidden DeleteCode input.
 */
const deleteCodeInput = document.querySelector('#delete-code');

/**
 * New Property Alert Notifications can only be edited or deleted if the 'DeleteCode' 
 * value is known, this value is contained as a parameter in links sent via 
 * email/text. 
 * If this parameter exists, set the value of the hidden input that is sent
 * to the backend on POST requests for validation.
 * If the parameter does not exist, do nothing as the user will not be able
 * to edit/delete the notification.
 */
function getDeleteCode() {
    const urlParams = new URLSearchParams(window.location.search);
    deleteCode = urlParams.get('code');
    if (deleteCode != null) {
        deleteCodeInput.value = deleteCode;
    }
}

/**
 * Add a page load event listener to get the code query parameter from the URL
 * (if it exists), and set the hidden form input.
 */
addEventListener('load', () => getDeleteCode());