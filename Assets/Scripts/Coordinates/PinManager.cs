using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PinManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _fireValue;
    [SerializeField] private Raycast _raycast;
    [SerializeField] private LocationScriptableObjectScript _locationScript;
    [SerializeField] private Transform _earthModel;

    private GameObject _pin;
    [SerializeField] private GameObject _pinPrefab;
    private bool _isPinned = false;

    [SerializeField] private UnityEvent _openMenu;
    /* PanelOpener.OpenPanel + EarthAnimator.MoveEarth */

    private void OnEnable()
    {
        _fireValue.action.started += OnFire;
    }

    private void OnDisable()
    {
        _fireValue.action.started -= OnFire;
    }

    private void OnFire(InputAction.CallbackContext action)
    {
        if (_raycast.HasHit && !MenuPointer.IsPointerOnMenu)
        {
            if (!_isPinned)
            {
                CreatePin(_raycast.Point, _raycast.Rotation);
                _locationScript.UpdateLocation(_raycast.Point);
                _openMenu.Invoke();
            }
            else
            {
                MovePin(_raycast.Point, _raycast.Rotation);
                _locationScript.UpdateLocation(_raycast.Point);
            }
        }
    }

    public void CreatePin(Vector3 raycastPoint, Quaternion raycastRotation)
    {
        _pin = Instantiate(_pinPrefab, raycastPoint, raycastRotation, _earthModel);
        _pin.SetActive(true);
        _isPinned = true;
    }

    public void MovePin(Vector3 raycastPoint, Quaternion raycastRotation)
    {
        _pin.SetActive(false);
        _pin.transform.position = raycastPoint;
        _pin.transform.rotation = raycastRotation;
        _pin.SetActive(true);
    }
}
