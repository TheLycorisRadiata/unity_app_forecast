using System.Collections;
using UnityEngine;

public class EarthAnimator : MonoBehaviour
{
    private float _distance = 45f;
    private Vector3 _fixRotation;
    private float _duration = 2f;
    private bool _earthMoved = false;
    private Vector3 _earthInitialPosition, _earthOpenPosition;

    void Start()
    {
        _fixRotation = new Vector3(0f, _distance / 3, 0f);
        _earthInitialPosition = transform.position;
        _earthOpenPosition = _earthInitialPosition + Vector3.left * _distance;
    }

    public void MoveEarth()
    {
        StopAllCoroutines();
        if (!_earthMoved)
        {
            StartCoroutine(ShiftEarth(_earthOpenPosition));
        }
        else
        {
            StartCoroutine(ShiftEarth(_earthInitialPosition));
        }
        _earthMoved = !_earthMoved;
    }

    private IEnumerator ShiftEarth(Vector3 targetPosition)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;

        /*
            Pushing Earth to the side does visually change its rotation, even though the values remain the same in the inspector.
            To make it so it doesn't visually change, for a distance of 45, do: rotY - 15.
        */
        Vector3 startRotation = transform.eulerAngles;
        Vector3 targetRotation = transform.eulerAngles - _fixRotation;

        while (timeElapsed < _duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / _duration);
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, timeElapsed / _duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
