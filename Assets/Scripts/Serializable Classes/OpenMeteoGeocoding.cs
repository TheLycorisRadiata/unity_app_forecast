using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class OpenMeteoGeocoding
{
    public List<OmgLocation> results;

    public OpenMeteoGeocoding()
    {
        this.results = new List<OmgLocation>();
    }

    public OpenMeteoGeocoding(string jsonText)
    {
        OpenMeteoGeocoding obj = JsonConvert.DeserializeObject<OpenMeteoGeocoding>(jsonText);
        this.results = obj.results;
    }
}

public class OmgLocation
{
    public float latitude;
    public float longitude;
    [JsonProperty("country_code")] public string countryCode;

    /* Set with Nominatim */
    public string displayName;

    public OmgLocation()
    {
        this.latitude = 0f;
        this.longitude = 0f;
        this.countryCode = null;
        this.displayName = null;
    }

    public OmgLocation(float latitude, float longitude, string countryCode, string displayName)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.countryCode = countryCode;
        this.displayName = displayName;
    }
}
