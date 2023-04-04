using System;
using UnityEngine;
using TMPro;

public class CoordinatesText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI latitude;
    [SerializeField] private TextMeshProUGUI longitude;
    
    public void CoordinatesTextUpdate(float fLatitude, float fLongitude)
    {
        latitude.text = "Latitude: " + (float)Math.Round(fLatitude, 2);
        longitude.text = "Longitude: " + (float)Math.Round(fLongitude, 2);
    }
}
