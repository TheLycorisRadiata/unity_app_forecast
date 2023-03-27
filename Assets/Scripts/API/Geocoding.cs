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
    private int userChoice;

    private List<OmgLocation> locationList;
    [SerializeField] private TextMeshProUGUI listCount;
    [SerializeField] private Transform locationListContent;
    [SerializeField] private GameObject locationListPrefab;

    /*
        TODO:

        - Cliquer sur un lieu --> Mettre le scriptable object à jour.
        - Bloquer l'épinglage et le déplacement avec l'input du clavier.
        - Cliquer sur un lieu --> Epingler le lieu sur le globe, avec les coordonnées etc qui sont mises à jour dans le menu.

        ---

        - Bouton pour valider : Une loupe.
        - Bouton pour afficher/cacher la liste : Un oeil / Un oeil barré.
    */

    /* OnClick event in the menu */
    public void FetchLocationList()
    {
        if (userInput.text == "")
            return;

        EmptyList();

        // TODO: The operation can take some time, so implement a loading spinner

        StartCoroutine(FetchOpenMeteoGeocodingData((locations) =>
        {
            if (locations == null)
                return;

            StartCoroutine(FetchNominatimDisplayNames(locations, (locations) => PopulateMenu(locations)));
        }));
    }

    private void EmptyList()
    {
        int i;
        locationList = new List<OmgLocation>();
        listCount.text = "0 lieu";
        for (i = 0; i < locationListContent.childCount; ++i)
            Destroy(locationListContent.GetChild(i).gameObject);
    }

    private void PopulateMenu(List<OmgLocation> locations)
    {
        int i;
        locationList = locations;
        listCount.text = locationList.Count < 2 ? $"{locationList.Count} lieu" : $"{locationList.Count} lieux";
        for (i = 0; i < locationList.Count; ++i)
        {
            Transform t = Instantiate(locationListPrefab, locationListContent.position, Quaternion.identity).transform;
            t.SetParent(locationListContent);
            t.localScale = new Vector3(1f, 1f, 1f);
            t.GetChild(0).GetComponent<TextMeshProUGUI>().text = locationList[i].displayName;
        }
    }

    private void PickLocation()
    {
        // TODO: Allow the user to pick a location
        userChoice = 0;

        // ONCE THE USER PICKS ONE OF THE DISPLAYED LOCATIONS:
        location.locationName = locationList[userChoice].displayName;
        location.latitude = locationList[userChoice].latitude;
        location.longitude = locationList[userChoice].longitude;
        location.countryCode = locationList[userChoice].countryCode;
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
