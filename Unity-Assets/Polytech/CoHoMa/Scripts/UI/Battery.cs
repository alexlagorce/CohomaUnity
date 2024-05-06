using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Polytech.CoHoMa.UI {
public class Battery : MonoBehaviour
{
    public int batteryValue;
    //private int batteryLevelMaxPixel = 236;
    private int batteryLevelMaxPixel;
    private int leftPoint;
    private int rightPoint;
    
    private string greenColor = "#34c759";
    private string yellowColor = "#ffcc0a";
    private string redColor = "#ff3b30";
    void Start()
    {
        GameObject battery = transform.GetChild(0).GetChild(0).gameObject;
        
        leftPoint = (int)battery.GetComponent<RectTransform>().offsetMin.x;
        rightPoint = (int)battery.GetComponent<RectTransform>().offsetMax.x;
        
        int widthBatteryContainer = (int)transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        widthBatteryContainer = (widthBatteryContainer - leftPoint) + rightPoint;
        batteryLevelMaxPixel = widthBatteryContainer;
        
        setBatteryLevel(batteryValue);
    }
    
    public void setBatteryLevel(int batteryLevel)
    {
        this.batteryValue = batteryLevel;
        // set the width of the battery level image
        GameObject batteryLevelImage = transform.GetChild(0).GetChild(0).gameObject;
        RectTransform batteryLevelImageTransform = batteryLevelImage.GetComponent<RectTransform>();
        
        
        int pixelAdded = batteryLevelMaxPixel - (int)((batteryLevelMaxPixel * batteryLevel )/ 100);
        int newRightPoint = pixelAdded - rightPoint;
        
        ModifyLeftRight(batteryLevelImageTransform, leftPoint, newRightPoint);
        
        GameObject batteryLevelText = transform.GetChild(1).gameObject;
        batteryLevelText.GetComponent<Text>().text = batteryLevel + "%";
        
        setBatteryColor();
    }
    
    void ModifyLeftRight(RectTransform rectTransform, float leftValue, float rightValue)
    {
        // Obtenir les offsets actuels
        Vector2 offsetMin = rectTransform.offsetMin;
        Vector2 offsetMax = rectTransform.offsetMax;

        // Modifier les valeurs left et right
        offsetMin.x = leftValue; // Modifier le bord gauche
        offsetMax.x = -rightValue; // Modifier le bord droit (négatif pour le côté droit)

        // Appliquer les modifications aux offsets
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }
    
    public void setBatteryColor()
    {
        GameObject batteryLevelImage = transform.GetChild(0).GetChild(0).gameObject;
        Image batteryLevelImageColor = batteryLevelImage.GetComponent<Image>();
        
        if (batteryValue > 50)
        {
            Color color;
            ColorUtility.TryParseHtmlString(greenColor, out color);
            batteryLevelImageColor.color = color;
        }
        else if (batteryValue > 20)
        {
            Color color;
            ColorUtility.TryParseHtmlString(yellowColor, out color);
            batteryLevelImageColor.color = color;
        }
        else
        {
            Color color;
            ColorUtility.TryParseHtmlString(redColor, out color);
            batteryLevelImageColor.color = color;
        }
    }
}

}
