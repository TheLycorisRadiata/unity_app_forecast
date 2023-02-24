using System;
using UnityEngine;
using UnityEngine.Events;

public class PinsManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private WebglRaycast webglRaycast;
    [SerializeField] private Transform earthModelTransform;
    [SerializeField] private LatLongText latLongText;

    public UnityEvent OpenMenu;
    public GameObject pinsPrefab;
    public Location location;
    public Transform parent;
    private GameObject pins;
    private bool isPinned;
    private Vector3 pinsPosition;
    private Quaternion pinsRotation;
    

    void Start()
    {
        isPinned = false;
    }

    void Update()
    {
        if (input.click)
            PinsWithRaycast();
    }

    public void PinsWithRaycast()
    {
        pinsPosition = webglRaycast.RaycastPoint;
        pinsRotation = webglRaycast.RaycastRotation;

        if (isPinned == false)
        {
            CreatePin();
            UpdatePolarCoordinates();
            latLongText.LatLongTextUpdate();
            OpenMenu.Invoke();
        }
        else
        {
            MovePin();
            UpdatePolarCoordinates();
            latLongText.LatLongTextUpdate();
        }
    }

    private void CreatePin()
    {
        pins = Instantiate(pinsPrefab, pinsPosition, pinsRotation, parent);
        pins.SetActive(true);
        isPinned = true;
    }

    private void MovePin()
    {
        pins.SetActive(false);
        pins.transform.position = pinsPosition;
        pins.transform.rotation = pinsRotation;
        pins.SetActive(true);
    }

    private void UpdatePolarCoordinates()
    {
        Vector3 coordinates = earthModelTransform.InverseTransformPoint(pinsPosition);
        coordinates.y = 90f - Mathf.Acos(coordinates.y / coordinates.magnitude) * Mathf.Rad2Deg;
        coordinates.x = Mathf.Atan2(coordinates.z, coordinates.x) * Mathf.Rad2Deg;

        location.locationLat = (float)Math.Round(coordinates.y, 2);
        location.locationLon = (float)Math.Round(coordinates.x, 2);
    }
}
