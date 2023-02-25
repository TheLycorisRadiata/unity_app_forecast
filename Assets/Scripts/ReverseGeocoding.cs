using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class RootObject
{
    public string error;
    public Address address;
}

[Serializable]
public class Address
{
    // Different properties for the location name
    public string city;
    public string municipality;
    public string village;
    public string county;
    public string province;
    public string country;
    public string man_made;

    // Country code (e.g. France is "fr")
    public string country_code;
}

public class ReverseGeocoding : MonoBehaviour
{
    [SerializeField] private Location location;

    public void UpdateLocationNameAndCountryCode()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        // Depending on the user's language, the dot in the float will become a comma when inserted inside a string.
        string latitude = StringFormat.Float(location.locationLat);
        string longitude = StringFormat.Float(location.locationLon);
        string uri = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&accept-language=en&format=json";

        string jsonText;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Reverse geocoding error: The request failed to fetch from the API.");
                ResetLocationNameAndCountryCode();
                yield break;
            }

            jsonText = webRequest.downloadHandler.text;
            RootObject ro = JsonUtility.FromJson<RootObject>(jsonText);

            if (ro.error != null)
            {
                Debug.LogWarning($"Reverse geocoding error: The request went through but the API couldn't find a location from these coordinates ({location.locationLat},{location.locationLon}).");
                ResetLocationNameAndCountryCode();
                yield break;
            }

            Address address = ro.address;
            location.locationName = address.city ?? address.municipality ?? address.village ?? address.county ?? address.province ?? address.country ?? address.man_made;
            location.locationCountryCode = address.country_code;
        }
    }

    private void ResetLocationNameAndCountryCode()
    {
        location.locationName = null;
        location.locationCountryCode = null;
    }
}
