using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using PositionMsg = RosMessageTypes.Pol.PositionMsg;
using Polytech.CoHoMa.Core;

namespace Polytech.CoHoMa.ROS.Publisher {

public class ObserverPublisherPosition : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "position";

    // The game object
    public GameObject observer;

    // Observer ID
    public int id = 1;

    // Publish the cube's position, rotation, latitude, and longitude every N seconds
    //public float publishMessageFrequency = 30f;

    // The size of the map in Unity units
    public float x_size = 144.01f;

    public float z_size = 92.82f;

    // The actual size of the map in meters
    private float x_actual_size = 1440.1f;

    private float z_actual_size = 928.2f;

    // Used to determine how much time has elapsed since the last message was published
    //private float timeElapsed = 0;

    // Store the last position to calculate movement
    private Vector3 lastPosition;
        
    // Minimum movement distance to trigger a publish (in meters)
    private const float minMovementDistance = 5.0f;

    void Start()
    {
        // Start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PositionMsg>(topicName); // Assuming you have a new message type

        // Initialize last position
        lastPosition = observer.transform.position;

    }

    private void Update()
    {
        //timeElapsed += Time.deltaTime;

        Vector3 currentPosition = observer.transform.position;
        float distance = Vector3.Distance(currentPosition, lastPosition);

        
        //if (timeElapsed > publishMessageFrequency)
        if (distance > minMovementDistance)
        {
            Debug.Log("Publishing message");

            // Convert Unity coordinates to latitude and longitude using GPSEncoder
            SetGpsOrigin.SetLocalOrigin();
            Vector3 observerPosition = observer.transform.position;
            observerPosition.x = observerPosition.x * x_size / x_actual_size;
            observerPosition.z = observerPosition.z * z_size / z_actual_size;
            Vector3 gpsCoordinates = GPSEncoder.UCSToGPS(observerPosition);


            PositionMsg observerPos = new PositionMsg(
                id, 
                gpsCoordinates.x, // latitude
                gpsCoordinates.y, // longitude
                observerPosition.y, // altitude 
                observerPosition.x, 
                observerPosition.z // y
            );


            // Finally send the message to ROS
            ros.Publish(topicName, observerPos);

            //timeElapsed = 0;

            lastPosition = currentPosition;
        }
    }
}

}