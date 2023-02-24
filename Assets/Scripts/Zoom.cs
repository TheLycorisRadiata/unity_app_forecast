using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField] private UserInput input;

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
        float newPosZ = Mathf.Clamp(transform.position.z + input.scroll * stepPosZ, minPosZ, maxPosZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPosZ);

        if (input.cancel)
            transform.position = new Vector3(transform.position.x, transform.position.y, defaultPosZ);
    }
}
