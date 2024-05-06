using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytech.CoHoMa.Scene {

public class FollowDrone : MonoBehaviour
{
    private Transform mainCamera;
    private float offsetX = 0f;
    private float offsetZ = 0f;
    private float LerpSpeed = 1.0f;
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(mainCamera.position.x + offsetX, 30f, mainCamera.position.z + offsetZ), LerpSpeed);
        
        Quaternion targetRotation = Quaternion.Euler(0f, mainCamera.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, LerpSpeed);

        // Appliquer la rotation fixe sur l'axe X à 90 degrés
        Vector3 fixedRotation = transform.eulerAngles;
        fixedRotation.x = 90f;
        transform.eulerAngles = fixedRotation;
    }
}

}