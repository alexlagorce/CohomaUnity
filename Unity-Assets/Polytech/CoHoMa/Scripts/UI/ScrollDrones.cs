using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;
using Polytech.CoHoMa.Scene;

namespace Polytech.CoHoMa.UI {

public class ScrollDrones : MonoBehaviour
{
    public List<PointDrone3D> drones;
    public List<Toggle> toggles;
    private GameObject togglePrefab;
    
    public void instantiateDrones()
    {
        togglePrefab = Resources.Load<GameObject>("Prefabs/UI/Components/Drone Toggle");
        foreach (PointDrone3D drone in drones)
        {
            AddToggle(drone);
        }
        transform.GetComponentsInChildren<Toggle>().ToList().ForEach(toggle => toggles.Add(toggle));
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate { onToggleClick(toggle); });
        }
        if (toggles.Count > 0)
        {
            toggles[0].isOn = true;
            toggles[0].interactable = false; // Désactiver l'interaction avec le premier toggle activé
        }
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(200, (togglePrefab.GetComponent<RectTransform>().rect.height + transform.GetComponent<VerticalLayoutGroup>().spacing)* drones.Count);
    }

    void AddToggle(PointDrone3D drone)
    {
        GameObject toggle = Instantiate(togglePrefab, transform);
        RectTransform toggleRect = toggle.GetComponent<RectTransform>();
        setRectToggle(toggle);
        setRectImage(toggle);
        toggle.transform.GetChild(0).Find("Text").GetComponent<Text>().text = drone.name;
        toggle.transform.name = drone.name;
        // scale toogle to 2
        toggle.transform.localScale = new Vector3(2, 2, 2);
        toggle.transform.GetComponentInChildren<Signal>().signalValue = drone.signal;
        toggle.transform.GetComponentInChildren<Battery>().batteryValue = drone.battery;
    }
    
    void setRectToggle(GameObject toggle)
    {
        // set anchor to middle center
        toggle.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        toggle.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        toggle.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        toggle.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);//Setting button position
        
    }
    void setRectImage(GameObject toggle)
    {
        
        RectTransform rt = toggle.transform.GetChild(0).GetComponent<RectTransform>();
        //rt.localPosition = new Vector3(-70, -15, 0);
    }
    
    void onToggleClick( Toggle toggle )
    {
        if (toggle.isOn)
        {
            toggle.interactable = false;
            toggles.ForEach(toggle2 => {
                if (toggle2 != toggle)
                {
                    toggle2.isOn = false;
                    toggle2.interactable = true;
                }
            });
            GameObject.Find("DroneManager").GetComponent<DroneManager>().selectDrone(drones.Find(drone => drone.name == toggle.name));
        }
    }
}

}
