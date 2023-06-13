using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class LocationScriptableObjectScript : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject _location;
    [SerializeField] private PolarCoordinates _polarCoordinates;
    [SerializeField] private ReverseGeocoding _reverseGeocoding;

    /* To update the menu */
    [SerializeField] private CoordinatesText _coordinatesText;
    [SerializeField] private CountryFlag _countryFlag;
    [SerializeField] private TextMeshProUGUI _menuLocationName;
    private string _menuLocationNameDefault;

    void Start()
    {
        ResetLocationData();
        _menuLocationNameDefault = _menuLocationName.text;
    }

    public void UpdateLocation(Vector3 raycastPoint)
    {
        ResetLocationData();
        UpdateLocationCoordinates(raycastPoint);
        UpdateLocationNameAndCountryCode();
    }

    public void UpdateLocation(OmgLocation geolocation)
    {
        _location.LocationName = geolocation.displayName;
        _menuLocationName.text = _location.LocationName;
        UpdateLocationCoordinates(geolocation.latitude, geolocation.longitude);
        UpdateCountryCode(geolocation.countryCode);
    }

    private void ResetLocationData()
    {
        _location.LocationName = null;
        _location.Latitude = 0f;
        _location.Longitude = 0f;
        _location.CountryCode = null;
    }

    private void UpdateLocationCoordinates(Vector3 raycastPoint)
    {
        Vector2 coord = _polarCoordinates.CalculateCoordinates(raycastPoint);
        _location.Latitude = coord.y;
        _location.Longitude = coord.x;
        _coordinatesText.CoordinatesTextUpdate(_location.Latitude, _location.Longitude);
    }

    private void UpdateLocationCoordinates(float latitude, float longitude)
    {
        _location.Latitude = latitude;
        _location.Longitude = longitude;
        _coordinatesText.CoordinatesTextUpdate(latitude, longitude);
    }

    private void UpdateLocationNameAndCountryCode()
    {
        StartCoroutine(_reverseGeocoding.FetchNominatimData(_location.Latitude, _location.Longitude, (nominatim) =>
        {
            if (nominatim == null)
            {
                _countryFlag.ResetFlag();
                _menuLocationName.text = _menuLocationNameDefault;
                return;
            }
            else if (nominatim.osmType == "node" && nominatim.osmId == "3815077900")
            {
                _location.LocationName = "Null Island";
                return;
            }

            UpdateCountryCode(nominatim.address.countryCode);
            UpdateLocationName(nominatim);
        }));
    }

    private void UpdateCountryCode(string countryCode)
    {
        _location.CountryCode = countryCode;
        _countryFlag.UpdateFlag(_location.CountryCode);
    }

    private void UpdateLocationName(Nominatim nominatim)
    {
        string countryName = nominatim.address.country;
        _location.LocationName = _reverseGeocoding.GetLocationNameFromNominatimData(nominatim.address.otherData);

        // If no location name has been found in the Nominatim API, or there is at least one non-latin character in the string
        if (_location.LocationName == null || !Regex.IsMatch(_location.LocationName, "[a-z]", RegexOptions.IgnoreCase))
        {
            StartCoroutine(_reverseGeocoding.FetchNameTranslationFromOSM(nominatim.osmType, nominatim.osmId, (translation) =>
            {
                if (_location.LocationName == null && translation == null)
                    return;
                else if (translation != null)
                    _location.LocationName = translation;

                // The location name is either non-latin or a translation, so you can add the country name
                if (countryName != null)
                    SetLocationName(countryName);
            }));
        }
        else if (countryName != null)
            SetLocationName(countryName);
    }

    private void SetLocationName(string countryName)
    {
        _location.LocationName += ", " + countryName;
        _menuLocationName.text = _location.LocationName;
    }
}
