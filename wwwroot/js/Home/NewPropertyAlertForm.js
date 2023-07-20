﻿/**
 * Search form fields
 */
const propertyLinkInput = document.querySelector('#property-link');
const propertyLinkErrorText = document.querySelector('#property-link-error');

/**
 * New Property Alert form fields.
 */
const newPropertyAlertForm = document.querySelector('#new-property-alert-form');
const targetSiteText = document.querySelector('#target-site');
const propertyTypeInput = document.querySelector('#property-type');
const locationInput = document.querySelector('#location');
const locationText = document.querySelector('#location-message');
const searchRadiusInput = document.querySelector('#search-radius');
const minPriceInput = document.querySelector('#min-price');
const maxPriceInput = document.querySelector('#max-price');
const minBedsInput = document.querySelector('#min-beds');
const maxBedsInput = document.querySelector('#max-beds');

/**
 * Supported property site hostnames.
 */
const RIGHTMOVE_HOSTNAME = 'www.rightmove.co.uk';
const PURPLEBRICKS_HOSTNAME = 'www.purplebricks.co.uk';

/**
 * Property search settings. Used to populate default values into the form
 * New Property Alert fields.
 */
const searchSettings = {
    targetSite: '',
    propertyType: '',
    location: '',
    locationMessage: '',
    searchRadius: '',
    minPrice: '',
    maxPrice: '',
    minBeds: '',
    maxBeds: '',
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
    let url = "";
    try {
        url = new URL(propertyLinkInput.value);
    } catch (error) {
        newPropertyAlertForm.classList.add('hidden');
        propertyLinkErrorText.textContent = 'Please enter a valid URL. Supported sites are: Rightmove and Purplebricks.';
        return;
    }

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

    // Default in values parsed from the URL.
    targetSiteText.textContent = searchSettings.targetSite;
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
 * This function parses a Rightmove URL, extracting the property type, location,
 * search radius, min price, max price, min beds and max beds values.
 */
function parseRightmoveURL(url) {
    // Location field is formated: 'OUTCODE^XXXX'. Get all characters after the
    // '^'.
    searchSettings.location = url.searchParams.get('locationIdentifier').split('^')[1];
    searchSettings.searchRadius = url.searchParams.get('radius');
    searchSettings.propertyType = url.searchParams.get('displayPropertyType');
    searchSettings.minPrice = url.searchParams.get('minPrice');
    searchSettings.maxPrice = url.searchParams.get('maxPrice');
    searchSettings.minBeds = url.searchParams.get('minBedrooms');
    searchSettings.maxBeds = url.searchParams.get('maxBedrooms');
    searchSettings.targetSite = 'Rightmove';

    // Rightmove uses 4 digit codes in the location parameter. Display a message
    // explaining this.
    searchSettings.locationMessage = 'Rightmove uses unique codes for locations, to change the location, copy in a new URL.';
}

/**
 * TODO: Add support for Purplebricks.
 */
function parsePurplebricksURL(url) {

}