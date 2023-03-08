using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class ReverseGeocoding : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;

    public void UpdateLocationNameAndCountryCode()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        // Depending on the user's language, the dot in the float will become a comma when inserted in a string
        string latitude = StringFormat.Float(location.latitude);
        string longitude = StringFormat.Float(location.longitude);
        string uri = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&accept-language=fr,en&format=json";

        string jsonText;
        Nominatim nominatim;

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
            nominatim = JsonUtility.FromJson<Nominatim>(jsonText);

            if (nominatim.error != null)
            {
                Debug.LogWarning($"Reverse geocoding error: The request went through but the API couldn't find a location from these coordinates ({location.latitude},{location.longitude}).");
                ResetLocationNameAndCountryCode();
                yield break;
            }

            location.locationName = GetLocationName(nominatim.address);
            location.countryCode = nominatim.address.country_code;

            // If there is at least one non-latin character in the string
            if (!Regex.IsMatch(location.locationName, "[a-z]", RegexOptions.IgnoreCase))
                StartCoroutine(FetchNameTranslation(nominatim.osm_type, nominatim.osm_id));
            // If the location is Null Island
            else if (nominatim.osm_type == "node" && nominatim.osm_id == "3815077900")
                StartCoroutine(FetchNameTranslation("node", "3815077900"));
        }
    }

    private void ResetLocationNameAndCountryCode()
    {
        location.locationName = null;
        location.countryCode = null;
    }

    private string GetLocationName(NominatimAddress address)
    {
        List<string> names = new List<string>();
        string nativeName = null;
        int i;

        // Add all the non-null names
        if (address.town != null)
            names.Add(address.town);
        if (address.city != null)
            names.Add(address.city);
        if (address.municipality != null)
            names.Add(address.municipality);
        if (address.village != null)
            names.Add(address.village);
        if (address.county != null)
            names.Add(address.county);
        if (address.province != null)
            names.Add(address.province);
        if (address.region != null)
            names.Add(address.region);
        if (address.state_district != null)
            names.Add(address.state_district);
        if (address.state != null)
            names.Add(address.state);
        if (address.country != null)
            names.Add(address.country);
        if (address.man_made != null)
            names.Add(address.man_made);

        // Save at least one name
        nativeName = names[0];

        // Remove the non-latin names
        for (i = 0; i < names.Count; ++i)
        {
            // If there is at least one non-latin character in the string
            if (!Regex.IsMatch(names[i], "[a-z]", RegexOptions.IgnoreCase))
                names[i] = null;
        }
        names.RemoveAll(element => element == null);

        // Return the first latin name or the native one
        return names.Count > 0 ? names[0] : nativeName;
    }

    private IEnumerator FetchNameTranslation(string osmType, string osmId)
    {
        string uri = $"https://www.openstreetmap.org/api/0.6/{osmType}/{osmId}";
        string xmlText;
        XmlSerializer serializer;
        OSM osm;
        string translation;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
                yield break;

            xmlText = webRequest.downloadHandler.text;
            serializer = new XmlSerializer(typeof(OSM));
            using (StringReader reader = new StringReader(xmlText))
                osm = (OSM)(serializer.Deserialize(reader)) as OSM;

            if (osm.relation != null)
            {
                translation = FindTranslation(osm.relation.tags);
                if (translation != null)
                    location.locationName = translation;
            }
            else if (osm.node != null)
            {
                translation = FindTranslation(osm.node.tags);
                if (translation != null)
                    location.locationName = translation;
            }
            else if (osm.way != null)
            {
                translation = FindTranslation(osm.way.tags);
                if (translation != null)
                    location.locationName = translation;
            }
        }
    }

    private string FindTranslation(List<TagOSM> tags)
    {
        string tagName;
        string french = null, english = null, international = null, native = null;

        foreach (var tag in tags)
        {
            tagName = tag.name;

            if (tagName == "name:fr")
                french = tag.value;
            else if (tagName == "name:en")
                english = tag.value;
            else if (tagName == "int_name")
                international = tag.value;
            else if (tagName == "name")
                native = tag.value;
        }

        return french ?? english ?? international ?? native ?? null;
    }
}
