using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinsManager : MonoBehaviour
{

    [SerializeField] public WebglRaycast webglRaycast;
    [SerializeField] private Vector3 pinsPosition;
    public GameObject pinsPrefab;
    public Transform parent;
    private GameObject pins;
    private bool IsPinned;
    private Quaternion pinsRotation;

    // Start is called before the first frame update
    void Start()
    {
        IsPinned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
