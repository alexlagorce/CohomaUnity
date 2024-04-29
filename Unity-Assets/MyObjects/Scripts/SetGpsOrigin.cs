using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGpsOrigin : MonoBehaviour
{
    // Set local origin
    public static void SetLocalOrigin()
    {
        GPSEncoder.SetLocalOrigin(new Vector2(43.63316570247406f, 3.8650580476922713f));
    }
}
