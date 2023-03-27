using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Geocoding : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;
    [SerializeField] private TMP_InputField userInput;

    // TODO: Delete when not needed anymore
    [SerializeField] private int userChoice;

    /*
        TODO:

        - Valider --> Afficher la liste des lieux, scrollable.
        - Cliquer sur un lieu --> Epingler le lieu sur le globe, avec les coordonnées etc qui sont mises à jour dans le menu.
        - Bloquer l'épinglage et le déplacement avec l'input du clavier.

        ---

        - Bouton pour valider : Une loupe.
        - Bouton pour afficher/cacher la liste : Un oeil / Un oeil barré.
    */

    /* OnClick event in the menu */
    public void FetchLocationList()
    {
        if (userInput.text == "")
            return;

        // TODO: The operation can take some time, so implement a loading spinner to make the user be patient

        StartCoroutine(FetchOpenMeteoGeocodingData((locations) =>
        {
            if (locations == null)
                return;

            StartCoroutine(FetchNominatimDisplayNames(locations, (locations) =>
            {
                // TODO: Display the locations in the GUI
                // TODO: Allow the user to pick one
                userChoice = 0;

                locations.ForEach(e => Debug.Log(e.displayName));
                Debug.Log("---------------");

                // ONCE THE USER PICKS ONE OF THE DISPLAYED LOCATIONS:
                location.locationName = locations[userChoice].displayName;
                location.latitude = locations[userChoice].latitude;
                location.longitude = locations[userChoice].longitude;
                location.countryCode = locations[userChoice].countryCode;
            }));
        }));
    }

    private IEnumerator FetchOpenMeteoGeocodingData(Action<List<OmgLocation>> callback)
    {
        string encodedInput = HttpUtility.UrlEncode(userInput.text);
        string uri = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedInput}";
        string jsonText;
        OpenMeteoGeocoding geocoding;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Geocoding error: The request failed to fetch from the API.");
                yield break;
            }

            jsonText = webRequest.downloadHandler.text;
            geocoding = new OpenMeteoGeocoding(jsonText);

            if (geocoding.results == null)
            {
                Debug.LogWarning($"Geocoding error: The request went through but the API couldn't find any location of this name ({userInput}).");
                yield break;
            }

            callback(geocoding.results);
        }
    }

    private IEnumerator FetchNominatimDisplayNames(List<OmgLocation> locations, Action<List<OmgLocation>> callback)
    {
        string latitude, longitude;
        string uri, jsonText;
        Nominatim nominatim;
        int i;

        /* Fetch the Nominatim "display name" for each location */
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
                nominatim = new Nominatim(jsonText);

                if (nominatim.error != null)
                {
                    locations[i] = null;
                    continue;
                }

                /*
                    OpenMeteo may send locations which do not match with the user input, therefore check if displayName contains the input.
                    RemoveDiacritics() is to remove accents (only for the check).
                */
                if (StringFormat.RemoveDiacritics(nominatim.displayName).Contains(StringFormat.RemoveDiacritics(userInput.text), StringComparison.OrdinalIgnoreCase))
                {
                    /* displayName is null by default, this will give it a value */
                    locations[i].displayName = nominatim.displayName;

                    /*
                        Since we're already in a loop, seize the opportunity to perform this operation:
                        Limit the coordinates up to 2 decimals after the floating point.
                    */
                    locations[i].latitude = (float)Math.Round(locations[i].latitude, 2);
                    locations[i].longitude = (float)Math.Round(locations[i].longitude, 2);
                }
            }
        }

        /* Remove incorrect locations */
        locations.RemoveAll(element => element?.displayName == null);

        callback(locations);
    }
}
