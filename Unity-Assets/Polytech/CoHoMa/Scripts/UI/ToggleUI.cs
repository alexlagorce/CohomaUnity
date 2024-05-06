using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Polytech.CoHoMa.UI {

public class ToggleUI : MonoBehaviour
{
    private GameObject toggleExterior;
    void Start()
    {
        if (transform.name == "Fullmap Toggle")
        {
            toggleExterior = GameObject.Find("ScrollDroneToggle");
        }
        else
        {
            toggleExterior = GameObject.Find("Fullmap Toggle");
        }
        transform.GetComponent<Toggle>().onValueChanged.AddListener(delegate { onTransformToggleClick(); });
        toggleExterior.GetComponent<Toggle>().onValueChanged.AddListener(delegate { onExteriorToggleClick(); });
    }
    
    void onTransformToggleClick()
    {
        if (transform.GetComponent<Toggle>().isOn)
        {
            toggleExterior.GetComponent<Toggle>().isOn = false;
        }
    }
    
    void onExteriorToggleClick()
    {
        if (toggleExterior.GetComponent<Toggle>().isOn)
        {
            transform.GetComponent<Toggle>().isOn = false;
        }
    }
}

}
