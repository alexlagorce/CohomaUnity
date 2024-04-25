using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altimeter : MonoBehaviour
{
    private GameObject TextAltimeter;
    private GameObject mainCamera;
    private Text text;
    
    void Start()
    {
        TextAltimeter = transform.GetChild(0).gameObject;
        mainCamera = GameObject.Find("Main Camera");
        text = TextAltimeter.GetComponent<Text>();
    }
    void Update()
    {
        if (mainCamera != null) text.text = "Altitude : " + mainCamera.GetComponent<Transform>().position.y.ToString("0.00") + " m";
    }
}