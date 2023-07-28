using System.Collections.Generic;
using System.IO;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class FrameManager : MonoBehaviour
{
    private Image _imageObject; // Assign the Image component to this field in the Inspector
    [HideInInspector] public string jsonFilePath; // Set the path to the JSON file in the Inspector
    [SerializeField] private GameObject buttonPrefab;
    private static int _revesConst;
    private Color frameImageColor;


    public void DrawFrames()
    {
        _imageObject = GetComponent<Image>();

        if (_imageObject != null && _imageObject.sprite != null)
        {
            var imageTexture = DestroyAllChildrenImageObjectAndGetimageObjectTexture();

            // Load the JSON data from the file path
            ParserModel.Root detectionData = LoadTheJSOnData();

            // Draw frames around the detected objects
            DrawFramesAroundTheDetectedObjects(detectionData, imageTexture);
        }
        else
        {
            Debug.LogWarning("Image object or its sprite is null.");
        }
    }

    private Texture2D DestroyAllChildrenImageObjectAndGetimageObjectTexture()
    {
        DestroyAllChildrenImageObject();
        Texture2D imageTexture = _imageObject.sprite.texture;
        return imageTexture;
    }

    private ParserModel.Root LoadTheJSOnData()
    {
        string jsonFileContent = File.ReadAllText(jsonFilePath);
        ParserModel.Root detectionData = JsonUtility.FromJson<ParserModel.Root>(jsonFileContent);
        return detectionData;
    }

    private void DrawFramesAroundTheDetectedObjects(ParserModel.Root detectionData, Texture2D imageTexture)
    {
        Vector2 pivotOffset = new Vector2(0.5f, 0.5f); // Pivot offset for the frame image

        var detectionDataDetectionList = detectionData.detection_list;

        foreach (ParserModel.DetectionList detection in detectionDataDetectionList)
        {
            DrawFrame(detection, imageTexture, pivotOffset);
        }
    }

    void DrawFrame(ParserModel.DetectionList detection, Texture2D imageTexture, Vector2 pivotOffset)
    {
        // TODO Calculate the position and size of the frame
        var position = CalculatePositionAndSize(detection, out var size);

        // Create a new GameObject for each frame
        var frame = CreateGameObjectForEachFrame(detection, imageTexture, pivotOffset, size, position);

        CreateButtonOnTopOfTheFrame(detection, frame, size, position);
    }

    private static Vector2 CalculatePositionAndSize(ParserModel.DetectionList detection, out Vector2 size)
    {
        _revesConst = 1000;
        Vector2 position = new Vector2((detection.brx - (detection.brx - detection.tlx) * 0.5f) / 2,
            _revesConst - (detection.bry - (detection.bry - detection.tly) * 0.5f) / 2);


        size = new Vector2(detection.brx - detection.tlx, detection.bry - detection.tly) / 2;
        return position;
    }

    private void CreateButtonOnTopOfTheFrame(ParserModel.DetectionList detection, GameObject frame, Vector2 size,
        Vector2 position)
    {
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

    private GameObject CreateGameObjectForEachFrame(ParserModel.DetectionList detection, Texture2D imageTexture,
        Vector2 pivotOffset,
        Vector2 size, Vector2 position)
    {
        GameObject frame = new GameObject("Frame_" + detection.id);
        frame.transform.SetParent(transform);

        // Add an Image component to the frame GameObject
        Image frameImage = frame.AddComponent<Image>();
        frameImage.color = frameImageColor;

        frameImage.rectTransform.sizeDelta = size;
        // Position the frame correctly
        frame.transform.position = position;
        return frame;
    }


    private void DestroyAllChildrenImageObject()
    {
        int childCount = _imageObject.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = _imageObject.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}