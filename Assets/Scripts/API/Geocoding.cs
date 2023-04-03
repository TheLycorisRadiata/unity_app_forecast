using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Geocoding : MonoBehaviour
{
    [SerializeField] private Raycast raycast;
    [SerializeField] private PolarCoordinates polarCoord;
    [SerializeField] private Transform earth;
    [SerializeField] private PinManager pinManager;

    [SerializeField] private LocationScriptableObject location;
    [SerializeField] private LocationScriptableObjectScript locationScript;
    [SerializeField] private TMP_InputField userInput;

    private List<OmgLocation> locationList;
    [SerializeField] private TextMeshProUGUI listCount;
    [SerializeField] private Transform locationListContent;
    [SerializeField] private GameObject locationListPrefab;

    /*
        TODO:
        - TODO: Make it possible to find Null Island from text.
        - TODO: Button to validate --> A magnifying glass.
        - TODO: Button to display/hide the list --> An eye / A slashed eye.
        - TODO: The operation can take some time, so implement a loading spinner.
    */

    /*
        METEO
        https://github.com/PolyMarsDev/Real-World-Weather/
        https://open-meteo.com/
        https://open-meteo.com/en/docs#latitude=43.72&longitude=7.30&hourly=temperature_2m
        https://api.open-meteo.com/v1/forecast?latitude=43.72&longitude=7.30&hourly=temperature_2m
    */

    /* OnClick event in the menu */
    public void FetchLocationList()
    {
        if (userInput.text == "")
            return;

        EmptyList();

        StartCoroutine(FetchOpenMeteoGeocodingData((locations) =>
        {
            if (locations == null)
            {
                listCount.text = "0 lieu";
                return;
            }

            StartCoroutine(FetchNominatimDisplayNames(locations, (locations) => PopulateMenu(locations)));
        }));
    }

    private IEnumerator FetchOpenMeteoGeocodingData(Action<List<OmgLocation>> callback)
    {
        string encodedInput = HttpUtility.UrlEncode(userInput.text);
        string uri = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedInput}&language=fr";
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
        string name;
        string[] arr;
        int firstIndex;

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

                name = nominatim.displayName;

                /* OpenMeteo may send locations which do not match with the user input, therefore check if displayName contains the input */
                if (StringFormat.WordContainsWord(name, userInput.text))
                {
                    /*
                        - [...], Paris, Île-de-France, France métropolitaine, 75004, France
                        - [...], Paris, Lemar County, Texas, 75460, Etats-Unis d'Amérique
                        - [...], Nice, Alpes-Maritimes, Provence-Alpes-Côte d'Azur, France métropolitaine, 06000, France
                        - 6e Soufrière, Commune d'Acul-du-Nord, Arrondissement d'Acul-du-Nord, Département du Nord, Haïti
                    */

                    arr = name.Split(", ");

                    /* Remove words before user input */
                    firstIndex = arr.ToList().FindIndex(element => StringFormat.WordComparison(element, userInput.text));
                    if (firstIndex < 0)
                        continue;
                    arr = Enumerable.Reverse(arr).Take(arr.Length - firstIndex).Reverse().ToArray();

                    /* Remove words with a number in them */
                    arr = arr.Where(element => !element.Any(char.IsDigit)).ToArray();
                    /* Remove words with a non-latin character in them */
                    arr = arr.Where(element => Regex.IsMatch(element, "[a-z]", RegexOptions.IgnoreCase)).ToArray();

                    name = string.Join(", ", arr);

                    /* Check if the input is still in the name, now that it has been reduced */
                    if (!StringFormat.WordContainsWord(name, userInput.text))
                        continue;
                    /* Check if the element doesn't already exist in the list */
                    else if (locations.Any(element => element.displayName == name))
                        continue;

                    /* displayName is null by default, this will give it a value */
                    locations[i].displayName = name;

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

        /* Sort in alphabetical order based on the country name, then the region, etc. */
        if (locations.Count > 1)
            locations = locations.OrderBy(e => e, new OmgLocationComparer()).ToList();

        callback(locations);
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
            t.name = i.ToString();
            t.localScale = new Vector3(1f, 1f, 1f);
            t.GetChild(0).GetComponent<TextMeshProUGUI>().text = locationList[i].displayName;
            t.GetComponent<Button>().onClick.AddListener(() => SelectLocation(int.Parse(t.name)));
        }
    }

    private void SelectLocation(int index)
    {
        StyleLocationElements(locationListContent.GetChild(index));
        locationScript.UpdateLocation(locationList[index]);
        GoToLocation(location.latitude, location.longitude);
    }

    private void StyleLocationElements(Transform selectedElement)
    {
        Color yellow = new Color(224f / 255f, 182f / 255f, 37f / 255f);
        Color blue = new Color(40f / 255f, 190f / 255f, 169f / 255f);
        int i;

        for (i = 0; i < locationListContent.childCount; ++i)
            locationListContent.GetChild(i).GetComponent<Image>().color = blue;
        selectedElement.GetComponent<Image>().color = yellow;
    }

    private void GoToLocation(float latitude, float longitude)
    {
        Tuple<Vector3, Quaternion> centerRaycast = null;
        Vector2 centerCoords;
        float centerLat, centerLon;
        float diffLat, diffLon;
        int counter = 0;

        do
        {
            /* For some reason several passes are needed. Add a limit. */
            ++counter;
            if (counter > 10)
                break;

            centerRaycast = raycast.EarthCenter();
            if (float.IsNaN(centerRaycast.Item1.x))
                return;
            centerCoords = polarCoord.CalculateCoordinates(centerRaycast.Item1);
            centerLat = centerCoords.y;
            centerLon = centerCoords.x;

            diffLat = centerLat - latitude;
            diffLon = centerLon - longitude;

            earth.Rotate(Camera.main.transform.right, diffLat, Space.World);
            earth.Rotate(-Vector3.up, diffLon, Space.World);
        }
        while (Mathf.Abs(diffLat) > 1f || Mathf.Abs(diffLon) > 1f);

        pinManager.MovePin(centerRaycast.Item1, centerRaycast.Item2);
    }
}
