[System.Serializable]
public class Nominatim
{
    public string error;
    public string osm_type;
    public int osm_id;
    public NominatimAddress address;
}

[System.Serializable]
public class NominatimAddress
{
    // Different properties for the location name
    public string town;
    public string city;
    public string municipality;
    public string village;
    public string county;
    public string province;
    public string region;
    public string state_district;
    public string state;
    public string country;
    public string man_made;

    // Country code (e.g. France is "fr")
    public string country_code;
}
