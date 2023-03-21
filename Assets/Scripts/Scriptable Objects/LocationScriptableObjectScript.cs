using System.Text.RegularExpressions;
using UnityEngine;

public class LocationScriptableObjectScript : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;
    [SerializeField] private PolarCoordinates polarCoordinates;
    [SerializeField] private ReverseGeocoding reverseGeocoding;

    /* To update the menu */
    [SerializeField] private CoordinatesText coordinatesText;
    [SerializeField] private CountryFlag countryFlag;

    public void UpdateLocation(Vector3 pinPosition)
    {
        ResetLocationData();
        UpdateLocationCoordinates(pinPosition);
        UpdateLocationNameAndCountryCode();
    }

    private void ResetLocationData()
    {
        location.locationName = null;
        location.latitude = 0f;
        location.longitude = 0f;
        location.countryCode = null;
    }

    private void UpdateLocationCoordinates(Vector3 pinPosition)
    {
        Vector2 coord = polarCoordinates.CalculateCoordinates(pinPosition);
        location.latitude = coord.y;
        location.longitude = coord.x;
        coordinatesText.CoordinatesTextUpdate(location.latitude, location.longitude);
    }

    private void UpdateLocationNameAndCountryCode()
    {
        StartCoroutine(reverseGeocoding.FetchNominatimData(location.latitude, location.longitude, (nominatim) =>
        {
            if (nominatim == null)
            {
                countryFlag.ResetFlag();
                return;
            }
            else if (nominatim.osmType == "node" && nominatim.osmId == "3815077900")
            {
                location.locationName = "Null Island";
                return;
            }

            UpdateCountryCode(nominatim.address.countryCode);
            UpdateLocationName(nominatim);
            
        }));
    }

    private void UpdateCountryCode(string countryCode)
    {
        location.countryCode = countryCode;
        countryFlag.UpdateFlag(location.countryCode);
    }

    private void UpdateLocationName(Nominatim nominatim)
    {
        string countryName = nominatim.address.country;
        location.locationName = reverseGeocoding.GetLocationNameFromNominatimData(nominatim.address.otherData);

        // If no location name has been found in the Nominatim API, or there is at least one non-latin character in the string
        if (location.locationName == null || !Regex.IsMatch(location.locationName, "[a-z]", RegexOptions.IgnoreCase))
        {
            StartCoroutine(reverseGeocoding.FetchNameTranslationFromOSM(nominatim.osmType, nominatim.osmId, (translation) =>
            {
                if (location.locationName == null && translation == null)
                    return;
                else if (translation != null)
                    location.locationName = translation;

                // The location name is either non-latin or a translation, so you can add the country name
                if (countryName != null)
                    location.locationName += ", " + countryName;
            }));
        }
        else if (countryName != null)
            location.locationName += ", " + countryName;
    }
}
