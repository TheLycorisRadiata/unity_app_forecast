using UnityEngine;

public class CartesianCoordinates : MonoBehaviour
{
    public Vector3 coordinates;
    private Vector3 modelRotationOffset;

    [SerializeField] private WebglRaycast raycastScript;
    [SerializeField] private RotateHorizontally horizontalRotationScript;
    [SerializeField] private RotateVertically verticalRotationScript;
 
    void Start()
    {
        modelRotationOffset = RotationOffsetRelativeToNullIsland();
    }

    void Update()
    {
        float xRotationOffset = horizontalRotationScript.pitch - modelRotationOffset.y;
        float yRotationOffset = modelRotationOffset.z - verticalRotationScript.yaw;

        // Apply the rotation offset to the cartesian coordinates
        coordinates = raycastScript.RaycastPoint;
        coordinates.x += xRotationOffset;
        coordinates.y += yRotationOffset;

        // Because of this, the coordinates may be outside of the -360/+360 range
        // If that's so, add or remove 360 so that "360" is considered as "0" (e.g. 367 becomes 7)
        if (coordinates.x > 360f)
            coordinates.x -= 360f;
        else if (coordinates.x < -360f)
            coordinates.x += 360f;

        if (coordinates.y > 360f)
            coordinates.y -= 360f;
        else if (coordinates.y < -360f)
            coordinates.y += 360f;
    }

    private Vector3 RotationOffsetRelativeToNullIsland()
    {
        /*
            Null Island is rotated at (0, -270.5, 1.5) in the inspector, 
            and in euler angles it's (0, 89.5, 1.5).
        */

        Vector3 modelRotationForCenteredNullIsland = new Vector3(360f, -270.5f, 1.5f);
        Vector3 modelRotation = transform.eulerAngles;
        Vector3 offset = Vector3.zero;

        // unused in our context
        if (modelRotation.x == 0f)
            modelRotation.x = 360f;

        // horizontal rotation
        if (modelRotation.y == 0f)
            modelRotation.y = 360f;

        // vertical rotation
        if (modelRotation.z == 0f)
            modelRotation.z = 360f;

        offset = modelRotationForCenteredNullIsland - modelRotation;

        if (offset.x < 0)
            offset.x += 360f;
        if (offset.y < 0)
            offset.y += 360f;
        if (offset.z < 0)
            offset.z += 360f;

        return offset;
    }
}
