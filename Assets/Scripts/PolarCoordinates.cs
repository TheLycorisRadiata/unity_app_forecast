using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    public float latitude, longitude;
    [SerializeField] private CartesianCoordinates cartesian;

    void Update()
    {
        latitude = cartesian.yCoordinate;
        longitude = cartesian.xCoordinate;

        // If you go north until you do a full 360
        if (cartesian.yCoordinate > 90f && cartesian.yCoordinate <= 270f)
            latitude = (cartesian.yCoordinate - 180f) * -1f;
        else if (cartesian.yCoordinate > 270f && cartesian.yCoordinate <= 360f)
            latitude = cartesian.yCoordinate - 360f;

        // If you go south until you do a full 360
        if (cartesian.yCoordinate < -90f && cartesian.yCoordinate >= -270f)
            latitude = cartesian.yCoordinate * -1f - 180f;
        else if (cartesian.yCoordinate < -270f && cartesian.yCoordinate >= -360f)
            latitude = cartesian.yCoordinate + 360f;

        // If you go east
        if (cartesian.xCoordinate > 180f && cartesian.xCoordinate <= 360f)
            longitude = cartesian.xCoordinate - 360f;
        // If you go west
        else if (cartesian.xCoordinate <= -180f && cartesian.xCoordinate >= -360f)
            longitude = cartesian.xCoordinate + 360f;

        //Debug.Log("Latitude: " + latitude + " / Longitude: " + longitude);
    }
}
