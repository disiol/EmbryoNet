using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDetection : MonoBehaviour
{
    private Image imageObject; // Assign the Image component to this field in the Inspector
    [HideInInspector] public string jsonFilePath; // Set the path to the JSON file in the Inspector
    [SerializeField] private GameObject buttonPrefab;

    [System.Serializable]
    public class Detection
    {
        public int id;
        public int tlx;
        public int tly;
        public int brx;
        public int bry;
        public Vector3 rotation;
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

            var detectionDataDetectionList = detectionData.detection_list;
            foreach (Detection detection in detectionDataDetectionList)
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
        Vector2 position = new Vector2(detection.brx - (detection.brx - detection.tlx) * 0.5f,
            detection.bry - (detection.bry - detection.tly) * 0.5f);
        Vector2 size = new Vector2(detection.brx - detection.tlx, detection.bry - detection.tly);

        // Create a new GameObject for each frame
        GameObject frame = new GameObject("Frame_" + detection.id);
        frame.transform.SetParent(transform);

        // Add an Image component to the frame GameObject
        Image frameImage = frame.AddComponent<Image>();
        frameImage.sprite = Sprite.Create(imageTexture,
            new Rect(detection.tlx, imageTexture.height - detection.bry, size.x, size.y), pivotOffset);
        frameImage.rectTransform.sizeDelta = size;

        // Position the frame correctly
        frame.transform.position = position;

        // Create the button on top of the frame
        GameObject button = Instantiate(buttonPrefab, frame.transform);
        button.name = "ButtonRotation_" + detection.id;

// Access the RectTransform of the button
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        

// Set the size of the button
        buttonRect.sizeDelta = size;
        button.transform.position = position;


// Handle button rotation based on the frame's rotation
        Vector3 detectionRotation = detection.rotation;
        if (detectionRotation != Vector3.zero)
        {
            // Set the button's rotation to match the frame's rotation
            button.transform.rotation = Quaternion.Euler(detectionRotation);
        }
        else
        {
            SaveDetectionDataToJson(detection.id);
        }
    }
    
    private DetectionData LoadJSONDataFromPath()
    {
        string jsonFileContent = File.ReadAllText(jsonFilePath);
        DetectionData detectionData = JsonUtility.FromJson<DetectionData>(jsonFileContent);
        return detectionData;
    }

    public void SaveDetectionDataToJson(int targetID)
    {
        DetectionData detectionData = LoadJSONDataFromPath();
        // Find the record by ID
        Detection targetDetection = null;


        foreach (Detection detection in detectionData.detection_list)
        {
            if (detection.id == targetID)
            {
                targetDetection = detection;
                break;
            }

            // Add the new field
            if (targetDetection != null)
            {
                targetDetection.rotation = new Vector3(1f, 2f, 3f);
            }

            // Save changes back to the file
            SaveDataToFile();
        }
    }

    private void SaveDataToFile()
    {
        // Convert the data to JSON format
        string jsonData = JsonUtility.ToJson(this);

        // Write the JSON data to the file
        File.WriteAllText(jsonFilePath, jsonData);

        Debug.Log("Changes saved to file: " + jsonFilePath);
    }
}