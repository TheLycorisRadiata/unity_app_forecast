using UnityEngine;

[CreateAssetMenu(fileName = "LocationScriptableObject", menuName = "ScriptableObjects/Location", order = 1)]
public class LocationScriptableObject : ScriptableObject
{
    public string locationName;
    public float latitude;
    public float longitude;
    public string countryCode;
}
