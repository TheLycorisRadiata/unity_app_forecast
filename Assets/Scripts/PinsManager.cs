using System;
using UnityEngine;
using UnityEngine.Events;

public class PinsManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private ApiInteraction api;
    [SerializeField] private WebglRaycast webglRaycast;
    [SerializeField] private PolarCoordinates polarScript;
    public float latitude, longitude;

    public UnityEvent OpenMenu;
    public GameObject pinsPrefab;
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
        {
            PinsWithRaycast();
            input.click = false;
        }
    }

    public void PinsWithRaycast()
    {
        pinsPosition = webglRaycast.RaycastPoint;
        pinsRotation = webglRaycast.RaycastRotation;

        if (isPinned == false)
        {
            CreatePin();
            SavePolarCoordinates();
            OpenMenu.Invoke();
        }
        else
        {
            MovePin();
            SavePolarCoordinates();
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

    private void SavePolarCoordinates()
    {
        latitude = (float)Math.Round(polarScript.coordinates.y, 2);
        longitude = (float)Math.Round(polarScript.coordinates.x, 2);

        Debug.Log("test");
    }
}
