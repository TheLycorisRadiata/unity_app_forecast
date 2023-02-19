using UnityEngine;

public class CartesianCoordinates : MonoBehaviour
{
    [SerializeField] private WebglRaycast raycastScript;
    [SerializeField] private RotateHorizontally horizontalRotationScript;
    [SerializeField] private RotateVertically verticalRotationScript;
    public float xCoordinate, yCoordinate;

    void Update()
    {
        /*
            TODO
            - [X] Get the raycast's vector3, the horizontal rotation (pitch) and the vertical rotation (yaw).
            - [X] Instead of using the raycast vector3 as is, add an offset with the earth's rotations.
            - [X] Rotation scripts: Reset the rotations to 0 if they are outside of -360/360.
            - [X] Convert the cartesian Y coordinate to the latitude equivalent.
            - [X] Convert the cartesian X coordinate to the longitude equivalent.

            NOTE: Only the X and Y values of the raycast have been used so far. This means that the coordinates are only correct when the cursor is right on the middle of the sphere.
            This can be fixed by adding the Z raycast value as an offset, but keep in mind that the Z raycast value changes depending on the zoom (camera position on the Z axis).
            - [ ] Add the Earth curvature to the cartesian coordinates computation.

            NOTE: The initial Earth rotation ("Model" gameobject) has its importance and is currently: 0, -250, 0.
            No matter the initial rotation, null island needs to be the 0x0 coordinates, it's around Africa.
            Once the horizontal and vertical rotations have been added to the cartesian coordinates as an offset (which is done), another offset needs to be added so that the coordinates are relative to null island.
            - [ ] Take the Earth default rotation into account, so that any chosen default rotation doesn't break the coordinates.
        */

        /*
            CURVATURE PROBLEM
            - zRaycast seems to be -47.5 at 0x0, and this no matter the zoom.
            - But, when we aim for the horizon of the sphere, zRaycast changes depending on the zoom.

            - Another observation is that around ~20 is added to the latitude when its sampled from the horizon and not 0x0.
            - I haven't checked for the longitude.
            - This is for the default zoom, the observed offset may be different from ~20 with another zoom.

            - Otherwise, I have the beginning of a calculus:
                * zRaycast + 47.5 --> Result is a number between 0 and ~25 (at default zoom)
                * latitude - result --> Correct latitude
        */

        float xRotation = horizontalRotationScript.pitch;
        float yRotation = -verticalRotationScript.yaw;

        float xRaycast = raycastScript.RaycastPoint.x;
        float yRaycast = raycastScript.RaycastPoint.y;
        float zRaycast = raycastScript.RaycastPoint.z;

        xCoordinate = xRaycast + xRotation;
        yCoordinate = yRaycast + yRotation;

        //Debug.Log("[Rotation] X: " + xRotation + " / Y: " + yRotation);
        //Debug.Log("[Raycast] X (no offset): " + xRaycast + " / X (with offset): " + xCoordinate);
        //Debug.Log("[Raycast] Y (no offset): " + yRaycast + " / Y (with offset): " + yCoordinate);
        //Debug.Log("[Raycast] Z: " + zRaycast);
        //Debug.Log("-------------");
    }
}
