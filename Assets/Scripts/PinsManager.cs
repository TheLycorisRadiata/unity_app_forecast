using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinsManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private WebglRaycast webglRaycast;
    [SerializeField] private PolarCoordinates polarScript;
    [SerializeField] private Vector3 pinsPosition;
    public Vector2 polarCoordinates;

    public UnityEvent OpenMenu;
    public GameObject pinsPrefab;
    public Transform parent;
    private GameObject pins;
    private bool isPinned;
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
            OpenMenu.Invoke();
        }
        else
        {
            MovePin();
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
}
