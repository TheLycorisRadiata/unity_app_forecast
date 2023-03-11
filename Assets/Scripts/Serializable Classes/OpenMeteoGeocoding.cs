using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class OpenMeteoGeocoding
{
    public List<OmgLocation> results;
}

public class OmgLocation
{
    public float latitude;
    public float longitude;
    [JsonProperty("country_code")] public string countryCode;

    /* Set with Nominatim */
    public string displayName;
}
