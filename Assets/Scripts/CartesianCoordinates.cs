using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartesianCoordinates : MonoBehaviour
{
    [SerializeField] private WebglRaycast raycastScript;
    [SerializeField] private RotateHorizontally horizontalRotationScript;
    [SerializeField] private RotateVertically verticalRotationScript;

    void Update()
    {
        /*
            TODO
            - [X] Get the raycast's vector3, the horizontal rotation (pitch) and the vertical rotation (yaw)
            - [ ] Instead of using the raycast vector3 as is, add an offset with the earth's rotations

            OBSERVATIONS
            - The zoom doesn't influence the raycast, so it's not to be taken into account
            - The raycast Z axis seems irrelevant, so only use X and Y

            POTENTIAL HELP
            - https://learn.unity.com/tutorial/cartesian-coordinates-1#5defc875edbc2a0c765b5ca8
        */

        Debug.Log("Raycast: " + raycastScript.RaycastPoint);
        Debug.Log("Horizontal rotation: " + horizontalRotationScript.pitch);
        Debug.Log("Vertical rotation: " + verticalRotationScript.yaw);
        Debug.Log("-------------");
    }
}
