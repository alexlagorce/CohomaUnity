using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class DronesUDP : MonoBehaviour
{
    private UdpClient udpClient = new UdpClient();
    public int remotePort = 5001;
    private IPEndPoint remoteEndPoint;
    private string Ip_Drone; // = "192.168.10.63"; // IP de la raspberry du drone cible, (l'utilisateur peut le changer)
    private bool streamOn = false;
    
    private DisplayStream displayStream;
    //public Material displayMaterial;

    [SerializeField] private bool useMaterial = false;
    [SerializeField] private Material displayMaterial = null;
    [SerializeField] private RawImage displayRawImage = null;
    [SerializeField] private TMP_InputField IpInputField;

    [SerializeField] private InputActionReference A_button;
    [SerializeField] private InputActionReference B_button;
    [SerializeField] private InputActionReference Grip_buttonL;
    [SerializeField] private InputActionReference Grip_buttonR;


    private void Start()
    {
        Ip_Drone = IpInputField.text;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(Ip_Drone), remotePort);

        displayStream = new DisplayStream(Ip_Drone, remotePort, displayMaterial, displayRawImage, useMaterial, A_button, B_button, Grip_buttonL, Grip_buttonR);
        displayStream.Start();
    }
    async public void StartStream()
    {
        Debug.Log("streamOn = "+ streamOn);
        if (streamOn == false) {
            streamOn = true;
            Debug.Log("Demande de lancement du stream envoyé");
            byte[] message = BitConverter.GetBytes(1);
            udpClient.Send(message, message.Length, remoteEndPoint);

            await displayStream.ReadStream();
        }
    }

    public void StopStream()
    {
        streamOn = false;
        displayStream.StopStream();
        Debug.Log("Demande d'arrêt du stream envoyé");
        byte[] message = BitConverter.GetBytes(2);
        udpClient.Send(message, message.Length, remoteEndPoint);
    }

    public void ReadStringInput(string txt)
    {
        // Regex pour l'adresse IP version 4
        string patternIPv4 = @"^([0-9]{1,3}\.){3}[0-9]{1,3}$";

        if (Regex.IsMatch(txt, patternIPv4))
        {
            Ip_Drone = txt;
            Debug.Log(Ip_Drone);
            displayStream.SetURL(Ip_Drone, remotePort);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(Ip_Drone), remotePort);
        }
        else
        {
            Debug.Log("le texte n'est pas une IP");
        }
    }
}
