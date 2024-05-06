using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytech.CoHoMa.Visual 
{

public class DescriptionPoint3D : MonoBehaviour
{
    void Update()
    {
        // Faire toujours face à la caméra
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}

}
