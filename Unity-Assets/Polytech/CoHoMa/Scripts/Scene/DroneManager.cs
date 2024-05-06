using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Polytech.CoHoMa.UI;

namespace Polytech.CoHoMa.Scene {
public class DroneManager : MonoBehaviour
{
    public List<PointDrone3D> drones = new List<PointDrone3D>();
    private List<GameObject> dronelist3D = new List<GameObject>();
    public GameObject ScrollDrones;
    GameObject pointDrone3D;
    private GameObject batteryUI;
    private GameObject signalUI;
    private ScrollDrones droneList;
    private PointDrone3D currentDrone;
    private Fullmap fullmap;
    
    void Start()
    {
        pointDrone3D = Resources.Load<GameObject>("Prefabs/UI/Components/PointDrone3D");
        batteryUI = GameObject.Find("BatteryUI");
        signalUI = GameObject.Find("SignalUI");
        //dronelist = GameObject.Find("drone list");
        fullmap = GameObject.Find("Fullmap").GetComponent<Fullmap>();
        CreateDrone("DA1", new Vector3(150, 89, 100), Color.blue,Random.Range(0,100),Random.Range(0,100));
        CreateDrone("DA2", new Vector3(70, 20, 30), Color.green,Random.Range(0,100),Random.Range(0,100));
        CreateDrone("DT1", new Vector3(-90, 50, 200), Color.red,Random.Range(0,100),Random.Range(0,100));
        droneList = ScrollDrones.GetComponentInChildren<ScrollDrones>();
        droneList.drones = drones;
        droneList.instantiateDrones();
        ScrollDrones.SetActive(false);
        
        fullmap.setDronesList(dronelist3D);
        fullmap.InitializeFullmap();
        fullmap.transform.gameObject.SetActive(false);
    }


    void CreateDrone(string name, Vector3 position, Color colorDrone, int battery, int signal)
    {
        GameObject pointDrone = Instantiate(pointDrone3D, transform);
        pointDrone.transform.parent = null;
        pointDrone.transform.position = new Vector3(position.x, 0, position.z);
        pointDrone.transform.GetComponent<PointDrone3D>().name = name;
        pointDrone.transform.GetComponent<PointDrone3D>().altitudeDrone = position.y;
        pointDrone.transform.GetComponent<PointDrone3D>().colorDrone = colorDrone;
        pointDrone.transform.GetComponent<PointDrone3D>().name = name;
        pointDrone.transform.GetComponent<PointDrone3D>().signal = signal;
        pointDrone.transform.GetComponent<PointDrone3D>().battery = battery;
        pointDrone.transform.GetComponent<PointDrone3D>().setDroneSelected(false);
        drones.Add(pointDrone.transform.GetComponent<PointDrone3D>());
        dronelist3D.Add(pointDrone);
    }
    
    void setUI(PointDrone3D drone)
    {
        batteryUI.GetComponent<Battery>().setBatteryLevel(drone.battery);
        signalUI.GetComponent<Signal>().setSignalLevel(drone.signal);
    }
    
    public void selectDrone(PointDrone3D drone)
    {
        if (currentDrone != null)
        {
            currentDrone.setDroneSelected(false);
        }
        currentDrone = drone;
        currentDrone.setDroneSelected(true);
        setUI(currentDrone);
    }

    public PointDrone3D GetCurrentDrone()
    {
        return currentDrone;
    }
}

}
