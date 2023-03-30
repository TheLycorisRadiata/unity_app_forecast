using UnityEngine;
using UnityEngine.Events;

public class PinManager : MonoBehaviour
{
    [SerializeField] private Raycast raycast;
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
        if (UserInput.click && raycast.hasHit && !MenuPointer.isPointerOnMenu)
        {
            PinWithRaycast();
            UserInput.click = false;
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
            OpenMenu.Invoke();
        }
        else
        {
            MovePin();
            locationScript.UpdateLocation(pinPosition);
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
}
