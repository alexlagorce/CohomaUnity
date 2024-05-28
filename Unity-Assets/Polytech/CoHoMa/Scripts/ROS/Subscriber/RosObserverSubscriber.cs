using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using SpatialDataMsg = RosMessageTypes.Pol.SpatialDataMsg;
using SpatialPointMsg = RosMessageTypes.Pol.SpatialPointMsg;

namespace Polytech.CoHoMa.ROS.Subscriber
{   
public class RosObserverSubscriber : MonoBehaviour
{

    // Specify the topic name to subscribe to
    public string observerTopic = "observer_1/spatial_data";

    // The cube to spawn
    public  GameObject cube;
    void Start()
    {
        // Register the subscriber with the specified topic and callback method
        ROSConnection.GetOrCreateInstance().Subscribe<SpatialDataMsg>(observerTopic, ObserverCallback);
    }

    // Callback method to handle incoming observer messages
    void ObserverCallback(SpatialDataMsg observerMessage)
    {

        // Print the number of points to the console
        Debug.Log($"Number of points: {observerMessage.points.Length}");

        UpdateObserverPoints(observerMessage);
    }

    void UpdateObserverPoints(SpatialDataMsg observerMessage)
    {
        for(int i = 0; i < observerMessage.points.Length; i++)
        {

            // Convert from cm to meters
            float x_in_meters = observerMessage.points[i].x / 100f;
            float y_in_meters = observerMessage.points[i].y / 100f;
            float z_in_meters = observerMessage.points[i].z / 100f;

            Vector3 position = new Vector3(x_in_meters, z_in_meters, y_in_meters);

            // Instantiate a cube at the specified position
            var tempCube = Instantiate(cube, position, Quaternion.identity);

            // Set the scale of the instantiated cube
            Vector3 cubeScale = new Vector3(0.05f, 0.05f, 0.05f);
            tempCube.transform.localScale = cubeScale;

            // Set the color of the instantiated cube
            Renderer renderer = tempCube.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", new Color(observerMessage.points[i].color_r, observerMessage.points[i].color_g, observerMessage.points[i].color_b, observerMessage.points[i].color_a));
        }
    }
}

}
