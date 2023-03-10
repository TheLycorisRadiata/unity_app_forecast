using UnityEngine;
using TMPro;

public class LatLongText : MonoBehaviour
{
    public LocationScriptableObject location;
    public TextMeshProUGUI latitudeText; 
    public TextMeshProUGUI longitudeText;
    
    public void LatLongTextUpdate()
    {
        latitudeText.text = "Latitude: " + location.latitude;
        longitudeText.text = "Longitude: " + location.longitude;
    }
}
