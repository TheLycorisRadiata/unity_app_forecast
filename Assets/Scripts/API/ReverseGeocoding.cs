using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ReverseGeocoding : MonoBehaviour
{
    public IEnumerator FetchNominatimData(float latitude, float longitude, Action<Nominatim> callback)
    {
        // Depending on the user's language, the dot in the float will become a comma when inserted in a string
        string strLatitude = StringFormat.Float(latitude);
        string strLongitude = StringFormat.Float(longitude);
        string uri = $"https://nominatim.openstreetmap.org/reverse?lat={strLatitude}&lon={strLongitude}&accept-language=fr,en&format=json";

        string jsonText;
        Nominatim nominatim;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Reverse geocoding error: The request failed to fetch from the API.");
                yield break;
            }

            jsonText = webRequest.downloadHandler.text;
            nominatim = new Nominatim(jsonText);

            if (nominatim.error != null)
            {
                Debug.LogWarning($"Reverse geocoding error: The request went through but the API couldn't find a location from these coordinates ({latitude},{longitude}).");
                yield break;
            }

            callback(nominatim);
        }
    }

    public string GetLocationNameFromNominatimData(Dictionary<string, JToken> otherAddressData)
    {
        string[] smallestAllowedLocation = { "municipality", "city", "town", "village" };
        int smallestIndex;
        string potentiallyNonLatinName = null;
        List<string> keysToRemove;

        if (otherAddressData?.Count == 0)
            return null;

        // Find the index of the smallest allowed location
        smallestIndex = otherAddressData.Keys.ToList().IndexOf(smallestAllowedLocation.FirstOrDefault(location => otherAddressData.ContainsKey(location)));

        // Remove all elements before this location (0 or -1 are legal and will simply not remove anything)
        otherAddressData = otherAddressData.Skip(smallestIndex).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        if (otherAddressData.Count != 0)
        {
            // Save at least one location name
            potentiallyNonLatinName = (string)otherAddressData.First().Value;

            // Select all KVP whose value has at least one non-latin character
            keysToRemove = otherAddressData.Where(kvp => !Regex.IsMatch((string)kvp.Value, "[a-z]", RegexOptions.IgnoreCase)).Select(kvp => kvp.Key).ToList();

            // Remove the non-latin names
            keysToRemove.ForEach(key => otherAddressData.Remove(key));
        }

        // Return the first latin name or the native one
        return otherAddressData.Count != 0 ? (string)otherAddressData.First().Value : potentiallyNonLatinName;
    }

    public IEnumerator FetchNameTranslationFromOSM(string osmType, string osmId, Action<string> callback)
    {
        string uri = $"https://www.openstreetmap.org/api/0.6/{osmType}/{osmId}";
        string xmlText;
        List<TagOSM> tags;
        string translation = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
                yield break;

            xmlText = webRequest.downloadHandler.text;
            tags = new OpenStreetMap(xmlText).GetTags();
            if (tags.Count != 0)
                translation = GetTranslation(tags);

            callback(translation);
        }
    }

    private string GetTranslation(List<TagOSM> tags)
    {
        string french = null, english = null, international = null, native = null;

        foreach (TagOSM tag in tags)
        {
            if (tag.name == "name:fr")
                french = tag.value;
            else if (tag.name == "name:en")
                english = tag.value;
            else if (tag.name == "int_name")
                international = tag.value;
            else if (tag.name == "name")
                native = tag.value;
        }

        return french ?? english ?? international ?? native;
    }
}
