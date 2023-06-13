using UnityEngine;

[CreateAssetMenu(fileName = "LocationScriptableObject", menuName = "ScriptableObjects/Location", order = 1)]
public class LocationScriptableObject : ScriptableObject
{
    public string LocationName;
    public float Latitude;
    public float Longitude;
    public string CountryCode;
}
