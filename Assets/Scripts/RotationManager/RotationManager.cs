using System;
using System.Collections.Generic;
using System.IO;
using Models;
using TMPro;
using Tolls;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationManager : MonoBehaviour
    {
        private ParserModel.Root _records;
        private string _dataFilePath;

        private List<ParserModel.Root> _dataList;


        private GameObject _rotationMenu;

        private TMP_InputField _menuXInput;
        private TMP_InputField _menuYInput;
        private TMP_InputField _menuZInput;

        private Button _saveChangesButton;
        private Button _buttonCancel;


        private readonly string _newFolderName = "new_Data_with_rotations";

        private string _fileName;
        private string _newFileName;


        private bool _isMenuVisible = false;
        private ParserModel.DetectionList _targetRecord;
        private int _targetID;
        private string _newFilePath;

        private TMP_InputField _inputFieldEnterFolderNameForSafeNewData;
        private FrameManager _frameManager;
        private Transform _panel;
        private Transform _rightSide;
        private Transform _infoPanel;
        private JasonManager _jasonManager;
      


        public void ShowMenu()
        {
            GameObject canvens = GameObject.Find("Canvas");
            _panel = canvens.transform.Find("Panel");
            _rightSide = _panel.transform.Find("RightSide");
            _infoPanel = _rightSide.transform.Find("InfoPanel");

            _rotationMenu = _infoPanel.Find("RotationMenu").GameObject();
            _rotationMenu.GetComponent<RotationController>().rotationManager =
                transform.GetComponent<RotationManager>();


            _isMenuVisible = !_isMenuVisible;
            _rotationMenu.SetActive(_isMenuVisible);

            // LoadData();

            if (_targetRecord != null)
            {
                ButtonSafe();
                // Show the current rotation values in the menu
                ShowCurrentRotationValuesInTheMenu();
            }
        }


        private void ButtonSafe()
        {
            Transform bottom = _rightSide.transform.Find("Bottom");
            _saveChangesButton = bottom.transform.Find("ButtonSave").gameObject
                .GetComponent<Button>();
            _saveChangesButton.onClick.AddListener(SaveDataToFile);
        }

        private void ShowCurrentRotationValuesInTheMenu()
        {
            GetRotationMenuFileds();

            float x = _targetRecord.rotation.x;
            
            _menuXInput.text = x.ToString();


            _menuYInput.text = _targetRecord.rotation.y.ToString();


            _menuZInput.text = _targetRecord.rotation.z.ToString();
        }

        private void GetRotationMenuFileds()
        {
            _menuXInput = _rotationMenu.transform.Find("RotationX").transform.Find("InputFieldX").gameObject
                .GetComponent<TMP_InputField>();

            _menuYInput = _rotationMenu.transform.Find("RotationY").transform.Find("InputFieldY").gameObject
                .GetComponent<TMP_InputField>();

            _menuZInput = _rotationMenu.transform.Find("RotationZ").GameObject().transform.Find("InputFieldZ")
                .gameObject
                .GetComponent<TMP_InputField>();
        }

        public void HideMenu()
        {
            _rotationMenu.SetActive(false);
            _isMenuVisible = false;
        }

        public void UpdateRotation()
        {
            GameObject canvens = GameObject.Find("Canvas");
            _panel = canvens.transform.Find("Panel");
            _jasonManager = _panel.GetComponent<JasonManager>();
            _dataList = _jasonManager.dataList;

            
            GetRotationMenuFileds();

            if (_dataList != null)
            {
                // Update the rotation in the target record
                //TODO логика: ты стоишь на фрейме 10 и у тебя ротации 0 0 0. Ты на фрейме 10 ставишь 5 5 0.
                //Программа смотрит что было в предыдущем состоянии - видит было 0 0 0.
                //Значит дельта 5 5 0. Она эту дельту приплюсовывает к каждой ротации от 10 - конец.
                //Если больше чем 360, то вычитает 360.

                float x = float.Parse(_menuXInput.text);
                float y = float.Parse(_menuYInput.text);
                float z = float.Parse(_menuZInput.text);


              

                UpdateRotationInDataList(x, y, z);
                ShowCurrentRotationValuesInTheMenu();
                _jasonManager.dataList = _dataList;
                
                SetNewRotation(x, y, z);


                // Hide the menu after saving changes
                // HideMenu();
            }
            else
            {
                //TODO exephen show
                Debug.Log("_targetRecord = " + _targetRecord);
            }
        }

        private void SetNewRotation(float x, float y, float z)
        {
            Vector3 newTargetRecordRotation = new Vector3(x, y, z);
            transform.rotation = Quaternion.Euler(newTargetRecordRotation);
        }

        private void UpdateRotationInDataList(float x, float y, float z)
        {
            for (int i = 0; i < _dataList.Count; i++)
            {
                List<ParserModel.DetectionList> detectionList = _dataList[i].detection_list;

                if (_dataList.Count > 0)
                {
                    _targetRecord = FindRecordById(_targetID, detectionList);


                    if (_targetRecord != null)
                    {
                        Vector3 recordOldRotation = _targetRecord.rotation;

                        var deltaAngleX = Mathf.DeltaAngle(recordOldRotation.x, x);
                        var deltaAngleY = Mathf.DeltaAngle(recordOldRotation.y, y);
                        var deltaAngleZ = Mathf.DeltaAngle(recordOldRotation.z, z);


                        _targetRecord.rotation = new Vector3(recordOldRotation.x + deltaAngleX,
                                recordOldRotation.y + deltaAngleY,
                                recordOldRotation.z + deltaAngleZ);
                    }
                 
                }
            }
        }

        public ParserModel.DetectionList FindRecordById(int targetID,
            List<ParserModel.DetectionList> recordsDetectionList)
        {
            Debug.Log("targetID = " + targetID);
            foreach (ParserModel.DetectionList record in recordsDetectionList)
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
            //TODO sow InputFieldEnterFolderNameForSafeNewData
            // _inputFieldEnterFolderNameForSafeNewData = _rotationMenu.transform
            //     .Find("InputFieldEnterFolderNameForSafeNewData").GetComponent<TMP_InputField>();
            // string inputFieldEnterFolderNameForSafeNewDataText = _inputFieldEnterFolderNameForSafeNewData.text;
            //
            // if (!inputFieldEnterFolderNameForSafeNewDataText.Equals(""))
            // {
            //     _newFolderName = inputFieldEnterFolderNameForSafeNewDataText;
            // }


            string newDataFilePath = _dataFilePath.Replace(_fileName, "");
            Debug.Log(newDataFilePath); // Output: "This is a phrase to remove."


            string folderPath = Path.Combine(newDataFilePath, _newFolderName);

            _newFilePath = Path.Combine(folderPath, _newFileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //  TODO зберегти масив даних з jsonFiles, та правити їх.
            // Save the modified data back to the JSON file
            string updatedJsonString = JsonUtility.ToJson(_records, true);
            File.WriteAllText(_newFilePath, updatedJsonString);

            Debug.Log("Changes saved to file: " + _newFilePath);

            // OpenNewFile();
        }

        private void LoadData()
        {
            // Read JSON data from the file
            if (File.Exists(_dataFilePath))
            {
                // Load the JSON data from the file path

              
                //TODO _fileName = Path.GetFileName(_dataFilePath);
                // _newFileName = Path.GetFileNameWithoutExtension(_dataFilePath) + "_3d_cods.json"; //TODO folder C10

                // Optionally, set the initial target record
                _targetRecord = FindRecordById(_targetID, _records.detection_list);

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

        public void SetCurrentDetection(ParserModel.DetectionList detection)
        {
            _targetRecord = detection;
        }
    }
}