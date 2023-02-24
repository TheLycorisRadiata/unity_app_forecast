using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinsManager : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private WebglRaycast webglRaycast;
    [SerializeField] private Vector3 pinsPosition;
    public UnityEvent OpenMenu;
    public GameObject pinsPrefab;
    public Transform parent;
    private GameObject pins;
    private bool IsPinned;
    private Quaternion pinsRotation; 

    void Start()
    {
        IsPinned = false;
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

        if (IsPinned == false)
        {
            pins = Instantiate(pinsPrefab, pinsPosition, pinsRotation, parent);
            pins.SetActive(true);
            IsPinned = true;
            OpenMenu.Invoke();
        }
        else
        {
            pins.SetActive(false);
            pins.transform.position = pinsPosition;
            pins.transform.rotation = pinsRotation;
            pins.SetActive(true);
        }
    }
}
