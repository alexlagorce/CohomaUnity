using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;
using Polytech.CoHoMa.Core;

namespace Polytech.CoHoMa.ROS {

public class ObserverPublisherPosition : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "position";

    // The game object
    public GameObject cube;

    // Observer ID
    public int id = 1;

    // Publish the cube's position, rotation, latitude, and longitude every N seconds
    public float publishMessageFrequency = 5.0f;

    // The size of the map in Unity units
    public float x_size = 144.01f;

    public float z_size = 92.82f;

    // The actual size of the map in meters
    private float x_actual_size = 1440.1f;

    private float z_actual_size = 928.2f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    void Start()
    {
        // Start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PositionMsg>(topicName); // Assuming you have a new message type

    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            // Convert Unity coordinates to latitude and longitude using GPSEncoder
            SetGpsOrigin.SetLocalOrigin();
            Vector3 cubePosition = cube.transform.position;
            cubePosition.x = cubePosition.x * x_size / x_actual_size;
            cubePosition.z = cubePosition.z * z_size / z_actual_size;
            Vector3 gpsCoordinates = GPSEncoder.UCSToGPS(cubePosition);


            PositionMsg cubePos = new PositionMsg(
                id, // id
                gpsCoordinates.x, // latitude
                gpsCoordinates.y, // longitude
                cubePosition.y, // altitude 
                cubePosition.x, // x
                cubePosition.z // y
            );


            // Finally send the message to ROS
            ros.Publish(topicName, cubePos);

            timeElapsed = 0;
        }
    }
}

}