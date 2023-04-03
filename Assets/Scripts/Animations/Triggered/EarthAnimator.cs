using System.Collections;
using UnityEngine;

public class EarthAnimator : MonoBehaviour
{
    private float distance = 45f;
    private Vector3 fixRotation;
    private float duration = 2f;
    private bool earthMoved = false;
    private Vector3 earthInitialPosition, earthOpenPosition;

    void Start()
    {
        fixRotation = new Vector3(0f, distance / 3, 0f);
        earthInitialPosition = transform.position;
        earthOpenPosition = earthInitialPosition + Vector3.left * distance;
    }

    public void MoveEarth()
    {
        StopAllCoroutines();
        if (!earthMoved)
        {
            StartCoroutine(ShiftEarth(earthOpenPosition));
        }
        else
        {
            StartCoroutine(ShiftEarth(earthInitialPosition));
        }
        earthMoved = !earthMoved;
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
        Vector3 targetRotation = transform.eulerAngles - fixRotation;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
