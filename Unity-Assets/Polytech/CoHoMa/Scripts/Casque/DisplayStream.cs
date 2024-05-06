using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text;
using UnityEngine.InputSystem;

namespace Polytech.CoHoMa.Casque {

// re�oit le flux de la vid�o � partir d'une URL de type: http://192.168.2.216:5000/video
// et qui l'applique � une texture pour l'afficher dans Unity

public class DisplayStream
{
    private readonly HttpClient client = new();
    private bool streamRunning = false;
    private bool streamStopped = false;
    private string streamURL;

    // il se peut que le serveur pour la vid�o mette du temps � se lancer et ne soit pas disponible tout de suite
    // il faut donc r�essayer plusieurs fois de s'y connecter
    private const int MaxRetries = 10;
    private const int RetryDelayMilliseconds = 5000;

    //private Material displayMaterial;
    private bool useMaterial = false;

    private Material displayMaterial = null;
    private RawImage displayRawImage = null;
    private Texture2D currentTexture; // texture de l'image
    private Texture2D FinalTexture; // texture finale avec les cadres noir autour des images
    private int halfImageW = 0; // moiti� de la largeur de l'image
    private int ImageH = 0; // hauteur de l'image

    private readonly InputActionReference X_button;
    private readonly InputActionReference Y_button;
    private readonly InputActionReference Grip_buttonL;
    private readonly InputActionReference Grip_buttonR;

    public DisplayStream(string Ip_Drone, int remotePort_, Material displayMaterial_, RawImage displayRawImage_, bool useMaterial_, InputActionReference X_button_, InputActionReference Y_button_, InputActionReference Grip_buttonL_, InputActionReference Grip_buttonR_)
    {
        streamURL = "http://" + Ip_Drone + ":"+ remotePort_;
        displayMaterial = displayMaterial_;
        displayRawImage = displayRawImage_;
        useMaterial = useMaterial_;
        X_button = X_button_;
        Y_button = Y_button_;
        Grip_buttonL = Grip_buttonL_;
        Grip_buttonR = Grip_buttonR_;
    }

    public void Start()
    {
        if (X_button == null)
        {
            Debug.LogError("X_button is not initialized");
            return;
        }

        if (Y_button == null)
        {
            Debug.LogError("Y_button is not initialized");
            return;
        }
        X_button.action.started += X_button_pressed;
        Y_button.action.started += Y_button_pressed;
        Grip_buttonL.action.started += Grip_buttonL_pressed;
        Grip_buttonR.action.started += Grip_buttonR_pressed;
        //await ReadStream();
    }

    private void OnDestroy()
    {
        streamRunning = false;
        client.Dispose();
    }
    public async Task ReadStream()
    {
        streamStopped = false;

        if (!streamRunning)
        {
            Debug.Log("Reading stream from URL: " + streamURL);
            client.Timeout = TimeSpan.FromMilliseconds(RetryDelayMilliseconds-1);
            int retryCount;
            for (retryCount = 0; retryCount < MaxRetries; retryCount++)
            {
                if (!Application.isPlaying) break;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(streamURL+"/video", HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();
                    Debug.Log("Stream reading successful");
                    streamRunning = true;

                    var content = response.Content;
                    var stream = await content.ReadAsStreamAsync();

                    byte[] imageBuffer = new byte[10000000]; // Taille maximale de l'image

                    while (streamRunning)
                    {
                        int imageLength = await ReadImageAsync(stream, imageBuffer);
                        if (imageLength > 0)
                        {
                            byte[] frame = new byte[imageLength]; 
/*                            if (frame.Length < imageLength)
                            {
                                // Si la taille du tableau frame est inf�rieure � la taille de l'image actuelle, r�allouez le tableau.
                                frame = new byte[imageLength];
                                Debug.Log("frame.Length < imageLength");
                            }*/
                            Array.Copy(imageBuffer, frame, imageLength);
                            DisplayImage(frame);
                        }
                    }
                    break;
                }
                catch (Exception e)
                {
                    if (streamStopped)
                    {
                        break;
                    }
                    Debug.LogError("Stream reading error: " + e.Message);
                    if (retryCount < MaxRetries)
                    {
                        Debug.Log($"Tentative de reconnexion {retryCount}/{MaxRetries} dans {RetryDelayMilliseconds}ms");
                        await Task.Delay(RetryDelayMilliseconds);
                        // = false
                    }
                    else
                    {
                        Debug.LogError("Nombre maximum de tentatives de connexion atteint.");
                    }
                }
            }
        }
    }

    public void StopStream()
    {
        streamRunning = false;
        streamStopped = true;
        Debug.Log("arr�t de l'affichage du stream");
    }

    private async Task<int> ReadImageAsync(Stream stream, byte[] buffer)
    {
        // Lire la ligne de la taille de l'image
        string sizeLine = await ReadLineAsync(stream);
        string sizePrefix = "Content-Length: ";
        if (!sizeLine.StartsWith(sizePrefix))
        {
            throw new InvalidOperationException("Invalid size line: " + sizeLine);
        }
        int imageSize = int.Parse(sizeLine.Substring(sizePrefix.Length));        

        // Lire l'image en fonction de la taille
        int totalBytesRead = 0;
        int bytesRead;
        while (totalBytesRead < imageSize)
        {
            bytesRead = await stream.ReadAsync(buffer, totalBytesRead, imageSize - totalBytesRead);
            if (bytesRead == 0)
            {
                throw new EndOfStreamException();
            }
            totalBytesRead += bytesRead;
        }

        // Consommer les s�parateurs entre les images
        await ReadLineAsync(stream); // lire /r/n
        await ReadLineAsync(stream); // lire /r/n

        return totalBytesRead;
    }
    private async Task<string> ReadLineAsync(Stream stream)
    {
        var line = new StringBuilder();
        var buffer = new byte[1];
        while (true)
        {
            await stream.ReadAsync(buffer, 0, 1);
            if (buffer[0] == '\n')
            {
                break;
            }
            if (buffer[0] != '\r')
            {
                line.Append((char)buffer[0]);
            }
        }
        return line.ToString();
    }

    /*    private void DisplayImage(byte[] imageData)
        {
            if (currentTexture == null)
            {
                currentTexture = new Texture2D(2, 2);
            }

            if (currentTexture.LoadImage(imageData))
            {
                if (useMaterial)
                {
                    if (oldTextureH != currentTexture.height || oldTextureW != currentTexture.width) // si la taille de l'image a chang�
                    {
                        oldTextureH = currentTexture.height;
                        oldTextureW = currentTexture.width;
                        borderedTexture = CreateTexture(currentTexture);
                    }

                    // Copier l'image originale au centre
                    // (largeur du cadre, hauteur du cadre, largeur de l'image, hauteur de l'image)
                    borderedTexture.SetPixels(currentTexture.width, currentTexture.height, currentTexture.width, currentTexture.height, currentTexture.GetPixels());
                    borderedTexture.Apply();

                    displayMaterial.mainTexture = borderedTexture;
                }
                else
                {
                    displayRawImage.texture = currentTexture;
                }
            }
            else
            {
                Debug.LogError("Error loading image data into texture.");
            }
        }*/

    private void DisplayImage(byte[] imageData)
    {
        if (currentTexture == null)
        {
            currentTexture = new Texture2D(2, 2);
        }

        if (currentTexture.LoadImage(imageData))
        {
            if (useMaterial) // s'il sagit d'une image st�r�o
            {
                if (ImageH != currentTexture.height || halfImageW != currentTexture.width/2) // si la taille de l'image a chang�
                {
                    ImageH = currentTexture.height;
                    halfImageW = currentTexture.width/2;

                    // l'angle des cam�ras est de 60� (on multiplie par 3 pour afficher sur 180�)
                    FinalTexture = new Texture2D(3 * 2 * halfImageW, ImageH * 3);
                    // Remplir la texture de noir
                    Color[] blackPixels = FinalTexture.GetPixels();
                    for (int i = 0; i < blackPixels.Length; i++)
                    {
                        blackPixels[i] = Color.black;
                    }
                    FinalTexture.SetPixels(blackPixels);
                }
                if (FinalTexture == null)
                {
                    // l'angle des cam�ras est de 60� (on multiplie par 3 pour afficher sur 180�)
                    FinalTexture = new Texture2D(3*2*halfImageW, ImageH*3);
                    // Remplir la texture de noir
                    Color[] blackPixels = FinalTexture.GetPixels();
                    for (int i = 0; i < blackPixels.Length; i++)
                    {
                        blackPixels[i] = Color.black;
                    }
                    FinalTexture.SetPixels(blackPixels);
                }

                // Copier les pixels des images st�r�o aux bons endroits dans la texture finale de mani�re � cr�er un cadre noir autour de chacune
                FinalTexture.SetPixels(halfImageW, ImageH, halfImageW, ImageH, currentTexture.GetPixels(0, 0, halfImageW, ImageH));
                FinalTexture.SetPixels(4*halfImageW, ImageH, halfImageW, ImageH, currentTexture.GetPixels(halfImageW, 0, halfImageW, ImageH));
                FinalTexture.Apply();

                displayMaterial.mainTexture = FinalTexture;
            }
            else // image mono
            {
                displayRawImage.texture = currentTexture;
            }
        }
        else
        {
            Debug.LogError("Error loading image data into texture.");
        }
    }

    // Fonction pour obtenir la moiti� de l'image st�r�oscopique
    private Texture2D GetHalfImage(Texture2D original, Texture2D halfImage, int startX)
    {
        halfImage.SetPixels(original.GetPixels(startX, 0, original.width / 2, original.height));
        halfImage.Apply();
        return halfImage;
    }

    // Fonction qui retourne la texture pour le cadre autour de l'image
    // en fonction de la texture de l'image pass� en param�tre
    private Texture2D CreateTexture(Texture2D texture)
    {
        // valable que si l'angle de la cam�ra est de 60�
        Texture2D newTexture = new Texture2D(texture.width*3, texture.height*3);

        // Remplir la texture de noir
        Color[] blackPixels = newTexture.GetPixels();
        for (int i = 0; i < blackPixels.Length; i++)
        {
            blackPixels[i] = Color.black;
        }
        newTexture.SetPixels(blackPixels);

        return newTexture;
    }


    public void SetURL(string Ip_Drone, int remotePort_)
    {
        streamURL = "http://" + Ip_Drone + ":" + remotePort_; 
    }

    private async void X_button_pressed(InputAction.CallbackContext obj)
    {
        if (streamRunning)
        { 
            try
            {
                HttpResponseMessage response = await client.GetAsync(streamURL + "/increase_Hoffset", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Debug.Log("Increase offset successful");
            }
            catch (Exception e)
            {
                Debug.LogError("Error increasing offset :" + e.Message);
            }
        }
    }
    private async void Y_button_pressed(InputAction.CallbackContext obj)
    {
        if (streamRunning)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(streamURL + "/decrease_Hoffset", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Debug.Log("Decrease offset successful");
            }
            catch (Exception e)
            {
                Debug.LogError("Error decreasing offset :" + e.Message);
            }
        }
    }
    private async void Grip_buttonL_pressed(InputAction.CallbackContext obj)
    {
        if (streamRunning)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(streamURL + "/ratioP", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Debug.Log("Increase ratio successful");
            }
            catch (Exception e)
            {
                Debug.LogError("Error increasing ratio :" + e.Message);
            }
        }
    }
    private async void Grip_buttonR_pressed(InputAction.CallbackContext obj)
    {
        if (streamRunning)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(streamURL + "/ratioM", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Debug.Log("Decrease ratio successful");
            }
            catch (Exception e)
            {
                Debug.LogError("Error decreasing ratio :" + e.Message);
            }
        }
    }
}

}