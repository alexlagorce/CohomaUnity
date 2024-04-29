using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text;

public class WebcamStream : MonoBehaviour
{
    public string streamURL; // URL de votre flux vidéo, par exemple "http://192.168.2.216:5000/video"
    /*    public RawImage display;
        private Texture2D texture;*/
    public Material displayMaterial;

    private HttpClient client = new HttpClient();
    private bool stop = false;

    async void Start()
    {      
        client.Timeout = TimeSpan.FromMilliseconds(5000);
        Debug.Log("Starting stream...");
        await ReadStream();
    }

    private void OnDestroy()
    {
        stop = true;
        client.Dispose();
    }

    private void InitTexture(byte[] imageData)
    {
        
    }
    private async Task ReadStream()
    {
        Debug.Log("Reading stream from URL: " + streamURL);
        try
        {
            HttpResponseMessage response = await client.GetAsync(streamURL, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var content = response.Content;
            var stream = await content.ReadAsStreamAsync();

            byte[] imageBuffer = new byte[10000000]; // Taille maximale de l'image
            int init = 0;
            Texture2D newTexture = new(2, 2);
            int startX = 0;
            int startY = 0;
            while (!stop)
            {
                int imageLength = await ReadImageAsync(stream, imageBuffer);
                if (imageLength > 0)
                {
                    byte[] frame = new byte[imageLength];
                    Array.Copy(imageBuffer, frame, imageLength);
                    if (init == 0)
                    {
                        Texture2D videoTexture = new Texture2D(2, 2);
                        if (videoTexture.LoadImage(frame))
                        {
                            int videoAngle = 120;
                            int newHeight = (int)((videoTexture.height * 180) / videoAngle);
                            int newWidth = (int)((videoTexture.width * 180) / videoAngle);
                            Debug.Log("newHeight, newWidth = " + newHeight + "," + newWidth);
                            // on ajoute des pixels noir autour de la vidéo pour pouvoir afficher la vidéo sur 120° (car le rendu est affiché obligatoirement sur 180°)
                            newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
                            Debug.Log("videoTexture.height, videoTexture.width = " + videoTexture.height + "," + videoTexture.width);
                            Debug.Log("newTexture.height, newTexture.width = " + newTexture.height + "," + newTexture.width);

                            // Calculer la position de départ pour copier l'image originale
                            startY = (int)((newHeight - videoTexture.height) / 2);
                            startX = (int)((newWidth - videoTexture.width) / 2);
                            Debug.Log("startX,startY = " + startX + "," + startY);
                            // Remplir la nouvelle texture avec des pixels noirs
                            Color[] blackPixels = newTexture.GetPixels();
                            for (int i = 0; i < blackPixels.Length; i++)
                            {
                                blackPixels[i] = Color.black;
                            }
                            newTexture.SetPixels(blackPixels);
                        }
                        init = 1;
                    }
                    //DisplayImage(frame, newTexture, startX, startY);
                    DisplayImage(frame);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Stream reading error: " + e.Message);
        }
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

        // Consommer les séparateurs entre les images
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
    /*    private void DisplayImage(byte[] imageData, Texture2D newTexture, int startX, int startY)
        {
            Texture2D videoTexture = new Texture2D(2, 2);
            if (videoTexture.LoadImage(imageData))
            {
                // Copier l'image originale dans la nouvelle texture
                for (int y = 0; y < videoTexture.height; y++)
                {
                    for (int x = 0; x < videoTexture.width; x++)
                    {
                        newTexture.SetPixel(startX + x, startY + y, videoTexture.GetPixel(x, y));
                    }
                }
                newTexture.Apply(); // Appliquer les changements à la texture
                displayMaterial.mainTexture = newTexture; // Utiliser la nouvelle texture dans le matériau
            }
            else
            {
                Debug.LogError("Error loading image data into texture.");
            }
        }*/
    private void DisplayImage(byte[] imageData)
    {
        Texture2D texture = new Texture2D(2, 2);
        // Chargement des données d'image dans la texture
        if (texture.LoadImage(imageData))
        {
            displayMaterial.mainTexture = texture;
            // This is a pseudo-code that gives you an idea of how to use the shader
            /*            Material mat = new Material(Shader.Find("Custom/VideoSurfaceShader"));
                        mat.mainTexture = texture;
                        displayMaterial = mat;*/
        }
        else
        {
            Debug.LogError("Error loading image data into texture.");
        }
    }

}