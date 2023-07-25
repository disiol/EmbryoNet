using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonData : MonoBehaviour
{
    [HideInInspector] public int targetID;
    [HideInInspector] public string jsonFilePath; // Set the path to the JSON file in the Inspector

    private RotationManager _rotationManager;

    private void Start()
    {
        _rotationManager = transform.GetComponent<RotationManager>();
        transform.GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }


    private void OnButtonClick()
    {
        //TODO увеличить скале
        _rotationManager.SetTargetID(targetID);
        _rotationManager.SetDataFilePath(jsonFilePath);
        _rotationManager.ShowMenu();
    }
}