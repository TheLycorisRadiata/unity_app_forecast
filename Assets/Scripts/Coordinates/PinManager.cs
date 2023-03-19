using System;
using UnityEngine;
using UnityEngine.Events;

public class PinManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private Raycast raycast;
    [SerializeField] private CountryFlag countryFlag;
    [SerializeField] private CoordinatesText coordinatesText;

    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private Transform earthModel;
    [SerializeField] private LocationScriptableObjectScript locationScript;

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
            locationScript.UpdateLocation(pinPosition);
            UpdateMenu();
            OpenMenu.Invoke();
        }
        else
        {
            MovePin();
            locationScript.UpdateLocation(pinPosition);
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

    private void UpdateMenu()
    {
        coordinatesText.CoordinatesTextUpdate();
        countryFlag.UpdateFlag();
    }
}
