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

        coordinates = raycastScript.RaycastPoint;
        coordinates.x += xRotationOffset;
        coordinates.y += yRotationOffset;

        // CHECK: Real latitude and longitude --> Nice (43.7, 7.2) and London (51.5, 0)
    }

    private Vector3 RotationOffsetRelativeToNullIsland()
    {
        /*
            Null Island is at the (0,0) polar coordinates.
            In the inspector, Null Island is centered at (0, -272.4, -2) but the euler angles are (0, 87.6, 358).
            The euler angles need to be used in script, as it's not possible to access the inspector rotation values.
            Also note that 360 is the same as 0, and vice versa.
        */

        Vector3 modelRotationForCenteredNullIsland = new Vector3(360f, 87.6f, 358f);
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

        return modelRotationForCenteredNullIsland - modelRotation;
    }
}
