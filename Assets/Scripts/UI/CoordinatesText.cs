using System;
using UnityEngine;
using TMPro;

public class CoordinatesText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _latitude;
    [SerializeField] private TextMeshProUGUI _longitude;
    
    public void CoordinatesTextUpdate(float fLatitude, float fLongitude)
    {
        _latitude.text = "Latitude: " + (float)Math.Round(fLatitude, 2);
        _longitude.text = "Longitude: " + (float)Math.Round(fLongitude, 2);
    }
}
