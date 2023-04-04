using System;
using System.Collections.Generic;
using System.Linq;
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

public class OmgLocationComparer : IComparer<OmgLocation>
{
    public int Compare(OmgLocation x, OmgLocation y)
    {
        string[] xParts = x.displayName.Split(", ").Reverse().ToArray();
        string[] yParts = y.displayName.Split(", ").Reverse().ToArray();
        int i, result;

        for (i = 0; i < Math.Min(xParts.Length, yParts.Length); ++i)
        {
            result = string.Compare(xParts[i], yParts[i]);
            if (result != 0)
                return result;
        }

        return 0;
    }
}