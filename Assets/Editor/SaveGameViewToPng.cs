using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveGameViewToPng {
    [MenuItem("Tools/Save Game View to PNG")]
    public static void _SaveGameViewToPng() {


        Camera camera = Camera.main ?? Camera.allCameras.FirstOrDefault();
        if (camera == null) {
            Debug.LogError("Game View camera is null.");
            return;
        }

        // Set up a RenderTexture
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;

        // Render the camera to the RenderTexture
        camera.Render();

        // Read pixels into a Texture2D
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        var pixels = screenShot.GetPixels();
        //clear alpha
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i].a = 1;
        }
        screenShot.SetPixels(pixels);
        //screenShot.Apply();

        // Cleanup
        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(rt);

        // Encode texture to PNG
        byte[] bytes = screenShot.EncodeToPNG();
        Object.DestroyImmediate(screenShot);

        //file save dialog
        if (bytes == null || bytes.Length == 0) {
            Debug.LogError("Failed to encode screenshot to PNG.");
            return;
        }

        //file save dialog
        string path = EditorUtility.SaveFilePanel("Save Scene View as PNG", "Screenshots", $"SceneView_{System.DateTime.Now:yyyyMMdd_HHmmss}.png", "png");
        if (string.IsNullOrEmpty(path)) {
            Debug.Log("Save canceled.");
            return;
        }
        // Save to file
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved Scene View screenshot to: {path}");
    }
}