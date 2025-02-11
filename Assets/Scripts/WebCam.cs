using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking; //UnityWebRequest
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WebCam : MonoBehaviour
{
    public string supabaseUrl = "https://egmlrswyxmjxuyegmwop.supabase.co"; // Replace with your Supabase URL
    public string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVnbWxyc3d5eG1qeHV5ZWdtd29wIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzg5MDUzOTIsImV4cCI6MjA1NDQ4MTM5Mn0.Z322fzLuy_thKkStEG1B6NKAKqs9lEjWc8vx7u02veM"; // Replace with your Supabase Anon Key
    public string bucketName = "profile-pictures"; // Replace with your bucket name

    //public Renderer targetRenderer; // The renderer of the 3D object to apply the webcam feed
    public RawImage targetImage;
    private WebCamTexture webcamTexture;

    private string screenshotsFolder = "/C:/Program Files/Unity/Hub/Editor/2022.3.24f1/Editor/WebCamScreenShots";

    //settings
    private const string WebCamScreenShotsFolder = "WebCamScreenShots";
    private const string UploadType = "image/png";
    private const string UploadFolder = "screenshots";
    public string profilePicURL;

    //private async void Start()
    public async void StartWebCam()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam detected!");
            return;
        }

        // Select the default webcam (or you can select by index)
        WebCamDevice webcamDevice = WebCamTexture.devices[0];
        Debug.Log($"Using webcam: {webcamDevice.name}");

        // Initialize the webcam texture
        webcamTexture = new WebCamTexture(webcamDevice.name, 640, 480, 30);

        RectTransform rt = targetImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 300);

        await Task.Delay(100);

        // Apply the webcam texture to the material of the target object
        if (targetImage != null)
        {
            webcamTexture.Play();
            //targetRenderer.material.mainTexture = webcamTexture;
            targetImage.texture = webcamTexture;
            targetImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);
            //targetImage.material.mainTextureScale = new Vector2(1, -1); // Flip horizontally
            targetImage.uvRect = new Rect(0, 0, 1, -1); // Flip texture properly
            Debug.Log($"Webcam Texture Applied: {webcamTexture.width}x{webcamTexture.height}");
        }
        else
        {
            Debug.LogError("Target renderer is not assigned!");
        }

        // Start the webcam feed
        //webcamTexture.Play();

        //Set up screenshots folder
        //this folder changes depending on OS platform (Win/Mac/iOS/Android) 
        //<where_your_Unity_app_resides>/WebcamScreenshots/
        screenshotsFolder = Path.Combine(Application.persistentDataPath, WebCamScreenShotsFolder);
        if (!Directory.Exists(screenshotsFolder))
        {
            Directory.CreateDirectory(screenshotsFolder);
            Debug.Log($"Created folder: {screenshotsFolder}");
        }
    }

    //private async void Update()
    public async void UploadPicture()
    {
        // Capture and upload screenshot on key press
        //if (Keyboard.current.cKey.wasPressedThisFrame)
        //{
        string fileName = $"Webcam_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string filePath = Path.Combine(screenshotsFolder, fileName);

        await CaptureWebcamFrameAsync(filePath);
        if (File.Exists(filePath))
        {
            await UploadFileUsingPost(filePath);
        }
        else
        {
            Debug.LogError("Failed to save the webcam frame.");
        }
        //}
    }

    private async Task CaptureWebcamFrameAsync(string filePath)
    {
        if (webcamTexture == null || !webcamTexture.isPlaying)
        {
            Debug.LogError("Webcam is not initialized or playing.");
            return;
        }

        try
        {
            // Create a Texture2D to capture the current frame
            Texture2D webcamFrame = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
            webcamFrame.SetPixels(webcamTexture.GetPixels());
            webcamFrame.Apply();

            // Save the Texture2D as a PNG file
            byte[] imageData = webcamFrame.EncodeToPNG();
            await File.WriteAllBytesAsync(filePath, imageData);

            Debug.Log($"Webcam frame captured and saved to: {filePath}");

            // Clean up memory
            Destroy(webcamFrame);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error capturing webcam frame: {ex.Message}");
        }
    }

    public async Task UploadFileUsingPost(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File does not exist: {filePath}");
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        //although we have public in the url path, it is excluded in this context
        //https://<yoursupabaseURL/storage/v1/object/<your_bucket>/<your_folder>/<specified_filename_to_save>
        profilePicURL = $"{supabaseUrl}/storage/v1/object/{bucketName}/{UploadFolder}/{fileName}";

        Debug.Log($"Uploading to URL: {profilePicURL}");

        try
        {

            // Create a multipart form to upload the file
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", fileData, fileName, UploadType);

            using (UnityWebRequest request = UnityWebRequest.Post(profilePicURL, form))
            {
                // Set required headers
                request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");

                // Send the request
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"File uploaded successfully \u2705: {fileName}");
                }
                else
                {
                    Debug.LogError($"Upload failed: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error uploading file: {ex.Message}");
        }
    }

    public void StopWebCam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
