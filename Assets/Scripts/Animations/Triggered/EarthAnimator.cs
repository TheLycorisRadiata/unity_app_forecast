using System.Collections;
using UnityEngine;

public class EarthAnimator : MonoBehaviour
{
    private float distance = 45f;
    private float duration = 2f;
    private bool earthMoved = false;
    private Vector3 earthInitialPosition, earthOpenPosition;

    public void Start()
    {
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
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
