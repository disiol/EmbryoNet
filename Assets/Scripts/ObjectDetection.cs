using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDetection : MonoBehaviour
{
    private Image imageObject; // Assign the Image component to this field in the Inspector
    [HideInInspector]public string jsonFilePath; // Set the path to the JSON file in the Inspector

    [System.Serializable]
    public class Detection
    {
        public int tlx;
        public int tly;
        public int brx;
        public int bry;
    }

    [System.Serializable]
    public class DetectionData
    {
        public List<Detection> detection_list;
    }

    public void DrawFrames()
    {
        imageObject = GetComponent<Image>();
        
        if (imageObject != null && imageObject.sprite != null)
        {
            Texture2D imageTexture = imageObject.sprite.texture;

            // Load the JSON data from the file path
            string jsonFileContent = File.ReadAllText(jsonFilePath);
            DetectionData detectionData = JsonUtility.FromJson<DetectionData>(jsonFileContent);

            // Draw frames around the detected objects
            Vector2 pivotOffset = new Vector2(0.5f, 0.5f); // Pivot offset for the frame image

            foreach (Detection detection in detectionData.detection_list)
            {
                DrawFrame(detection, imageTexture, pivotOffset);
            }
        }
        else
        {
            Debug.LogWarning("Image object or its sprite is null.");
        }
    }

    void DrawFrame(Detection detection, Texture2D imageTexture, Vector2 pivotOffset)
    {
        // Calculate the position and size of the frame
        Vector2 position = new Vector2(detection.brx - (detection.brx - detection.tlx) * 0.5f, detection.bry - (detection.bry - detection.tly) * 0.5f);
        Vector2 size = new Vector2(detection.brx - detection.tlx, detection.bry - detection.tly);

        // Create a new GameObject for each frame
        GameObject frame = new GameObject("Frame");
        frame.transform.SetParent(transform);

        // Add an Image component to the frame GameObject
        Image frameImage = frame.AddComponent<Image>();
        frameImage.sprite = Sprite.Create(imageTexture, new Rect(detection.tlx, imageTexture.height - detection.bry, size.x, size.y), pivotOffset);
        frameImage.rectTransform.sizeDelta = size;

        // Position the frame correctly
        frame.transform.position = position;
    }
}
