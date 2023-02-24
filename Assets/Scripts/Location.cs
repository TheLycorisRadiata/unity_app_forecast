using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location")]
public class Location : ScriptableObject
{
    public string locationName;
    public float locationLat;
    public float locationLon;
    public string locationCountryCode;
}
