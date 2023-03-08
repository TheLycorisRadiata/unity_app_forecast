using System;
using UnityEngine;
using UnityEngine.Events;

public class PinManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private Raycast raycast;
    [SerializeField] private ReverseGeocoding reverseGeocoding;
    [SerializeField] private LatLongText latLongText;

    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private Transform earthModel;
    [SerializeField] private LocationScriptableObject location;

    [SerializeField] private UnityEvent OpenMenu;

    private GameObject pin;
    private bool isPinned;
    private Vector3 pinPosition;
    private Quaternion pinRotation;

    void Start()
    {
        isPinned = false;
    }

    void Update()
    {
        if (input.click)
        {
            PinWithRaycast();
            input.click = false;
        }
    }

    public void PinWithRaycast()
    {
        pinPosition = raycast.point;
        pinRotation = raycast.rotation;

        if (isPinned == false)
        {
            CreatePin();
            UpdateLocationData();
            UpdateMenu();
            OpenMenu.Invoke();
        }
        else
        {
            MovePin();
            UpdateLocationData();
            UpdateMenu();
        }
    }

    private void CreatePin()
    {
        pin = Instantiate(pinPrefab, pinPosition, pinRotation, earthModel);
        pin.SetActive(true);
        isPinned = true;
    }

    private void MovePin()
    {
        pin.SetActive(false);
        pin.transform.position = pinPosition;
        pin.transform.rotation = pinRotation;
        pin.SetActive(true);
    }

    private void UpdateLocationData()
    {
        UpdatePolarCoordinates();
        reverseGeocoding.UpdateLocationNameAndCountryCode();
    }

    private void UpdatePolarCoordinates()
    {
        Vector3 coordinates = earthModel.InverseTransformPoint(pinPosition);
        coordinates.y = 90f - Mathf.Acos(coordinates.y / coordinates.magnitude) * Mathf.Rad2Deg;
        coordinates.x = Mathf.Atan2(coordinates.z, coordinates.x) * Mathf.Rad2Deg;

        location.latitude = (float)Math.Round(coordinates.y, 2);
        location.longitude = (float)Math.Round(coordinates.x, 2);
    }

    private void UpdateMenu()
    {
        latLongText.LatLongTextUpdate();
    }
}
