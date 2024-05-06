using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Polytech.CoHoMa.Casque {

public class Pico_ButtonsInput : MonoBehaviour
{
    public InputActionReference X_button;
    public GameObject canvas1;
    public GameObject canvas2;

    // Start is called before the first frame update
    void Start()
    {
        X_button.action.started += X_button_pressed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void X_button_pressed(InputAction.CallbackContext obj)
    {
        // Si canvas n'est pas assign�, ne rien faire
        if ((canvas1 == null)||(canvas2==null)) return;

        // Basculer l'�tat actif du canvas
        canvas1.SetActive(!canvas1.activeSelf);
        canvas2.SetActive(!canvas2.activeSelf);
    }
}

}