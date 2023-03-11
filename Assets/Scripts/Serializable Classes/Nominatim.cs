using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class Nominatim
{
    public string error;
    [JsonProperty("osm_type")] public string osmType;
    [JsonProperty("osm_id")] public string osmId; /* string instead of a number, because some IDs are way too big */
    [JsonProperty("display_name")] public string displayName; /* Used for geocoding */
    public NominatimAddress address; /* Used for reverse geocoding (+ it contains a "countryCode" property) */
}

public class NominatimAddress
{
    [JsonProperty("country_code")] public string countryCode;
    public string country;

    /* All the other address properties (e.g. shop, road, town, region...) */
    [JsonExtensionData] public Dictionary<string, JToken> otherData;

    /*
        The properties below must not be in otherData and are to be ignored.
        However, using [JsonIgnore] would make them caught by otherData,
        so instead they are declared here and simply not used.
    */
    public string postcode;
    [JsonProperty("ISO3166-2-lvl4")] public string iso4;
    [JsonProperty("ISO3166-2-lvl6")] public string iso6;
}
