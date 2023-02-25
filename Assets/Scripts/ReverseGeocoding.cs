using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

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
        string uri = $"https://nominatim.openstreetmap.org/reverse?lat={location.locationLat}&lon={location.locationLon}&accept-language=en&format=json";
        string jsonText;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Reverse geocoding error: The request failed to fetch from the API.");
                Debug.LogWarning(webRequest);
                yield break;
            }

            jsonText = webRequest.downloadHandler.text;
            RootObject ro = JsonUtility.FromJson<RootObject>(jsonText);

            if (ro.error != null)
            {
                Debug.LogWarning($"Reverse geocoding error: The request went through but the API couldn't find a location from these coordinates ({location.locationLat},{location.locationLon}).");
                yield break;
            }

            Address address = ro.address;
            location.locationName = address.city ?? address.municipality ?? address.village ?? address.county ?? address.province ?? address.country ?? address.man_made;
            location.locationCountryCode = address.country_code;
        }
    }
}
