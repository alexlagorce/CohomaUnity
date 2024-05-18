using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using SpatialDataMsg = RosMessageTypes.Pol.SpatialDataMsg;
using SpatialPointMsg = RosMessageTypes.Pol.SpatialPointMsg;

public class RosObserverSubscriber : MonoBehaviour
{

    // Specify the topic name to subscribe to
    public string observerTopic = "observer_1/spatial_data";

    // The cube to spawn
    public  GameObject cube;

    // Start is called before the first frame update
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

        // Print observer values with labels to the console
        for(int i = 0; i < observerMessage.points.Length; i++)
        {
            Debug.Log($"Point {i}: ({observerMessage.points[i].x}, {observerMessage.points[i].y}, {observerMessage.points[i].z})");
            Debug.Log($"Color: ({observerMessage.points[i].color_r}, {observerMessage.points[i].color_g}, {observerMessage.points[i].color_b}, {observerMessage.points[i].color_a})");
            Debug.Log($"Timestamp: {observerMessage.points[i].timestamp}");
        }


        // Do something with observer data
        UpdateObserverPoints(observerMessage);
    }

    // Update is called once per frame
    void UpdateObserverPoints(SpatialDataMsg observerMessage)
    {
        for(int i = 0; i < observerMessage.points.Length; i++)
        {
            var tempCube = Instantiate(cube, new Vector3(observerMessage.points[i].x, observerMessage.points[i].y, observerMessage.points[i].z), Quaternion.identity);

            Renderer renderer = tempCube.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", new Color(observerMessage.points[i].color_r, observerMessage.points[i].color_g, observerMessage.points[i].color_b, observerMessage.points[i].color_a));
        }
    }
}
