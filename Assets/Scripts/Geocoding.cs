using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

public class Geocoding : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;

    // For debug purposes - To delete later
    [SerializeField] private string userInput;
    [SerializeField] private bool isButtonClicked = false;

    void Update()
    {
        if (isButtonClicked && userInput != "")
        {
            isButtonClicked = false;
            FetchLocationList();
        }
    }
    // -----------------------------------

    public void FetchLocationList()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        string encodedInput = HttpUtility.UrlEncode(userInput);
        string uri = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedInput}";
        string jsonText;
        string latitude, longitude;
        OpenMeteoGeocoding geocoding;
        Nominatim nominatim;
        List<OmgLocation> locations;
        int i;

        // Fetch the OpenMeteo geocoding location data
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Geocoding error: The request failed to fetch from the API.");
                yield break;
            }

            jsonText = webRequest.downloadHandler.text;
            geocoding = JsonUtility.FromJson<OpenMeteoGeocoding>(jsonText);

            if (geocoding.results == null)
            {
                Debug.LogWarning($"Geocoding error: The request went through but the API couldn't find any location of this name ({userInput}).");
                yield break;
            }

            locations = geocoding.results;
        }

        // Fetch the Nominatim display_name for each location
        // TODO: The operation can take some time, so implement a loading spinner to make the user be patient
        for (i = 0; i < locations.Count; ++i)
        {
            latitude = StringFormat.Float(locations[i].latitude);
            longitude = StringFormat.Float(locations[i].longitude);
            uri = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&accept-language=fr,en&format=json";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    locations[i] = null;
                    continue;
                }

                jsonText = webRequest.downloadHandler.text;
                nominatim = JsonUtility.FromJson<Nominatim>(jsonText);

                if (nominatim.error != null)
                {
                    locations[i] = null;
                    continue;
                }

                // OpenMeteo may send locations which do not match with the user input, therefore check if display_name contains it
                // RemoveDiacritics() is to remove accents (only for the check)
                if (RemoveDiacritics(nominatim.display_name).Contains(RemoveDiacritics(userInput), StringComparison.OrdinalIgnoreCase))
                {
                    // display_name is null by default, this will give it a value
                    locations[i].display_name = nominatim.display_name;

                    // Since we're already in a loop, seize the opportunity to perform this operation:
                    // Limit the coordinates up to 2 decimals after the floating point
                    locations[i].latitude = (float)Math.Round(locations[i].latitude, 2);
                    locations[i].longitude = (float)Math.Round(locations[i].longitude, 2);
                }
            }
        }

        // Remove incorrect locations
        locations.RemoveAll(element => element.display_name == null);

        // TODO: Display the locations in the GUI
        // TODO: Allow the user to pick one

        /*
            ONCE THE USER PICKS ONE OF THE DISPLAYED LOCATIONS:

            location.locationName = locations[userChoice].display_name;
            location.locationLat = locations[userChoice].latitude;
            location.locationLon = locations[userChoice].longitude;
            location.locationCountryCode = locations[userChoice].country_code;
        */
    }

    // \p{Mn} or \p{Non_Spacing_Mark}: a character intended to be combined with another character without taking up extra space (e.g. accents, umlauts, etc)
    private readonly static Regex nonSpacingMarkRegex = new Regex(@"\p{Mn}", RegexOptions.Compiled);
    static string RemoveDiacritics(string text)
    {
        if (text == null)
            return string.Empty;

        string normalizedText = text.Normalize(NormalizationForm.FormD);
        return nonSpacingMarkRegex.Replace(normalizedText, string.Empty);
    }
}
