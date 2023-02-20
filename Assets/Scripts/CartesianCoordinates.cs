using UnityEngine;

public class CartesianCoordinates : MonoBehaviour
{
    public float xCoordinate, yCoordinate;
    [SerializeField] private WebglRaycast raycastScript;
    [SerializeField] private RotateHorizontally horizontalRotationScript;
    [SerializeField] private RotateVertically verticalRotationScript;

    private float zRaycastCenter = -47.434f;
    /*
        The Z raycast value when sampled from the center of the sphere seems to be -47.434.
        I couldn't find the way to save the value automatically, and resorted to run manual tests to find it.
        This value is used to normalize the current Z raycast value, so it's 0 on the center and a positive value everywhere else.
    */

    private Vector3 modelRotationOffset;
    /*
        On the actual Earth, Null Island is on the 0x0 coordinates.
        The Earth model needs to be rotated a certain way for its Null Island to be centered.
        This vector3 is used as an offset for the cartesian coordinates computation.
    */

    void Start()
    {
        /*
            In the inspector, Null Island is centered at (0, -270, -1) but the euler angles are (0, 90, 359).
            The euler angles need to be used in script, as it's not possible to access the inspector rotation values.
            Also note that 360 is the same as 0, and vice versa.
        */

        Vector3 modelRotationForCenteredNullIsland = new Vector3(360f, 90f, 359f);
        Vector3 modelRotation = transform.eulerAngles;

        if (modelRotation.x == 0f)
            modelRotation.x = 360f;
        if (modelRotation.y == 0f)
            modelRotation.y = 360f;
        if (modelRotation.z == 0f)
            modelRotation.z = 360f;

        modelRotationOffset = modelRotationForCenteredNullIsland - modelRotation;
    }

    void Update()
    {
        /*
            TODO
            - [X] Get the raycast's vector3, the horizontal rotation (pitch) and the vertical rotation (yaw).
            - [X] Instead of using the raycast vector3 as is, add an offset with the earth's rotations.
            - [X] Rotation scripts: Reset the rotations to 0 if they are outside of -360/360.
            - [X] Convert the cartesian Y coordinate to the latitude equivalent.
            - [X] Convert the cartesian X coordinate to the longitude equivalent.

            NOTE: The initial Earth rotation ("Model" gameobject) has its importance and is currently (0, -250, 0).
            No matter the initial rotation, null island needs to be the 0x0 coordinates, it's around Africa.
            Once the horizontal and vertical rotations have been added to the cartesian coordinates as an offset (which is done),
            another offset needs to be added so that the coordinates are relative to null island.
            - [ ] Take the Earth default rotation into account, so that any chosen default rotation doesn't break the coordinates.

            NOTE: Only xRaycast and yRaycast have been used so far. This means that the coordinates are only correct when the cursor is right on the middle of the sphere.
            This can be fixed by adding zRaycast as an offset, but keep in mind that zRaycast changes depending on the zoom (camera position on the Z axis).
            - [ ] Add the Earth curvature to the cartesian coordinates computation.
        */

        float xRotation = horizontalRotationScript.pitch;
        float yRotation = -verticalRotationScript.yaw;

        float xRaycast = raycastScript.RaycastPoint.x;
        float yRaycast = raycastScript.RaycastPoint.y;
        float zRaycast = raycastScript.RaycastPoint.z - zRaycastCenter;

        xCoordinate = xRaycast + xRotation;
        yCoordinate = yRaycast + yRotation;

        /*
            CURVATURE PROBLEM
            - zRaycast is 0 on the center, otherwise its value changes depending on the zoom.
            - At default zoom it's a number between 0 and ~25.

            - Another observation is that around ~20 is added to the latitude when its sampled from the horizon instead of 0x0.
            - I haven't checked for the longitude.
            - This is for the default zoom, the observed offset may be different from ~20 with another zoom.

            - CALCULUS: latitude - zRaycast --> Correct latitude?
        */

        //Debug.Log("[Model rotation offset] " + modelRotationOffset);
        //Debug.Log("[Rotation] X: " + xRotation + " / Y: " + yRotation);
        //Debug.Log("[Raycast] X (no offset): " + xRaycast + " / X (with rotation offset): " + xCoordinate);
        //Debug.Log("[Raycast] Y (no offset): " + yRaycast + " / Y (with rotation offset): " + yCoordinate);
        //Debug.Log("[Raycast] Z: " + zRaycast);
        //Debug.Log("-------------");
    }
}
