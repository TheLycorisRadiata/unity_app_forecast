using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LatLongText : MonoBehaviour
{
    public Location location;
    public TextMeshProUGUI latitudeText; 
    public TextMeshProUGUI longitudeText;
    
    public void LatLongTextUpdate()
    {
        var lat = location.locationLat;
        var lon = location.locationLon;

        latitudeText.text = "Latitude: " + lat ;
        longitudeText.text = "Longitude: " + lon;
    }

}
