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
    public RenderTexture renderTexture;
    private WebCamTexture webcamTexture;

    private string screenshotsFolder = "/C:/Program Files/Unity/Hub/Editor/2022.3.24f1/Editor/WebCamScreenShots";

    //settings
    private const string WebCamScreenShotsFolder = "WebCamScreenShots";
    private const string UploadType = "image/png";
    private const string UploadFolder = "screenshots";
    public string profilePicURL;

    public async void StartWebCam()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam detected!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        string selectedCameraName = ""; // Store selected webcam name

        // Try to find a front-facing camera
        foreach (var device in devices)
        {
            if (device.isFrontFacing)
            {
                selectedCameraName = device.name;
                break; // Use the first front-facing camera found
            }
        }

        // If no front camera found, use the first available camera
        if (string.IsNullOrEmpty(selectedCameraName) && devices.Length > 0)
        {
            selectedCameraName = devices[0].name;
        }

        // Ensure a valid camera is selected
        if (string.IsNullOrEmpty(selectedCameraName))
        {
            Debug.LogError("No available camera found!");
            return;
        }

        // Initialize WebCamTexture with the selected camera
        webcamTexture = new WebCamTexture(selectedCameraName, renderTexture.width, renderTexture.height);
        webcamTexture.Play();
        Debug.Log($"Using webcam: {selectedCameraName}");

        // If a UI RawImage is used, assign it (Optional)
        if (targetImage != null)
        {
            targetImage.texture = renderTexture;
        }

        Debug.Log($"Webcam feed applied to Render Texture: {renderTexture.width}x{renderTexture.height}");

        // Set up screenshots folder
        screenshotsFolder = Path.Combine(Application.persistentDataPath, WebCamScreenShotsFolder);
        if (!Directory.Exists(screenshotsFolder))
        {
            Directory.CreateDirectory(screenshotsFolder);
            Debug.Log($"Created folder: {screenshotsFolder}");
        }
    }

    // Update the RenderTexture every frame to keep the webcam feed LIVE!
    private void Update()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            Graphics.Blit(webcamTexture, renderTexture);
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
            // Capture current frame from WebCamTexture
            RenderTexture.active = renderTexture;
            Texture2D frameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            frameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            frameTexture.Apply();

            // Save as PNG
            byte[] imageData = frameTexture.EncodeToPNG();
            await File.WriteAllBytesAsync(filePath, imageData);

            Debug.Log($"Webcam frame captured and saved to: {filePath}");

            // Cleanup
            RenderTexture.active = null;
            Destroy(frameTexture);
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
