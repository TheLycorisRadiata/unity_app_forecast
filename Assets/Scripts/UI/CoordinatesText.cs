using UnityEngine;
using TMPro;

public class CoordinatesText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI latitude;
    [SerializeField] private TextMeshProUGUI longitude;
    
    public void CoordinatesTextUpdate(float fLatitude, float fLongitude)
    {
        latitude.text = "Latitude: " + fLatitude;
        longitude.text = "Longitude: " + fLongitude;
    }
}
