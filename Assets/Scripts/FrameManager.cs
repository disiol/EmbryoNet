using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using Models;
using RotationManager;
using SafeDadta;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FrameManager : MonoBehaviour
{
    private Image _imageObject; // Assign the Image component to this field in the Inspector
    [HideInInspector] public ParserModel.Root detectionData; // Set the path to the JSON filePath in the Inspector
    
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Color frameImageColor;
    [SerializeField] private Color selectedFrameImageColor;

    private static int _revesConst;
    private static int _corectedScaleIndex; 
    
    private int _selectedDetectionID;
    public string dataFilePath;
    private SafeAndLoadData _safeAndLoadData;


    public void DrawFrames()
    {
        _imageObject = GetComponent<Image>();
        _safeAndLoadData = gameObject.GetComponent<SafeAndLoadData>();
        _selectedDetectionID = _safeAndLoadData.LoadCurrentId();


        if (_imageObject != null && _imageObject.sprite != null)
        {
            var imageTexture = DestroyAllChildrenImageObjectAndGetimageObjectTexture();

            // Load the JSON data from the filePath path

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

    private void DrawFramesAroundTheDetectedObjects(ParserModel.Root detectionData, Texture2D imageTexture)
    {
        var detectionDataDetectionList = detectionData.detection_list;

        foreach (ParserModel.DetectionList detection in detectionDataDetectionList)
        {
            DrawFrame(detection);
        }
    }

    void DrawFrame(ParserModel.DetectionList detection)
    {
        // TODO Calculate the position and size of the frame
        var position = CalculatePositionAndSize(detection, out var size);

        // Create a new GameObject for each frame
        var frame = CreateGameObjectForEachFrame(detection, size, position);

        CreateButtonOnTopOfTheFrame(detection, frame, size, position);
    }

    private static Vector2 CalculatePositionAndSize(ParserModel.DetectionList detection, out Vector2 size)
    {
        _revesConst = 1048;
        _corectedScaleIndex = 2;
        Vector2 position = new Vector2((detection.brx - (detection.brx - detection.tlx) * 0.5f) / _corectedScaleIndex,
            _revesConst - (detection.bry - (detection.bry - detection.tly) * 0.5f) / _corectedScaleIndex);


        size = new Vector2(detection.brx - detection.tlx, detection.bry - detection.tly) / _corectedScaleIndex;
        return position;
    }

    private void CreateButtonOnTopOfTheFrame(ParserModel.DetectionList detection, GameObject frame, Vector2 size,
        Vector2 position)
    {
        // Create the button on top of the frame
        GameObject button = Instantiate(buttonPrefab, frame.transform);
        button.name = "ButtonRotation_" + detection.id;

        RotationManagerButtonData rotationManagerButtonData = button.GetComponent<RotationManagerButtonData>();
        rotationManagerButtonData.targetID = detection.id;
      
        RotationManager.RotationManager rotationManager = button.GetComponent<RotationManager.RotationManager>();
        rotationManager.dataFilePath = dataFilePath;
        rotationManager.targetRecord = detection;

        

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

    private GameObject CreateGameObjectForEachFrame(ParserModel.DetectionList detection,
        Vector2 size, Vector2 position)
    {
        int detectionID = detection.id;
        GameObject frame = new GameObject("Frame_" + detectionID);
        frame.transform.SetParent(transform);

        // Add an Image component to the frame GameObject
        Image frameImage = frame.AddComponent<Image>();

        SetColorFrame(detectionID, frameImage);

        frameImage.rectTransform.sizeDelta = size;
        // Position the frame correctly
        frame.transform.position = position;
        return frame;
    }

    private void SetColorFrame(int detectionID, Image frameImage)
    {
        if (detectionID.Equals(_selectedDetectionID))
        {
            frameImage.color = selectedFrameImageColor;
        }
        else
        {
            frameImage.color = frameImageColor;
        }
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