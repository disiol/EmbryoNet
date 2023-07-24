using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Models;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

[System.Serializable]
public class RotationManager : MonoBehaviour
{
    private ParserModel.Root _records;
    private string _dataFilePath;

    private GameObject _rotationMenu;

    private TMP_InputField _menuXInput;
    private TMP_InputField _menuYInput;
    private TMP_InputField _menuZInput;

    private Button _saveChangesButton;
    private Button _buttonCancel;


    private string _newFolderName = "new_Data_with_rotations";
    private string _fileName;


    private bool _isMenuVisible = false;
    private ParserModel.DetectionList _targetRecord;
    private int _targetID;
    private string _newFilePath;

    private TMP_InputField _inputFieldEnterFolderNameForSafeNewData;


    public void ShowMenu()
    {
        _rotationMenu = GameObject.Find("Canvas").transform.Find("RotationMenu").gameObject;

        _isMenuVisible = !_isMenuVisible;
        _rotationMenu.SetActive(_isMenuVisible);

        LoadDataFromFile();


        if (_isMenuVisible && _targetRecord != null)
        {
            ButtonSafe();
            ButtonCancel();


            // Show the current rotation values in the menu
            ShowCurrentRotationValuesInTheMenu();
        }
    }

    private void ButtonCancel()
    {
        _buttonCancel = _rotationMenu.transform.Find("ButtonCancel").GetComponent<Button>();
        _buttonCancel.onClick.AddListener(HideMenu);
    }

    private void ButtonSafe()
    {
        _saveChangesButton = _rotationMenu.transform.Find("ButtonSafe").GetComponent<Button>();
        _saveChangesButton.onClick.AddListener(SaveChanges);
    }

    private void ShowCurrentRotationValuesInTheMenu()
    {
        Transform rotationX = _rotationMenu.transform.Find("RotationX");
        GameObject inputFieldX = rotationX.transform.Find("InputFieldX").gameObject;
        _menuXInput = inputFieldX.GetComponent<TMP_InputField>();

        float x = _targetRecord.rotation.x;
        _menuXInput.text = x.ToString();

        _menuYInput = _rotationMenu.transform.Find("RotationY").transform.Find("InputFieldY").gameObject
            .GetComponent<TMP_InputField>();
        _menuYInput.text = _targetRecord.rotation.y.ToString();

        _menuZInput = _rotationMenu.transform.Find("RotationZ").GameObject().transform.Find("InputFieldZ").gameObject
            .GetComponent<TMP_InputField>();
        _menuZInput.text = _targetRecord.rotation.z.ToString();
    }

    public void HideMenu()
    {
        _rotationMenu.SetActive(false);
        _isMenuVisible = false;
    }

    private void SaveChanges()
    {
        if (_targetRecord != null)
        {
            _inputFieldEnterFolderNameForSafeNewData = _rotationMenu.transform
                .Find("InputFieldEnterFolderNameForSafeNewData").GetComponent<TMP_InputField>();
            string inputFieldEnterFolderNameForSafeNewDataText = _inputFieldEnterFolderNameForSafeNewData.text;

            if (!inputFieldEnterFolderNameForSafeNewDataText.Equals(""))
            {
                _newFolderName = inputFieldEnterFolderNameForSafeNewDataText;
            }


            string newDataFilePath = _dataFilePath.Replace(_fileName, "");
            Debug.Log(newDataFilePath); // Output: "This is a phrase to remove."


            string folderPath = Path.Combine(newDataFilePath, _newFolderName);

            _newFilePath = Path.Combine(folderPath, _fileName);

            // Update the rotation in the target record
            float x = float.Parse(_menuXInput.text);
            float y = float.Parse(_menuYInput.text);
            float z = float.Parse(_menuZInput.text);

            Vector3 targetRecordRotation = new Vector3(x, y, z);
            _targetRecord.rotation = targetRecordRotation;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Save changes back to the file
            try
            {
                SaveDataToFile();
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving file _newFilePath: " + e.Message);
            }

            transform.rotation = Quaternion.Euler(targetRecordRotation);


            // Hide the menu after saving changes
            HideMenu();
        }
        else
        {
            //TODO exephen show
            Debug.Log("_targetRecord = " + _targetRecord);
        }
    }

    private ParserModel.DetectionList FindRecordById(int targetID)
    {
        Debug.Log("targetID = " + targetID);
        foreach (ParserModel.DetectionList record in _records.detection_list)
        {
            if (record.id == targetID)
            {
                return record;
            }
        }

        return null;
    }

    private void SaveDataToFile()
    {
        // Save the modified data back to the JSON file
        string updatedJsonString = JsonUtility.ToJson(_records, true);
        File.WriteAllText(_newFilePath, updatedJsonString);

        Debug.Log("Changes saved to file: " + _newFilePath);

        //TODO open ne file dadata _newFilePath
    }

    private void LoadDataFromFile()
    {
        // Read JSON data from the file
        if (File.Exists(_dataFilePath))
        {
            // Load the JSON data from the file path
            string jsonFileContent = File.ReadAllText(_dataFilePath);
            _records = JsonUtility.FromJson<ParserModel.Root>(jsonFileContent);
            _fileName = Path.GetFileName(_dataFilePath);

            // Optionally, set the initial target record
            _targetRecord = FindRecordById(_targetID);
            Vector3 recordRotation = _targetRecord.rotation;

            if (recordRotation != null)
            {
                Vector3 targetRecordRotation = recordRotation;
                transform.rotation = Quaternion.Euler(targetRecordRotation);
            }
        }
        else
        {
            Debug.LogWarning("JSON data file not found: " + _dataFilePath);
        }
    }

    public void SetTargetID(int targetID)
    {
        this._targetID = targetID;
    }

    public void SetDataFilePath(string jsonFilePath)
    {
        this._dataFilePath = jsonFilePath;
    }
}