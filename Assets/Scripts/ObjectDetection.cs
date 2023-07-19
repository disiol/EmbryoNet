using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectDetection : MonoBehaviour
{
    public Texture2D imageTexture; // Assign the image texture to this field in the Inspector
    public string jsonFilePath; // Assign the JSON file to this field in the Inspector

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

    public void DrowFrames()
    {
        // Load the JSON data
        string jsonFileContent = File.ReadAllText(jsonFilePath);
        DetectionData detectionData = JsonUtility.FromJson<DetectionData>(jsonFileContent);
        
        // Create a SpriteRenderer to display the image
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));

        // Loop through the detections and draw frames around the objects
        foreach (Detection detection in detectionData.detection_list)
        {
            Vector2 position = new Vector2(detection.tlx + (detection.brx - detection.tlx) * 0.5f, detection.tly + (detection.bry - detection.tly) * 0.5f);
            Vector2 size = new Vector2(detection.brx - detection.tlx, detection.bry - detection.tly);

            // Create a new GameObject for each frame
            GameObject frame = new GameObject("Frame");
            frame.transform.SetParent(transform);

            // Add a SpriteRenderer to the frame GameObject
            SpriteRenderer frameRenderer = frame.AddComponent<SpriteRenderer>();
            frameRenderer.sprite = Sprite.Create(imageTexture, new Rect(detection.tlx, imageTexture.height - detection.bry, size.x, size.y), new Vector2(0.5f, 0.5f));

            // Position the frame correctly
            frame.transform.position = position;
            
            
        }
        
    }
    
    private void CrateSprite(Texture2D texture, Image imageObject)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        imageObject.sprite = sprite;
    }
}