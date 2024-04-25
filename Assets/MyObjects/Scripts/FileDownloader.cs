using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Dummiesman;

public class FileDownloader : MonoBehaviour
{
    public string server_ip = "172.20.10.3";

    private string apiUrl;
    private string downloadPath = "/maps";
    private HashSet<string> downloadedFiles = new HashSet<string>();
    private HttpClient httpClient;

    void Start()
    {
        apiUrl = "http://" + server_ip + ":8000/zip";
        httpClient = new HttpClient();
        InitDownloadedFiles();
        ImportDownloadedFiles();
        InvokeRepeating(nameof(CheckAndDownloadNewZips), 0f, 60f); // TODO: change this to receive a message from the server when a new zip is available
    }

    void Update()
    {

    }

    // Initializes the list of already downloaded files
    private void InitDownloadedFiles()
    {
        string directoryPath = Application.persistentDataPath + downloadPath;
        EnsureDirectoryExists(directoryPath);

        string[] directories = Directory.GetDirectories(directoryPath); // Get all directories in /maps
        foreach (string dir in directories) // Add all directories names to the list of downloaded files
        {
            downloadedFiles.Add(Path.GetFileName(dir));
        }
    }

    // Imports downloaded files into the game
    private void ImportDownloadedFiles()
    {
        string directoryPath = Application.persistentDataPath + downloadPath;
        string[] directories = Directory.GetDirectories(directoryPath);
        foreach (string dir in directories)
        {
            string objPath = Path.Combine(dir, Path.GetFileName(dir) + ".obj");
            LoadOBJ(objPath);
        }
    }

    // Checks if there are new zips to download and downloads them if so
    private async Task CheckAndDownloadNewZips()
    {
        try
        {
            string mapsDirectoryPath = Application.persistentDataPath + downloadPath;
            EnsureDirectoryExists(mapsDirectoryPath);

            string[] files = await GetDownloadableFiles();

            foreach (string file in files)
            {
                string correspondingFolder = file.Replace(".zip", "");
                if (!downloadedFiles.Contains(correspondingFolder))
                {
                    string fileUrl = $"{apiUrl}/{file}";
                    string localPath = Path.Combine(mapsDirectoryPath, file);

                    await DownloadFileFromUrl(fileUrl, localPath);

                    string extractPath = UnzipAndDelete(localPath); // => ....../maps/<file_name>/
                    if (extractPath == null)
                    {
                        Debug.LogError("Error in CheckAndDownloadNewZips: extractPath is null");
                        return;
                    }
                    downloadedFiles.Add(file.Replace(".zip", ""));
                    string objPath = Path.Combine(extractPath, file.Replace(".zip", ".obj")); // => ....../maps/<file_name>/<file_name>.obj
                    LoadOBJ(objPath);
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            Debug.LogError("HttpRequestException: " + httpEx.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    // Unzips a zip file to a folder and deletes the zip
    private string UnzipAndDelete(string filePath)
    {
        try
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extractPath = Path.Combine(Path.GetDirectoryName(filePath), fileNameWithoutExtension);

            EnsureDirectoryExists(extractPath);

            ZipFile.ExtractToDirectory(filePath, extractPath);
            File.Delete(filePath);
            return extractPath;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in UnzipAndDelete: " + ex.Message);
            return null;
        }
    }

    // Loads an .obj file into the game with its corresponding .mtl and .png files    
    private void LoadOBJ(string path)
    {
        if (File.Exists(path))
        {
            OBJLoader objLoader = new OBJLoader();
            string mtlPath = path.Replace(".obj", ".mtl");
            string coordinatesPath = path.Replace(".obj", ".txt");

            // Load coordinates from .txt file, latitude, longitude, altitude on each line
            string[] coordinates = File.ReadAllLines(coordinatesPath);
            double longitude = Double.Parse(coordinates[0], System.Globalization.CultureInfo.InvariantCulture);
            double latitude = Double.Parse(coordinates[1], System.Globalization.CultureInfo.InvariantCulture);
            double altitude = Double.Parse(coordinates[2], System.Globalization.CultureInfo.InvariantCulture);

            // Convert GPS coordinates to UCS coordinates while setting the local origin
            SetGpsOrigin.SetLocalOrigin();
            Vector3 ucsCoordinates = GPSEncoder.GPSToUCS(latitude, longitude);

            ucsCoordinates.y = (float)altitude; // TODO: check if this is needed

            // Load .obj file and its .mtl file
            GameObject obj = objLoader.Load(path, mtlPath);
            obj.transform.localScale = new Vector3(10f, 10f, 10f);
            obj.transform.position = ucsCoordinates;

            // Load .png file and apply it to the object
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(path.Replace(".obj", ".png")));
            obj.GetComponentInChildren<Renderer>().material.mainTexture = texture;
            obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponentInChildren<MeshFilter>().mesh;


        }
        else
        {
            Debug.LogError("OBJ file not found at " + path);
        }
    }

    // Creates a directory if it doesn't exist
    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    // Gets the list of downloadable files from the server
    private async Task<string[]> GetDownloadableFiles()
    {
        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        FileList fileList = JsonUtility.FromJson<FileList>("{\"files\":" + json + "}");
        return fileList.files;
    }

    // Downloads a zip file from a url to a local path
    private async Task DownloadFileFromUrl(string url, string localPath)
    {
        using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            HttpResponseMessage fileResponse = await httpClient.GetAsync(url);
            fileResponse.EnsureSuccessStatusCode();
            await fileResponse.Content.CopyToAsync(fileStream);
        }
    }
}

[Serializable]
public class FileList
{
    public string[] files;
}