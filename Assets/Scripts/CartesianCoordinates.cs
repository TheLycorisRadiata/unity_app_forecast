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
            - [X] Get the raycast's vector3, the horizontal rotation (pitch) and the vertical rotation (yaw).
            - [X] Instead of using the raycast vector3 as is, add an offset with the earth's rotations.
            - [X] Reset the rotations to 0 if they are outside of -360/360.
            - [X] Convert the cartesian Y coordinate to the latitude equivalent.
            - [X] Convert the cartesian X coordinate to the longitude equivalent.
            - [ ] Add the earth curvature to the cartesian coordinates calculus.
            - [ ] Don't forget to take the Earth default rotation into account (e.g. -250 on the y axis), 
                  so that any chosen default rotation doesn't break the coordinates.
                  The 0x0 polar coordinates need to be at null island (around Africa).
        */

        float xRotation = horizontalRotationScript.pitch;
        float yRotation = -verticalRotationScript.yaw;

        float xRaycast = raycastScript.RaycastPoint.x;
        float yRaycast = raycastScript.RaycastPoint.y;
        float zRaycast = raycastScript.RaycastPoint.z;

        float xRaycastWithOffset = xRaycast + xRotation;
        float yRaycastWithOffset = yRaycast + yRotation;

        // Latitude goes first
        float latitude = yRaycastWithOffset;
        float longitude = xRaycastWithOffset;

        // If you go north until you do a full 360
        if (yRaycastWithOffset > 90f && yRaycastWithOffset <= 270f)
            latitude = (yRaycastWithOffset - 180f) * -1f;
        else if (yRaycastWithOffset > 270f && yRaycastWithOffset <= 360f)
            latitude = yRaycastWithOffset - 360f;

        // If you go south until you do a full 360
        if (yRaycastWithOffset < -90f && yRaycastWithOffset >= -270f)
            latitude = yRaycastWithOffset * -1f - 180f;
        else if (yRaycastWithOffset < -270f && yRaycastWithOffset >= -360f)
            latitude = yRaycastWithOffset + 360f;

        // If you go east
        if (xRaycastWithOffset > 180f && xRaycastWithOffset <= 360f)
            longitude = xRaycastWithOffset - 360f;
        // If you go west
        else if (xRaycastWithOffset <= -180f && xRaycastWithOffset >= -360f)
            longitude = xRaycastWithOffset + 360f;

        //Debug.Log("[Rotation] X: " + xRotation + " / Y: " + yRotation);
        Debug.Log("[Raycast] X (no offset): " + xRaycast + " / X (with offset): " + xRaycastWithOffset);
        Debug.Log("[Raycast] Y (no offset): " + yRaycast + " / Y (with offset): " + yRaycastWithOffset);
        Debug.Log("[Raycast] Z: " + zRaycast);
        Debug.Log("[Polar] Latitude: " + latitude + " / Longitude: " + longitude);
        Debug.Log("-------------");

        /*
            CURVATURE PROBLEM
            - zRaycast is -47.5 at 0x0, and this no matter the zoom.
            - But, when we aim for the horizon of the sphere, zRaycast changes depending on the zoom.

            - Otherwise, I have the beginning of a calculus:
                * zRaycast + 47.5 --> Result is a number between 0 and ~25 (at default zoom)
                * latitude - result --> Correct latitude (around +20 is added when latitude is sampled from the horizon and not 0x0)
        */
    }
}
