using System.Collections;
using System.Collections.Generic;
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
        float newPosZ = transform.position.z + input.scroll * stepPosZ;

        if (newPosZ >= minPosZ && newPosZ <= maxPosZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, newPosZ);
    }
}
