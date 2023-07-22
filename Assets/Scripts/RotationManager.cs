using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Serialization;

[System.Serializable]
public class MyRecord
{
    public string id;
    public int tlx;
    public int tly;
    public int brx;
    public int bry;
    public Vector3 rotation; // Add rotation information
}

public class RotationManager : MonoBehaviour
{
    private ObjectDetection.DetectionData _records;
    private string _dataFilePath;

    private GameObject _rotationMenu;
    private Button _saveChangesButton;

    private InputField _menuXInput;
    private InputField _menuYInput;
    private InputField _menuZInput;

    private bool _isMenuVisible = false;
    private ObjectDetection.Detection _targetRecord;
    private int _targetID;


    public void ShowMenu()
    {
        _rotationMenu = GameObject.Find("Canvas").transform.Find("RotationMenu").gameObject;
        _isMenuVisible = !_isMenuVisible;
        _rotationMenu.SetActive(_isMenuVisible);

        LoadDataFromFile();


        if (_isMenuVisible && _targetRecord != null)
        {
            _saveChangesButton = _rotationMenu.transform.Find("ButtonSafe").GetComponent<Button>();

            _saveChangesButton.onClick.AddListener(SaveChanges);


            // Show the current rotation values in the menu
            _menuXInput = _rotationMenu.transform.Find("RotationX").transform.Find("InputFieldX")
                .GetComponent<InputField>();
            _menuXInput.text = _targetRecord.rotation.x.ToString();

            _menuYInput = _rotationMenu.transform.Find("RotationY").transform.Find("InputFieldY")
                .GetComponent<InputField>();
            _menuYInput.text = _targetRecord.rotation.y.ToString();

            _menuZInput = _rotationMenu.transform.Find("RotationZ").transform.Find("InputFieldZ")
                .GetComponent<InputField>();
            _menuZInput.text = _targetRecord.rotation.z.ToString();
        }
    }

    private void HideMenu()
    {
        _rotationMenu.SetActive(false);
        _isMenuVisible = false;
    }

    private void SaveChanges()
    {
        if (_targetRecord != null)
        {
            // Update the rotation in the target record
            float x = float.Parse(_menuXInput.text);
            float y = float.Parse(_menuYInput.text);
            float z = float.Parse(_menuZInput.text);

            Vector3 targetRecordRotation = new Vector3(x, y, z);
            _targetRecord.rotation = targetRecordRotation;

            // Save changes back to the file
            SaveDataToFile();

            transform.rotation = Quaternion.Euler(targetRecordRotation);


            // Hide the menu after saving changes
            HideMenu();
        }
    }

    private ObjectDetection.Detection FindRecordById(int targetID)
    {
        foreach (ObjectDetection.Detection record in _records.detection_list)
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
        // Convert the data to JSON format
        string jsonData = JsonUtility.ToJson(this);

        // Write the JSON data to the file
        File.WriteAllText(_dataFilePath, jsonData);

        Debug.Log("Changes saved to file: " + _dataFilePath);
    }

    private void LoadDataFromFile()
    {
        // Read JSON data from the file
        if (File.Exists(_dataFilePath))
        {
            // Load the JSON data from the file path
            // Load the JSON data from the file path
            string jsonFileContent = File.ReadAllText(_dataFilePath);
            _records = JsonUtility.FromJson<ObjectDetection.DetectionData>(jsonFileContent);

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