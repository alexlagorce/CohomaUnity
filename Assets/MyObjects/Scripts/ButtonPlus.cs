using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlus : MonoBehaviour
{
    private Camera mapCamera;
    void Start()
    {
        mapCamera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(onClick);
    }
    void Update()
    {
        if (mapCamera.orthographicSize <= 0)
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
        mapCamera.orthographicSize -= 1;
    }
}
