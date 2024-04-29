using System;
using UnityEngine;
public class FullmapCamera : MonoBehaviour
{
    GameObject TeleportMap; // Objet cible
    float heightAboveObject = 201f; // Hauteur au-dessus de l'objet

    void Start()
    {
        TeleportMap = GameObject.Find("Teleport Map");
        
        if (TeleportMap != null)
        {
            // Position de la caméra au centre de l'objet sur les axes X et Y
            Vector3 targetPos = TeleportMap.transform.position;
            Vector3 cameraPos = new Vector3(targetPos.x, 0f, targetPos.z);

            // Ajouter la hauteur au-dessus de l'objet sur l'axe Z
            cameraPos.y += heightAboveObject;

            // Positionner la caméra
            transform.position = cameraPos;

            // Faire en sorte que la caméra regarde vers le bas
            transform.rotation = Quaternion.LookRotation(Vector3.down);
            
            // tourner la caméra sur elle même de 180°
            transform.Rotate(180, 180, 0, Space.Self);    
        }
        else
        {
            Debug.LogWarning("Aucun objet cible défini pour la caméra.");
        }
    }

    void Update()
    {
        // si la caméra a changé de position, on la replace au centre de l'objet
        if (transform.position != TeleportMap.transform.position)
        {
            // Position de la caméra au centre de l'objet sur les axes X et Y
            Vector3 targetPos = TeleportMap.transform.position;
            Vector3 cameraPos = new Vector3(targetPos.x, 0f, targetPos.z);

            // Ajouter la hauteur au-dessus de l'objet sur l'axe Z
            cameraPos.y += heightAboveObject;

            // Positionner la caméra
            transform.position = cameraPos;

            // Faire en sorte que la caméra regarde vers le bas
            transform.rotation = Quaternion.LookRotation(Vector3.down);
            
            // tourner la caméra sur elle même de 180°
            //transform.Rotate(180, 180, 0, Space.Self);   
            transform.Rotate(180, 180, 180, Space.Self);    
            
        }
    }
}
