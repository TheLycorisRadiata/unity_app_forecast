﻿using System;
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
    [SerializeField] private Raycast _raycast;
    [SerializeField] private PolarCoordinates _polarCoord;
    [SerializeField] private Transform _earth;
    [SerializeField] private PinManager _pinManager;
    [SerializeField] private Forecast _forecast;

    [SerializeField] private LocationScriptableObject _location;
    [SerializeField] private LocationScriptableObjectScript _locationScript;
    [SerializeField] private TMP_InputField _userInput;
    private string _previousUserInput = "";

    private List<OmgLocation> _locationList;
    [SerializeField] private TextMeshProUGUI _listCount;
    [SerializeField] private GameObject _locationListSpinner;
    [SerializeField] private Transform _locationListContent;
    [SerializeField] private GameObject _locationListPrefab;

    /* OnClick event in the validate button OR from the TextInput script */
    public void FetchLocationList()
    {
        _userInput.text = StringFormat.RemoveExtraSpaces(_userInput.text.ToLower());
        if (_userInput.text == "")
            return;
        else if (_userInput.text == _previousUserInput)
        {
            _forecast.HideForecast();
            DisplayGeocoding();
            return;
        }
        if (_previousUserInput == "")
            _previousUserInput = _userInput.text;

        _forecast.HideForecast();
        EmptyList();
        _locationListSpinner.SetActive(true);

        StartCoroutine(FetchOpenMeteoGeocodingData((locations) =>
        {
            if (locations == null)
            {
                PopulateMenu(locations);
                return;
            }

            StartCoroutine(FetchNominatimDisplayNames(locations, (locations) => PopulateMenu(locations)));
        }));
    }

    public void HideGeocoding()
    {
        _locationListSpinner.SetActive(false);
        _locationListContent.gameObject.SetActive(false);
    }

    public void DisplayGeocoding()
    {
        _locationListSpinner.SetActive(false);
        _locationListContent.gameObject.SetActive(true);
    }

    private IEnumerator FetchOpenMeteoGeocodingData(Action<List<OmgLocation>> callback)
    {
        string encodedInput = HttpUtility.UrlEncode(_userInput.text);
        string uri = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedInput}&language=en";
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
                Debug.LogWarning($"Geocoding error: The request went through but the API couldn't find any location of this name ({_userInput}).");
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
            uri = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&accept-language=en&format=json";

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
                if (StringFormat.WordContainsWord(name, _userInput.text))
                {
                    /*
                        - [...], Paris, Île-de-France, France métropolitaine, 75004, France
                        - [...], Paris, Lemar County, Texas, 75460, Etats-Unis d'Amérique
                        - [...], Nice, Alpes-Maritimes, Provence-Alpes-Côte d'Azur, France métropolitaine, 06000, France
                        - 6e Soufrière, Commune d'Acul-du-Nord, Arrondissement d'Acul-du-Nord, Département du Nord, Haïti
                    */

                    arr = name.Split(", ");

                    /* Remove words before user input */
                    firstIndex = arr.ToList().FindIndex(element => StringFormat.WordComparison(element, _userInput.text));
                    if (firstIndex < 0)
                        continue;
                    arr = Enumerable.Reverse(arr).Take(arr.Length - firstIndex).Reverse().ToArray();

                    /* Remove words with a number in them */
                    arr = arr.Where(element => !element.Any(char.IsDigit)).ToArray();
                    /* Remove words with a non-latin character in them */
                    arr = arr.Where(element => Regex.IsMatch(element, "[a-z]", RegexOptions.IgnoreCase)).ToArray();

                    name = string.Join(", ", arr);

                    /* Check if the input is still in the name, now that it has been reduced */
                    if (!StringFormat.WordContainsWord(name, _userInput.text))
                        continue;
                    /*
                        Check if the element doesn't already exist in the list 
                        (Note: the first iteration is kept because it's likely to be the most accurate one for this location, 
                        as the API tries to order the elements according to their relevancy)
                    */
                    else if (locations.Any(element => element.displayName == name))
                        continue;

                    /* displayName is null by default, this will give it a value */
                    locations[i].displayName = name;
                }
            }
        }

        /* Remove incorrect locations */
        locations.RemoveAll(element => element?.displayName == null);

        if (locations.Count > 2)
        {
            /* The first element remains the first, because it's likely to be what the user is searching for. */
            OmgLocation first = locations[0];
            locations.RemoveAt(0);

            /* The rest is sorted in alphabetical order based on the country name, then the region, etc. */
            locations = locations.OrderBy(e => e, new OmgLocationComparer()).ToList();

            /* Add back the first element */
            locations.Insert(0, first);
        }

        callback(locations);
    }

    private void EmptyList()
    {
        int i;
        _locationList = new List<OmgLocation>();
        _listCount.text = "0 location";
        for (i = 0; i < _locationListContent.childCount; ++i)
            Destroy(_locationListContent.GetChild(i).gameObject);
    }

    private void PopulateMenu(List<OmgLocation> locations)
    {
        int i;
        _locationList = locations;

        /* Make it possible to find Null Island */
        if (_locationList.Count == 0 && StringFormat.WordComparison(_userInput.text, "Null Island") || StringFormat.WordComparison(_userInput.text, "Null"))
            _locationList.Add(new OmgLocation(0f, 0f, null, "Null Island"));

        _listCount.text = _locationList.Count < 2 ? $"{_locationList.Count} location" : $"{_locationList.Count} locations";

        for (i = 0; i < _locationList.Count; ++i)
        {
            Transform t = Instantiate(_locationListPrefab, _locationListContent.position, Quaternion.identity).transform;
            t.SetParent(_locationListContent);
            t.name = i.ToString();
            t.localScale = new Vector3(1f, 1f, 1f);
            t.GetChild(0).GetComponent<TextMeshProUGUI>().text = _locationList[i].displayName;
            t.GetComponent<Button>().onClick.AddListener(() => SelectLocation(int.Parse(t.name)));
        }

        DisplayGeocoding();
    }

    private void SelectLocation(int index)
    {
        StyleLocationElements(_locationListContent.GetChild(index));
        _locationScript.UpdateLocation(_locationList[index]);
        GoToLocation(_location.Latitude, _location.Longitude);
    }

    private void StyleLocationElements(Transform selectedElement)
    {
        Color yellow = new Color(224f / 255f, 182f / 255f, 37f / 255f);
        Color blue = new Color(40f / 255f, 190f / 255f, 169f / 255f);
        int i;

        for (i = 0; i < _locationListContent.childCount; ++i)
            _locationListContent.GetChild(i).GetComponent<Image>().color = blue;
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

            centerRaycast = _raycast.EarthCenter();
            if (float.IsNaN(centerRaycast.Item1.x))
                return;
            centerCoords = _polarCoord.CalculateCoordinates(centerRaycast.Item1);
            centerLat = centerCoords.y;
            centerLon = centerCoords.x;

            diffLat = centerLat - latitude;
            diffLon = centerLon - longitude;

            _earth.Rotate(Camera.main.transform.right, diffLat, Space.World);
            _earth.Rotate(-Vector3.up, diffLon, Space.World);
        }
        while (Mathf.Abs(diffLat) > 1f || Mathf.Abs(diffLon) > 1f);

        _pinManager.MovePin(centerRaycast.Item1, centerRaycast.Item2);
    }
}
