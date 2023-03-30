using UnityEngine;

public class Zoom : MonoBehaviour
{
    private float rangePosZ = 40f;
    private float stepPosZ = 5f;
    private float defaultPosZ, minPosZ, maxPosZ;

    void Awake()
    {
        defaultPosZ = transform.position.z;
        minPosZ = defaultPosZ - rangePosZ;
        maxPosZ = defaultPosZ + rangePosZ;
    }

    void LateUpdate()
    {
        float newPosZ;

        if (MenuPointer.isPointerOnMenu)
            return;

        newPosZ = Mathf.Clamp(transform.position.z + UserInput.scroll * stepPosZ, minPosZ, maxPosZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPosZ);

        if (UserInput.cancel)
            transform.position = new Vector3(transform.position.x, transform.position.y, defaultPosZ);
    }
}
