using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    public int signalValue;
    void Start()
    {
        GameObject valueBar = transform.GetChild(1).gameObject;
        RectTransform valueBarRect = valueBar.GetComponent<RectTransform>();
        valueBarRect.sizeDelta = new Vector2(signalValue * 10, valueBarRect.sizeDelta.y);
    }
    
    public void setSignalLevel(int newSignalValue)
    {
        signalValue = newSignalValue;
        GameObject valueBar = transform.GetChild(1).gameObject;
        RectTransform valueBarRect = valueBar.GetComponent<RectTransform>();
        valueBarRect.sizeDelta = new Vector2(signalValue * 10, valueBarRect.sizeDelta.y);
    }
}
