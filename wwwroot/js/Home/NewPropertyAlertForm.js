/**
 * Search form fields
 */
const propertyLinkInput = document.querySelector('#property-link');
const propertyLinkErrorText = document.querySelector('#property-link-error');

/**
 * New Property Alert form fields.
 */
const newPropertyAlertForm = document.querySelector('#new-property-alert-form');
const targetSiteText = document.querySelector('#target-site-text');
const targetSiteInput = document.querySelector('#target-site');
const propertyTypeInput = document.querySelector('#property-type');
const locationInput = document.querySelector('#location');
const locationText = document.querySelector('#location-message');
const searchRadiusInput = document.querySelector('#search-radius');
const minPriceInput = document.querySelector('#min-price');
const maxPriceInput = document.querySelector('#max-price');
const minBedsInput = document.querySelector('#min-beds');
const maxBedsInput = document.querySelector('#max-beds');

/**
 * Supported property site hostnames and internal Ids.
 */
const RIGHTMOVE_HOSTNAME = 'www.rightmove.co.uk';
const PURPLEBRICKS_HOSTNAME = 'www.purplebricks.co.uk';
const RIGHTMOVE_ID = 0;
const PURPLEBRICKS_ID = 1;


/**
 * Property search settings. Used to populate default values into the form
 * New Property Alert fields.
 */
const searchSettings = {
    targetSiteText: '',
    targetSite: '',
    propertyType: '',
    location: '',
    locationMessage: '',
    searchRadius: '0',
    minPrice: '0',
    maxPrice: '0',
    minBeds: '',
    maxBeds: '',
}

/**
 * This function initializes the values of the searchSettings to their
 * defaults.
 */
function initializeSearchSettings() {
    searchSettings.targetSiteText = '';
    searchSettings.targetSite = '';
    searchSettings.propertyType = '';
    searchSettings.location = '';
    searchSettings.locationMessage = '';
    searchSettings.searchRadius = '0';
    searchSettings.minPrice = '0';
    searchSettings.maxPrice = '0';
    searchSettings.minBeds = '';
    searchSettings.maxBeds = '';
}

/**
 * Form handler for the Property URL form.
 * Parses the URL to determine the website and property search settings.
 */
function handleSearchLinkForm(e) {
    e.preventDefault();

    // Clear previous errors.
    propertyLinkErrorText.textContent = '';

    // If nothing entered, keep the main property alert form hidden.
    if (propertyLinkInput.value === '') {
        newPropertyAlertForm.classList.add('hidden');
        return;
    }

    // Attempt to parse the URL. Set an error message if the URL is invalid.
    try {
        const url = new URL(propertyLinkInput.value);

        // Parse the URL parameters depending on the property site entered. Give an 
        // error if the site is not currently suppported.
        switch (url.hostname) {
            case RIGHTMOVE_HOSTNAME:
                parseRightmoveURL(url);
                break;
            case PURPLEBRICKS_HOSTNAME:
                parsePurplebricksURL(url);
                break;
            default:
                newPropertyAlertForm.classList.add('hidden');
                propertyLinkErrorText.textContent = 'This site is not currently supported. Please enter a Rightmove or Purplebricks link.';
                return;
        }
    } catch (error) {
        newPropertyAlertForm.classList.add('hidden');
        propertyLinkErrorText.textContent = 'Please enter a valid URL. Supported sites are: Rightmove and Purplebricks.';
        return;
    }

    

    // Default in values parsed from the URL.
    targetSiteText.textContent = searchSettings.targetSiteText;
    targetSiteInput.value = searchSettings.targetSite;
    propertyTypeInput.value = searchSettings.propertyType;
    locationInput.value = searchSettings.location;
    locationText.textContent = searchSettings.locationMessage;
    searchRadiusInput.value = searchSettings.searchRadius;
    minPriceInput.value = searchSettings.minPrice;
    maxPriceInput.value = searchSettings.maxPrice;
    minBedsInput.value = searchSettings.minBeds;
    maxBedsInput.value = searchSettings.maxBeds;

    // Display the notification setup form.
    newPropertyAlertForm.classList.remove("hidden");
}

/**
 * This function parses a Rightmove URL, extracting values from the following
 * query parameters: property type, location, search radius, min price, max price,
 * min beds and max beds values.
 * If the query parameter does not exist then corresponding value in searchSettings
 * will be left as the default.
 * Throws error if there is no locationIdentifier parameter.
 */
function parseRightmoveURL(url) {
    initializeSearchSettings();

    // Location field is formated: 'OUTCODE^XXXX'. Get all characters after the '^'.
    // If locationIdentifier field does not exist or is formatted incorrectly, throw an
    // error as Location cannot be entered later by the user.
    if (url.searchParams.get('locationIdentifier') != null) {
        searchSettings.location = url.searchParams.get('locationIdentifier').split('^')[1];
        if (searchSettings.location == null) {
            throw new Error("locationIdentifer invalid");
        }
    } else {
        throw new Error("locationIdentifier missing");
    }
    if (url.searchParams.get('radius') != null) {
        searchSettings.searchRadius = url.searchParams.get('radius');
    }
    if (url.searchParams.get('displayPropertyType') != null) {
        searchSettings.propertyType = url.searchParams.get('displayPropertyType');
    }
    if (url.searchParams.get('minPrice') != null) {
        searchSettings.minPrice = url.searchParams.get('minPrice');
    }
    if (url.searchParams.get('maxPrice') != null) {
        searchSettings.maxPrice = url.searchParams.get('maxPrice');
    }
    if (url.searchParams.get('minBedrooms') != null) {
        searchSettings.minBeds = url.searchParams.get('minBedrooms');
    }
    if (url.searchParams.get('maxBedrooms') != null) {
        searchSettings.maxBeds = url.searchParams.get('maxBedrooms');
    }
    searchSettings.targetSiteText = 'Rightmove';
    searchSettings.targetSite = RIGHTMOVE_ID;

    // Rightmove uses 4 digit codes in the location parameter. Display a message
    // explaining this.
    searchSettings.locationMessage = 'Rightmove uses unique codes for locations, to change the location, copy in a new URL.';
}

/**
 * This function parses a Purplebricks URL, extracting values from the following
 * query parameters: property type, location, search radius, min price, max price,
 * min beds and max beds values.
 * If the query parameter does not exist then corresponding value in searchSettings
 * will be left as the default.
 */
function parsePurplebricksURL(url) {
    initializeSearchSettings();

    // If location field does not exist, throw an error as Location cannot be
    // entered later by the user.
    if (url.searchParams.get('location') != null) {
        searchSettings.location = url.searchParams.get('location');
    } else {
        throw new Error('location missing');
    }
    if (url.searchParams.get('searchRadius') != null) {
        searchSettings.searchRadius = url.searchParams.get('searchRadius');
    }

    // Property type is numeric values 1-6, need to map these to the values
    // of the dropdown list.
    const propertyType = url.searchParams.get('type');
    if (propertyType != null) {
        let typeMapping = {
            1: 'houses',
            2: 'bungalows',
            3: 'flats',
            4: 'land',
            5: 'commercial',
            6: 'other'
        }
        if (typeMapping[propertyType] != null) {
            searchSettings.propertyType = typeMapping[propertyType];
        }
    }
    if (url.searchParams.get('priceFrom') != null) {
        searchSettings.minPrice = url.searchParams.get('priceFrom');
    }
    if (url.searchParams.get('priceTo') != null) {
        searchSettings.maxPrice = url.searchParams.get('priceTo');
    }
    if (url.searchParams.get('bedroomsFrom') != null) {
        searchSettings.minBeds = url.searchParams.get('bedroomsFrom');
    }
    if (url.searchParams.get('bedroomsTo')) {
        searchSettings.maxBeds = url.searchParams.get('bedroomsTo');
    }
    searchSettings.targetSiteText = 'Purplebricks';
    searchSettings.targetSite = PURPLEBRICKS_ID;
}