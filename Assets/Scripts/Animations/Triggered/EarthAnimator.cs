using System.Collections;
using UnityEngine;

public class EarthAnimator : MonoBehaviour
{
    private float distance = 45f;
    private Vector3 fixRotation;
    private float duration = 2f;
    private bool earthMoved = false;
    private Vector3 earthInitialPosition, earthOpenPosition;

    public void Start()
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

    public void RotateEarthToCenterLocation(float latitude, float longitude)
    {
        Vector2 initialCenterCoordinates = new Vector2(40f, 20f); /* Not certain */
        Vector3 initialCenterRotation = new Vector3(0f, -15f, 0f);

        /*
            Initial center (40, 20): 0, -15, 0
            Centered Tokyo (36.5, 139.35): -13.5, 71, -57

            --> What I got: 69.4, 72.4, -45.4 --> What would be cool: x, 68 or 69, -64
            transform.Rotate(Camera.main.transform.right, latitude + initialCenterCoordinates.x, Space.World);
            transform.Rotate(Vector3.up, longitude - initialCenterCoordinates.y, Space.World);
        */

        transform.Rotate(Camera.main.transform.right, latitude + initialCenterCoordinates.x, Space.World);
        transform.Rotate(Vector3.up, longitude - initialCenterCoordinates.y, Space.World);

        /*
            - Initial Earth rotation is Vector3.zero, but it may have changed since then.
            - At Vector3.zero, the initial center coordinates are (40, 20).
        */
    }
}
