using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using Models;
using RotationManager;
using SafeDadta;
using UnityEngine;
using UnityEngine.UI;

public class FrameManager : MonoBehaviour
{
    private Image _imageObject; // Assign the Image component to this field in the Inspector
    public ParserModel.Root detectionData; // Set the path to the JSON filePath in the Inspector

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject arrowsPreab;
    [SerializeField] private RawImage arrowsRawImage;

    [SerializeField] private Color frameImageColor;
    [SerializeField] private Color selectedFrameImageColor;

    private static int _revesConst;
    private static float _corectedScaleIndex;

    private int _selectedDetectionID;
    public string dataFilePath;
    private SafeAndLoadData _safeAndLoadData;
    private GameObject _selectedButtonRotation;
    private GameObject _arrows;


    public void DrawFrames()
    {
        _imageObject = GetComponent<Image>();
        _safeAndLoadData = GameObject.Find("Canvas/Panel").GetComponent<SafeAndLoadData>();
        _selectedDetectionID = _safeAndLoadData.LoadCurrentId();
        _arrows = GameObject.Find("Arrows");


        if (_imageObject != null && _imageObject.sprite != null)
        {
            DestroyAllChildrenImageObjectAndGetimageObjectTexture();

            // Load the JSON data from the filePath path

            // Draw frames around the detected objects
            DrawFramesAroundTheDetectedObjects(detectionData);
        }
        else
        {
            Debug.LogWarning("Image object or its sprite is null.");
        }
    }

    private void DestroyAllChildrenImageObjectAndGetimageObjectTexture()
    {
        DestroyAllChildrenImageObject(_arrows.transform);
        DestroyAllChildrenImageObject(_imageObject.transform);
        DestroyAllChildrenImageObject(arrowsRawImage.transform);
    }

    private void DrawFramesAroundTheDetectedObjects(ParserModel.Root detectionData)
    {
        var detectionDataDetectionList = detectionData.detection_list;

        foreach (ParserModel.DetectionList detection in detectionDataDetectionList)
        {
            DrawFrame(detection);
        }

        // TODO if (_selectedButtonRotation != null)
        // {
        //     _selectedButtonRotation.GetComponent<RotationManager.RotationManager>().LoadData();
        //
        // }
    }

    void DrawFrame(ParserModel.DetectionList detection)
    {
        // TODO Calculate the position and size of the frame
        var position = CalculatePositionAndSize(detection, out var size);

        // Create a new GameObject for each frame
        var frame = CreateGameObjectForEachFrame(detection, size, position);

        CreateButtonOnTopOfTheFrame(detection, frame, size, position);
        CrateArrows(detection, frame.transform.localPosition, size);
    }

    private Vector2 CalculatePositionAndSize(ParserModel.DetectionList detection, out Vector2 size)
    {
        _revesConst = 1048;
        Sprite spriteObjectSprite = _imageObject.sprite;
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
        GameObject button = Instantiate(buttonPrefab, arrowsRawImage.transform);
        int detectionID = detection.id;
        button.name = "ButtonRotation_" + detectionID;

        RotationManagerButtonData rotationManagerButtonData = button.GetComponent<RotationManagerButtonData>();
        rotationManagerButtonData.targetID = detectionID;

        RotationManager.RotationManager rotationManager = button.GetComponent<RotationManager.RotationManager>();
        rotationManager.dataFilePath = dataFilePath;
        rotationManager.targetRecord = detection;


        // var renderTexture = CrateRenderTextureForButton(detection, button, detectionID);


        // Access the RectTransform of the button
        SetTheSizeOfTheButton(size, position, button);


        if (_selectedDetectionID == detectionID)
        {
            _selectedButtonRotation = button;
        }
    }

    private static void SetTheSizeOfTheButton(Vector2 size, Vector2 position, GameObject button)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
// Set the size of the button
        buttonRect.sizeDelta = size;
        button.transform.position = position;
    }

    private void CrateArrows(ParserModel.DetectionList detection, Vector2 position, Vector3 size)
    {
        var trideCorectedScaleIndex = 100;
        GameObject arrows = Instantiate(arrowsPreab, _arrows.transform);
        arrows.name = "Arrows_" + detection.id;

        Transform arrowsTransform = arrows.transform;

        arrowsTransform.position =
            new Vector3(position.x / trideCorectedScaleIndex, position.y / trideCorectedScaleIndex, 10);

        arrowsTransform.localScale = new Vector3(size.x / trideCorectedScaleIndex, size.y / trideCorectedScaleIndex,
            size.y / trideCorectedScaleIndex);


        // Handle button rotation based on the frame's rotation
        Vector3 detectionRotation = detection.rotation;
        if (detectionRotation != Vector3.zero)
        {
            // Set the Arrows rotation to match the frame's rotation
            arrows.transform.rotation = Quaternion.Euler(detectionRotation);
        }
    }

    private RenderTexture CrateRenderTextureForButton(ParserModel.DetectionList detection, GameObject button,
        int detectionID)
    {
        RenderTexture renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        // Release the hardware resources used by the render texture 
        renderTexture.Release();
        renderTexture.name = "RenderTexture_" + detectionID;
        return renderTexture;
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

        frameImage.GetComponent<RectTransform>().sizeDelta = size;
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


    private void DestroyAllChildrenImageObject(Transform objectTransform)
    {
        int childCount = objectTransform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = objectTransform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}