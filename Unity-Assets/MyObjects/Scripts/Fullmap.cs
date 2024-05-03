using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class Fullmap : MonoBehaviour
{
    public InputActionReference A_button;
    public InputActionReference Trigger_button;
    public InputActionReference Grip_button;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    
    private GameObject PointTarget3D;
    private GameObject Line3D;
    private GameObject Map2D;
    private GameObject currentDrone;
    private List<GameObject> dronelist3D = new List<GameObject>(); 
    private GameObject[] drones;
    private GameObject[] dronesTarget;
    private GameObject[] dronesLine;
    public void InitializeFullmap()
    {
        PointTarget3D = Resources.Load<GameObject>("Prefabs/UI/Components/PointTarget3D");
        Line3D = Resources.Load<GameObject>("Prefabs/UI/Components/Line3D");
        Map2D = GameObject.Find("Map2D");
        // drones = FindObjectsWithName("PointDrone3D");
        drones = dronelist3D.ToArray();
        dronesTarget = new GameObject[drones.Length];
        dronesLine = new GameObject[drones.Length];
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.B) || A_button.action.triggered) && currentDrone != null)
        {
            HandleAddPoint();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || Trigger_button.action.triggered)
        {
            HandleSelectDrone();
        }

        if ((Input.GetKeyDown(KeyCode.Mouse1) || Grip_button.action.triggered) && currentDrone != null)
        {
            HandleRemoveDronePoint();
        }
    }

    void HandleAddPoint()
    {
        RaycastHit hit;
        bool isHitting = rayInteractor.TryGetCurrent3DRaycastHit(out hit);

        if (isHitting)
        {
            Vector3[] cornersMinimap = GetPlaneCorners(transform.gameObject);
            Vector3[] cornersReal = GetPlaneCorners(Map2D);
            Vector3 point = hit.point;

            Vector3 pointReal = ConvertCoordonates(point, cornersMinimap, cornersReal);
            //pointReal.y += 5;

            GameObject newPoint = Instantiate(PointTarget3D, pointReal, Quaternion.identity);
            HandleLineCreation(newPoint, pointReal);
        }
    }

    void HandleSelectDrone()
    {
        RaycastHit hit;
        bool isHitting = rayInteractor.TryGetCurrent3DRaycastHit(out hit);

        if (isHitting)
        {
            Debug.Log("Hit object: " + hit.transform.gameObject.name);
            Vector3[] cornersMinimap = GetPlaneCorners(transform.gameObject);
            Vector3[] cornersReal = GetPlaneCorners(Map2D);
            Vector3 pointDel = hit.point;
            Vector3 pointReal = ConvertCoordonates(pointDel, cornersMinimap, cornersReal);

            currentDrone = null;

            foreach (GameObject drone in drones)
            {
                drone.transform.GetChild(2).gameObject.SetActive(false);

                if (Vector3.Distance(drone.transform.position, pointReal) < 30)
                {
                    drone.transform.GetChild(2).gameObject.SetActive(true);
                    currentDrone = drone;
                }
            }
        }
    }

    void HandleRemoveDronePoint()
    {
        int index = System.Array.IndexOf(drones, currentDrone);

        Destroy(dronesTarget[index]);
        Destroy(dronesLine[index]);
    }

    void HandleLineCreation(GameObject newPoint, Vector3 pointReal)
    {
        int index = System.Array.IndexOf(drones, currentDrone);

        if (dronesTarget[index] != null)
        {
            Destroy(dronesTarget[index]);
            Destroy(dronesLine[index]);
        }

        GameObject line = AddLine(currentDrone.transform.position, pointReal);
        newPoint.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", currentDrone.GetComponent<PointDrone3D>().colorDrone);
        newPoint.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", currentDrone.GetComponent<PointDrone3D>().colorDrone);
        //newPoint.GetComponent<MeshRenderer>().material.SetColor("_Color", currentDrone.GetComponent<PointDrone3D>().colorDrone);

        dronesTarget[index] = newPoint;
        dronesLine[index] = line;
    }

    Vector3[] GetPlaneCorners(GameObject plane)
    {
        Vector3[] corners = new Vector3[4]
        {
            new Vector3(5, 0, 5),
            new Vector3(-5, 0, 5),
            new Vector3(-5, 0, -5),
            new Vector3(5, 0, -5)
        };

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = plane.transform.TransformPoint(corners[i]);
        }

        return corners;
    }

    Vector3 ConvertCoordonates(Vector3 point, Vector3[] cornersMinimap, Vector3[] cornersReal)
    {
        Vector3 origin = cornersMinimap[0];
        Vector3 x = cornersMinimap[1] - cornersMinimap[0];
        Vector3 y = cornersMinimap[3] - cornersMinimap[0];

        float xMinimap = Vector3.Dot(point - origin, x) / Vector3.Dot(x, x);
        float yMinimap = Vector3.Dot(point - origin, y) / Vector3.Dot(y, y);

        Vector3 originReal = cornersReal[0];
        Vector3 xReal = cornersReal[1] - cornersReal[0];
        Vector3 yReal = cornersReal[3] - cornersReal[0];

        float xRealPoint = xMinimap * xReal.x + yMinimap * yReal.x;
        float yRealPoint = xMinimap * xReal.y + yMinimap * yReal.y;
        float zRealPoint = xMinimap * xReal.z + yMinimap * yReal.z;

        xRealPoint += originReal.x;
        yRealPoint += originReal.y;
        zRealPoint += originReal.z;

        return new Vector3(xRealPoint, yRealPoint, zRealPoint);
    }

    GameObject AddLine(Vector3 point1, Vector3 point2)
    {
        GameObject newLine = Instantiate(Line3D, point1, Quaternion.identity);
        newLine.GetComponent<Line3D>().xStart = (int)point1.x;
        newLine.GetComponent<Line3D>().zStart = (int)point1.z;
        newLine.GetComponent<Line3D>().xEnd = (int)point2.x;
        newLine.GetComponent<Line3D>().zEnd = (int)point2.z;

        newLine.transform.position = new Vector3(newLine.transform.position.x, 29, newLine.transform.position.z);

        newLine.GetComponent<MeshRenderer>().material.SetColor("_Color", currentDrone.GetComponent<PointDrone3D>().colorDrone);

        return newLine;
    }

    GameObject[] FindObjectsWithName(string objectName)
    {
        Object[] allObjects = GameObject.FindObjectsOfTypeAll(typeof(GameObject));
        System.Collections.Generic.List<GameObject> matchingObjects = new System.Collections.Generic.List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            Debug.Log("Object name: " + obj.name);
            if (obj.name.Contains(objectName))
            {
                matchingObjects.Add(obj);
            }
        }

        return matchingObjects.ToArray();
    }
    
    public void setDronesList(List<GameObject> drones)
    {
        this.dronelist3D = drones;
    }
}
