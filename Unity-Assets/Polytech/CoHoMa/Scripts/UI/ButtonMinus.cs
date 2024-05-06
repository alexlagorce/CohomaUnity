using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytech.CoHoMa.UI {

public class ButtonMinus : MonoBehaviour
{
    private Camera mapCamera;
    void Start()
    {
        mapCamera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClick);
    }
    void Update()
    {
        if (mapCamera.orthographicSize >= 20)
        {
            transform.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            transform.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
    }
    public void onClick()
    {
        mapCamera.orthographicSize += 1;
    }
}

}