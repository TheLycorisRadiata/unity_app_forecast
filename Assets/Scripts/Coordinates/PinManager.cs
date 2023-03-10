using System;
using UnityEngine;
using UnityEngine.Events;

public class PinManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private Raycast raycast;
    [SerializeField] private PolarCoordinates polarCoordinates;
    [SerializeField] private ReverseGeocoding reverseGeocoding;
    [SerializeField] private CountryFlag countryFlag;
    [SerializeField] private CoordinatesText coordinatesText;

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
        polarCoordinates.UpdateLocationCoordinates(pinPosition);
        reverseGeocoding.UpdateLocationNameAndCountryCode();
    }

    private void UpdateMenu()
    {
        coordinatesText.CoordinatesTextUpdate();
        countryFlag.UpdateFlag();
    }
}
