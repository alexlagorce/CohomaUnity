// G�re le menu 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Polytech.CoHoMa.Scene {

public class MenuManager : MonoBehaviour
{
    private string sceneName;
    private PointDrone3D currentDrone; // drone sélectionné

    public void changeScene()
    {
        try
        {
            currentDrone = GameObject.Find("DroneManager").GetComponent<DroneManager>().GetCurrentDrone();
            sceneName = currentDrone.name;
            Debug.Log("sceneName = "+sceneName);
        }
        catch
        {
            // si le programme ne trouve pas le composant DroneManager, il est déjà dans la scène camera d'un drone. 
            // Le bouton doit donc renvoyer vers la map
            sceneName = "Map";
            Debug.Log("sceneName = "+sceneName);
        }
        SceneManager.LoadScene(sceneName);
    }
}

}