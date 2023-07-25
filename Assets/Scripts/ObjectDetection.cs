using System.Collections.Generic;
using System.IO;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDetection : MonoBehaviour
{
    private Image _imageObject; // Assign the Image component to this field in the Inspector
    [HideInInspector] public string jsonFilePath; // Set the path to the JSON file in the Inspector
    [SerializeField] private GameObject buttonPrefab;


    public void DrawFrames()
    {
        _imageObject = GetComponent<Image>();

        if (_imageObject != null && _imageObject.sprite != null)
        {
            DestroyAllChildrenImageObject();
            Texture2D imageTexture = _imageObject.sprite.texture;

            // Load the JSON data from the file path
            string jsonFileContent = File.ReadAllText(jsonFilePath);
            ParserModel.Root detectionData = JsonUtility.FromJson<ParserModel.Root>(jsonFileContent);

            // Draw frames around the detected objects
            Vector2 pivotOffset = new Vector2(0.5f, 0.5f); // Pivot offset for the frame image

            var detectionDataDetectionList = detectionData.detection_list;
          
            foreach (ParserModel.DetectionList detection in detectionDataDetectionList)
            {
                DrawFrame(detection, imageTexture, pivotOffset);
            }
        }
        else
        {
            Debug.LogWarning("Image object or its sprite is null.");
        }
    }

    void DrawFrame(ParserModel.DetectionList detection, Texture2D imageTexture, Vector2 pivotOffset)
    {
        // TODO Calculate the position and size of the frame
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

        button.GetComponent<ButtonData>().targetID = detection.id;
        button.GetComponent<ButtonData>().jsonFilePath = jsonFilePath;


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
    }
    
    
    public void DestroyAllChildrenImageObject()
    {
        int childCount = _imageObject.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = _imageObject.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}