using System;
using UnityEngine;

public class CartesianCoordinates : MonoBehaviour
{
    public float xCoordinate, yCoordinate;
    [SerializeField] private WebglRaycast raycastScript;
    [SerializeField] private RotateHorizontally horizontalRotationScript;
    [SerializeField] private RotateVertically verticalRotationScript;
    private Vector3 modelRotationOffset;
    /*
        The Z raycast value when sampled from the center of the sphere seems to be -47.434.
        I couldn't find the way to save the value automatically, and resorted to run manual tests to find it.
        Note that the absolute value is used, hence the multiplication by -1.
    */
    private float zRaycastCenter = -47.434f * -1f;

    void Start()
    {
        /*
            Null Island is at the 0x0 polar coordinates.
            In the inspector, Null Island is centered at (0, -270, -1) but the euler angles are (0, 90, 359).
            The euler angles need to be used in script, as it's not possible to access the inspector rotation values.
            Also note that 360 is the same as 0, and vice versa.
        */

        Vector3 modelRotationForCenteredNullIsland = new Vector3(360f, 90f, 359f);
        Vector3 modelRotation = transform.eulerAngles;

        // unused in our context
        if (modelRotation.x == 0f)
            modelRotation.x = 360f;
        // horizontal rotation
        if (modelRotation.y == 0f)
            modelRotation.y = 360f;
        // vertical rotation
        if (modelRotation.z == 0f)
            modelRotation.z = 360f;

        modelRotationOffset = modelRotationForCenteredNullIsland - modelRotation;
    }

    void Update()
    {
        /*
            TODO

            Only xRaycast and yRaycast have been used so far. This means that the coordinates are only correct when the cursor is right on the middle of the sphere.
            This can be fixed by adding zRaycast as an offset.
            - [ ] Add the Earth curvature to the cartesian coordinates computation.
        */

        float xRaycast = raycastScript.RaycastPoint.x;
        float yRaycast = raycastScript.RaycastPoint.y;
        float zRaycast = Math.Abs(raycastScript.RaycastPoint.z);

        float xRotationOffset = horizontalRotationScript.pitch;
        float yRotationOffset = -verticalRotationScript.yaw;

        xCoordinate = xRaycast + xRotationOffset - modelRotationOffset.y;
        yCoordinate = yRaycast + yRotationOffset + modelRotationOffset.z;

        /*
            CURVATURE PROBLEM
            - Real latitude and longitude: Nice (43.7, 7.2) and London (51.5, 0).
        */

        // The inclination and the azimuth are degrees
        float radius = zRaycastCenter;
        float distanceToSphere = radius - zRaycast;
        float inclination = (float)Math.Acos(distanceToSphere / radius);
        float azimuth = (float)Math.Atan2(yRaycast, xRaycast); // maybe it should be yCoordinate and xCoordinate instead
    }
}
