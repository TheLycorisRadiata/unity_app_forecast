using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAnimator : MonoBehaviour
{
    public float distance = 45f;
    public float duration = 2;
    bool earthMoved;
    Vector3 earthCenterPosition;
    public void Start()
    {
        // Sets the first position of the earth.
        earthCenterPosition = transform.position;
    }
    public void MoveEarth()
    {
        StopAllCoroutines();
        if (!earthMoved)
        {
            Vector3 openPosition = earthCenterPosition + Vector3.left * distance;
            StartCoroutine(ShiftEarth(openPosition));
        }
        else
        {
            StartCoroutine(ShiftEarth(earthCenterPosition));
        }
        earthMoved = !earthMoved;
    }
    IEnumerator ShiftEarth(Vector3 targetPosition)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
