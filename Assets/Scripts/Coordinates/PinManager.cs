using UnityEngine;
using UnityEngine.Events;

public class PinManager : MonoBehaviour
{
    [SerializeField] private Raycast raycast;

    private GameObject pin;
    [SerializeField] private GameObject pinPrefab;
    private bool isPinned = false;

    [SerializeField] private Transform earthModel;
    [SerializeField] private LocationScriptableObjectScript locationScript;
    [SerializeField] private UnityEvent OpenMenu;

    void Update()
    {
        if (UserInput.click && raycast.hasHit && !MenuPointer.isPointerOnMenu)
        {
            if (!isPinned)
            {
                CreatePin(raycast.point, raycast.rotation);
                locationScript.UpdateLocation(raycast.point);
                OpenMenu.Invoke();
            }
            else
            {
                MovePin(raycast.point, raycast.rotation);
                locationScript.UpdateLocation(raycast.point);
            }

            UserInput.click = false;
        }
    }

    public void CreatePin(Vector3 raycastPoint, Quaternion raycastRotation)
    {
        pin = Instantiate(pinPrefab, raycastPoint, raycastRotation, earthModel);
        pin.SetActive(true);
        isPinned = true;
    }

    public void MovePin(Vector3 raycastPoint, Quaternion raycastRotation)
    {
        pin.SetActive(false);
        pin.transform.position = raycastPoint;
        pin.transform.rotation = raycastRotation;
        pin.SetActive(true);
    }
}
