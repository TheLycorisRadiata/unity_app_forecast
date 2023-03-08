using System;
using System.Collections.Generic;

[Serializable]
public class OpenMeteoGeocoding
{
    public List<OmgLocation> results;
}

[Serializable]
public class OmgLocation
{
    public string display_name; // Set with Nominatim
    public float latitude;
    public float longitude;
    public string country_code;
}
