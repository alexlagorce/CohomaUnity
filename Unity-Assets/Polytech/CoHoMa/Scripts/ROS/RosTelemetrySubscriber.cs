using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using TelemetryMsg = RosMessageTypes.UnityRoboticsDemo.TelemetryMsg;

namespace Polytech.CoHoMa.ROS {

public class RosTelemetrySubscriber : MonoBehaviour
{
    // Specify the topic name to subscribe to
    public string telemetryTopic = "telemetry";

    // The size of the map in Unity units
    public float x_size = 144.01f;

    public float z_size = 92.82f;

    // The actual size of the map in meters
    private float x_actual_size = 1440.1f;

    private float z_actual_size = 928.2f;

    // Start is called before the first frame update
    void Start()
    {
        // Register the subscriber with the specified topic and callback method
        ROSConnection.GetOrCreateInstance().Subscribe<TelemetryMsg>(telemetryTopic, TelemetryCallback);
    }

    // Callback method to handle incoming telemetry messages
    void TelemetryCallback(TelemetryMsg telemetryMessage)
    {
        // Print telemetry values with labels to the console
        Debug.Log($"Battery Percentage: {telemetryMessage.battery_percentage}");
        Debug.Log($"Battery Voltage: {telemetryMessage.battery_voltage}");
        Debug.Log($"Latitude: {telemetryMessage.latitude}");
        Debug.Log($"Longitude: {telemetryMessage.longitude}");
        Debug.Log($"Altitude: {telemetryMessage.altitude}");

        // Do something with telemetry data
        UpdateDrone(telemetryMessage);
    }

    // Update the drone GameObject with telemetry data
    void UpdateDrone(TelemetryMsg telemetry)
    {
        Vector3 position = GPSEncoder.GPSToUCS(telemetry.latitude, telemetry.longitude);
        position.x = position.x * x_actual_size / x_size;
        position.z = position.z * z_actual_size / z_size;
        position.y = telemetry.altitude;


        this.transform.position = position;
    }
}

}