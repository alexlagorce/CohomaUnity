using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SwitchDrone : MonoBehaviour
{
   public GameObject togglePrefab;
    public int height = 40;
    public int width = 160;
    public List<string> drones = new List<string>();
    public List<Toggle> toggles;
    void Start()
    {
        this.drones.Add("Drone");
        this.drones.Add("Drone2");
        this.drones.Add("Drone3");
        this.drones.Add("Drone4");
        this.drones.Add("Drone5");
        this.drones.Add("Drone6");
        this.drones.Add("Drone7");
        this.drones.Add("Drone8");
        foreach (string drone in this.drones)
        {
            AddToggle(drone);
        }
        // set first toggle to true
        
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
    }

    void AddToggle(string toggleText)
    {
        GameObject toggle = Instantiate(togglePrefab, transform); 
        RectTransform toggleRect = toggle.GetComponent<RectTransform>();
        setRectToggle(toggle);
        setRectImage(toggle);
        toggle.transform.GetChild(0).Find("Text").GetComponent<Text>().text = toggleText; 
        toggle.transform.name = toggleText;
        //toggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { onToggleClick(toggle.GetComponent<Toggle>()); });
        
        // set signal level
        toggle.transform.GetComponentInChildren<Signal>().signalValue = Random.Range(0, 100);
        // set battery level
        toggle.transform.GetComponentInChildren<Battery>().batteryValue = Random.Range(0, 100);
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
        Debug.Log("clicked!");
        if (toggle.isOn)
        {
            toggle.interactable = false;
            Debug.Log("toggle number " + toggle.name + " is on");
            toggles.ForEach(toggle2 => {
                if (toggle2 != toggle)
                {
                    toggle2.isOn = false;
                    toggle2.interactable = true;
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
