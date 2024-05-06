using UnityEngine;

namespace Polytech.CoHoMa.Casque {

public class ControllerMovement : MonoBehaviour
{
    public float speed = 10.0f; // Vitesse de d�placement du controller
    public float rotationSpeed = 1.0f; // Vitesse de rotation

    void Update()
    {
        // R�cup�rer les valeurs des axes du joystick pour le d�placement avant/arri�re
        float moveHorizontal = Input.GetAxis("Horizontal"); // Joystick gauche pour les c�t�s
        float moveVertical = Input.GetAxis("Vertical"); // Joystick gauche pour l'avant/arri�re

        // Calculer la direction de d�placement bas�e sur la rotation du controller
        Vector3 movementDirection = (transform.forward * moveVertical) + (transform.right * moveHorizontal);
        movementDirection = movementDirection.normalized; // Normaliser pour �viter des vitesses plus rapides en diagonale

        // Appliquer le mouvement en prenant en compte la rotation du controller
        transform.position += movementDirection * speed * Time.deltaTime;

        // Rotation avec le joystick droit (ajustez si n�cessaire selon votre configuration)
        float rotationHorizontal = Input.GetAxis("Right Stick Horizontal");
        float rotationVertical = Input.GetAxis("Right Stick Vertical");
        transform.Rotate(0, rotationHorizontal * rotationSpeed * Time.deltaTime, 0, Space.World);
        transform.Rotate(-rotationVertical * rotationSpeed * Time.deltaTime, 0, 0);
        Debug.Log("rotationHorizontal: " + rotationHorizontal);
    }
}

}