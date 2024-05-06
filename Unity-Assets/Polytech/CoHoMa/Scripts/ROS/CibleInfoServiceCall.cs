using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

namespace Polytech.CoHoMa.ROS {

public class CibleInfoServiceCall : MonoBehaviour
{
    ROSConnection ros;

    public string serviceName = "cible_info_service";

    public GameObject cible;

    // The size of the map in Unity units
    public float x_size = 144.01f;

    public float z_size = 92.82f;

    // The actual size of the map in meters
    private float x_actual_size = 1440.1f;

    private float z_actual_size = 928.2f;

    // Flag to ensure that the service is called only once
    private bool serviceCalled = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<CibleInfoRequest, CibleInfoResponse>(serviceName);
    }

    private void Update()
    {
        // Check if the service has been called already
        if (!serviceCalled)
        {
            // Obtain the latitude and longitude from the GameObject's position
            Vector3 ciblePosition = cible.transform.position;
            ciblePosition.x = ciblePosition.x * x_size / x_actual_size;
            ciblePosition.z = ciblePosition.z * z_size / z_actual_size;
            Vector2 gpsCoordinates = GPSEncoder.UCSToGPS(ciblePosition);

            float latitude = gpsCoordinates.x;
            float longitude = gpsCoordinates.y;

            Debug.Log($"Calling service with Latitude: {latitude}, Longitude: {longitude}");

            // Create a request with the current latitude and longitude
            CibleInfoRequest cibleInfoRequest = new CibleInfoRequest(latitude, longitude);

            // Send message to ROS and return the response
            ros.SendServiceMessage<CibleInfoResponse>(serviceName, cibleInfoRequest, Callback_ServiceResponse);

            // Set the flag to true to avoid calling the service again
            serviceCalled = true;
        }
    }

    void Callback_ServiceResponse(CibleInfoResponse response)
    {
        // Print the received information in the console
        Debug.Log($"Identite Cible: {response.identite}");
        Debug.Log($"Qrcode Cible: {response.qrcode}");
        Debug.Log($"Statut Cible: {response.statut}");
        Debug.Log($"Rayon de sécurité Cible : {response.rayon_secu}");
        Debug.Log($"Désactivation Cible : {response.desactivation}");
        Debug.Log($"Lima Cible : {response.lima}");
    }
}

}