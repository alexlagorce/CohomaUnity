using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytech.CoHoMa.Visual
{

public class Line3D : MonoBehaviour
{
    public int xStart;
    public int zStart;
    public int xEnd;
    public int zEnd;
    private int height = 29;
    
    void Start()
    {
        GenerateLine();
    }

    void GenerateLine()
    {
        // Calcul des coordonnées du milieu
        float midX = (xStart + xEnd) / 2.0f;
        float midZ = (zStart + zEnd) / 2.0f;

        // Calcul de la longueur de la ligne
        float lineLength = Mathf.Sqrt(Mathf.Pow(xEnd - xStart, 2) + Mathf.Pow(zEnd - zStart, 2))/10.0f;
        
        // Calcul de l'angle de la ligne
        float angle = Mathf.Atan2(zEnd - zStart, xEnd - xStart) * 180 / Mathf.PI;
        
        // On applique les transformations
        transform.position = new Vector3(midX, height, midZ);
        transform.localScale = new Vector3(lineLength, 1, 0.5f);
        transform.rotation = Quaternion.Euler(0, -angle, 0);
        
    }
}

}