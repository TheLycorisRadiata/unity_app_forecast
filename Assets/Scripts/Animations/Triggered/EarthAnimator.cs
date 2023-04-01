using System.Collections;
using UnityEngine;

public class EarthAnimator : MonoBehaviour
{
    private PolarCoordinates polarCoordinates;
    private float distance = 45f;
    private Vector3 fixRotation;
    private float duration = 2f;
    private bool earthMoved = false;
    private Vector3 earthInitialPosition, earthOpenPosition;

    void Start()
    {
        polarCoordinates = GetComponent<PolarCoordinates>();
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
        Vector2 currCenterCoord;
        float currCenterLat, currCenterLon;
        float diffLat, diffLon;
        int counter = 0;

        do
        {
            currCenterCoord = polarCoordinates.CalculateCoordinatesAtCenter();
            currCenterLat = currCenterCoord.y;
            currCenterLon = currCenterCoord.x;

            diffLat = currCenterLat - latitude;
            diffLon = currCenterLon - longitude;

            /* For some reason several passes are needed */
            transform.Rotate(Camera.main.transform.right, diffLat, Space.World);
            transform.Rotate(-Vector3.up, diffLon, Space.World);

            /* Add a limit */
            ++counter;
            if (counter > 20)
                break;
        }
        while (Mathf.Abs(diffLat) > 1f || Mathf.Abs(diffLon) > 1f);
    }
}
