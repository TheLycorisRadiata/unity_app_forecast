using UnityEngine;

public class Zoom : MonoBehaviour
{
    private float _rangePosZ = 40f;
    private float _stepPosZ = 5f;
    private float _defaultPosZ, _minPosZ, _maxPosZ;

    void Awake()
    {
        _defaultPosZ = transform.position.z;
        _minPosZ = _defaultPosZ - _rangePosZ;
        _maxPosZ = _defaultPosZ + _rangePosZ;
    }

    void LateUpdate()
    {
        if (MenuPointer.IsPointerOnMenu)
            return;

        ApplyZoom();

        if (UserInput.Cancel)
            ResetPosition();
    }

    private void ApplyZoom()
    {
        float newPosZ = Mathf.Clamp(transform.position.z + UserInput.Scroll * _stepPosZ, _minPosZ, _maxPosZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPosZ);
    }

    private void ResetPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _defaultPosZ);
    }
}
