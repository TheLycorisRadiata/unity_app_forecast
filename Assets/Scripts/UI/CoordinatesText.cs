using UnityEngine;
using TMPro;

public class CoordinatesText : MonoBehaviour
{
    public LocationScriptableObject location;
    public TextMeshProUGUI latitude;
    public TextMeshProUGUI longitude;
    
    public void CoordinatesTextUpdate()
    {
        latitude.text = "Latitude: " + location.latitude;
        longitude.text = "Longitude: " + location.longitude;
    }
}
