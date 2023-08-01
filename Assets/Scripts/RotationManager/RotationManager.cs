using System;
using System.Collections.Generic;
using System.IO;
using Models;
using SafeDadta;
using TMPro;
using Tolls;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationManager : MonoBehaviour
    {
        private GameObject _rotationButton;

        private ParserModel.Root _records;
        public string dataFilePath;

        private Dictionary<string, ParserModel.Root> _dataList;


        private GameObject _rotationMenu;

        private TMP_InputField _menuXInput;
        private TMP_InputField _menuYInput;
        private TMP_InputField _menuZInput;

        private Button _saveChangesButton;


        private readonly string _newFolderName = "_new_Data_with_rotations";

        private string _fileName;
        private string _newFileName;

        private bool _isMenuVisible = false;
        public ParserModel.DetectionList targetRecord;
        private int _targetID;

        public int TargetID
        {
            set
            {
                // Значение можно будет установить
                this._targetID = value;
                // При установке значения также будем выводить текст в консоль
                Console.Write("Hello, " + this.name);
            }
        }

        private string _newFilePath;

        private TMP_InputField _inputFieldEnterFolderNameForSafeNewData;

        private Transform _panel;
        private Transform _rightSide;
        private Transform _infoPanel;

        private JasonManager _jasonManager;
        private SafeAndLoadData _safeAndLoadData;

        private void Start()
        {
            ShowMenu();
        }

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
            ButtonSafe();
            GetRotationMenuFileds();
        }


        private void ButtonSafe()
        {
            _saveChangesButton = _rotationMenu.transform.Find("ButtonSave").GetComponent<Button>();
            _saveChangesButton.onClick.AddListener(SaveDataToFile);
        }

        private void ShowCurrentRotationValuesInTheMenu()
        {
            GetRotationMenuFileds();

            Vector3 targetRecordRotation = targetRecord.rotation;
            float x = targetRecordRotation.x;

            _menuXInput.text = x.ToString();


            _menuYInput.text = targetRecordRotation.y.ToString();


            _menuZInput.text = targetRecordRotation.z.ToString();
        }

        private void GetRotationMenuFileds()
        {
            Debug.Log("GetRotationMenuFileds");

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
            //TODO refactoring
            _safeAndLoadData = gameObject.AddComponent<SafeAndLoadData>();

            _targetID = _safeAndLoadData.LoadCurrentId();
            Debug.Log("UpdateRotation");

            GameObject canvens = GameObject.Find("Canvas");
            _panel = canvens.transform.Find("Panel");
            _jasonManager = _panel.GetComponent<JasonManager>();
            _dataList = _jasonManager.dataList;
            ButtonSafe();

            Debug.Log("UpdateRotation");


            GetRotationMenuFileds();

            if (_dataList != null)
            {
                _jasonManager = _panel.GetComponent<JasonManager>();

                // Update the rotation in the target record
                //TODO логика: ты стоишь на фрейме 10 и у тебя ротации 0 0 0. Ты на фрейме 10 ставишь 5 5 0.
                //Программа смотрит что было в предыдущем состоянии - видит было 0 0 0.
                //Значит дельта 5 5 0. Она эту дельту приплюсовывает к каждой ротации от 10 - конец.
                //Если больше чем 360, то вычитает 360.

                float x = float.Parse(_menuXInput.text);
                float y = float.Parse(_menuYInput.text);
                float z = float.Parse(_menuZInput.text);


                UpdateRotationInDataList(x, y, z);
                _jasonManager.dataList = _dataList;

//TODO
                UpdateTargetRecord();
                ShowCurrentRotationValuesInTheMenu();
                SetNewRotation(x, y, z);


                // Hide the menu after saving changes
                // HideMenu();
            }
            else
            {
                //TODO exephen show
                Debug.Log("_exephen targetRecord = " + targetRecord);
            }
        }

        private void GetCurentRotationButton()
        {
            Debug.Log("GetCurentRotationButton");

            var image = _panel.transform.Find("LeftSide").transform.Find("Image").transform;

            for (int i = 0; i < image.childCount; i++)
            {
                Transform childTransform = image.GetChild(i);

                GameObject rotationButton = childTransform.transform.Find("ButtonRotation_" + _targetID).GameObject();
                if (rotationButton != null)
                {
                    _rotationButton = rotationButton;
                }
            }
        }

        private void SetNewRotation(float x, float y, float z)
        {
            Debug.Log("SetNewRotation");
            GetCurentRotationButton();
            Vector3 newTargetRecordRotation = new Vector3(x, y, z);
            _rotationButton.transform.rotation = Quaternion.Euler(newTargetRecordRotation);
        }

        private void UpdateRotationInDataList(float x, float y, float z)
        {
            Debug.Log("UpdateRotationInDataList");

            foreach (var data in _dataList)
            {
                List<ParserModel.DetectionList> detectionList = data.Value.detection_list;

                if (_dataList.Count > 0)
                {
                    targetRecord = FindRecordById(_targetID, detectionList);


                    if (targetRecord != null)
                    {
                        Vector3 recordOldRotation = targetRecord.rotation;

                        var deltaAngleX = Mathf.DeltaAngle(recordOldRotation.x, x);
                        var deltaAngleY = Mathf.DeltaAngle(recordOldRotation.y, y);
                        var deltaAngleZ = Mathf.DeltaAngle(recordOldRotation.z, z);


                        targetRecord.rotation = new Vector3(recordOldRotation.x + deltaAngleX,
                            recordOldRotation.y + deltaAngleY,
                            recordOldRotation.z + deltaAngleZ);
                    }
                }
            }
        }

        private void UpdateTargetRecord()
        {
            Debug.Log("UpdateTargetRecord");
            _dataList = _jasonManager.dataList;

            foreach (var data in _dataList)
            {
                List<ParserModel.DetectionList> detectionList = data.Value.detection_list;

                if (_dataList.Count > 0)
                {
                    targetRecord = FindRecordById(_targetID, detectionList);
                }
            }
        }


        private ParserModel.DetectionList FindRecordById(int targetID,
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
            _jasonManager = _panel.GetComponent<JasonManager>();

            Debug.Log("SaveDataToFile: " + _newFilePath);

            //TODO sow InputFieldEnterFolderNameForSafeNewData
            // _inputFieldEnterFolderNameForSafeNewData = _rotationMenu.transform
            //     .Find("InputFieldEnterFolderNameForSafeNewData").GetComponent<TMP_InputField>();
            // string inputFieldEnterFolderNameForSafeNewDataText = _inputFieldEnterFolderNameForSafeNewData.text;
            //
            // if (!inputFieldEnterFolderNameForSafeNewDataText.Equals(""))
            // {
            //     _newFolderName = inputFieldEnterFolderNameForSafeNewDataText;
            // }


            string folderPath = null;


            // Save the modified data back to the JSON filePath

            foreach (var root in _jasonManager.dataList)
            {
                _fileName = Path.GetFileName(root.Key);

                _newFileName = _fileName + "_3d_cods.json";
                folderPath = CrateNewDataFilePathAndDirectory();


                string updatedJsonString = JsonUtility.ToJson(root.Value, true);
                File.WriteAllText(folderPath + _newFileName, updatedJsonString);
            }


            Debug.Log("Changes saved to filePath: " + folderPath);

            // OpenNewFile();
        }

        private string CrateNewDataFilePathAndDirectory()
        {
            string newDataFilePath = dataFilePath.Replace(_fileName, "");
            string directoryName = Path.GetDirectoryName(newDataFilePath);

            if (directoryName != null) newDataFilePath = newDataFilePath.Replace(directoryName, "");

            Debug.Log(" SaveDataToFile newDataFilePath" + newDataFilePath); // Output: "This is a phrase to remove."


            string folderPath = Path.Combine(newDataFilePath, directoryName + _newFolderName);
            
            Debug.Log(" SaveDataToFile folderPath" + folderPath); // Output: "This is a phrase to remove."

            // _newFilePath = folderPath;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        public void LoadData()
        {
            Debug.Log("LoadData");
            if (targetRecord != null)
            {
                // Show the current rotation values in the menu
                ShowCurrentRotationValuesInTheMenu();
            }

            GameObject canvens = GameObject.Find("Canvas");
            _panel = canvens.transform.Find("Panel");
            _jasonManager = _panel.GetComponent<JasonManager>();

            dataFilePath = _jasonManager.dataFilePath;
            // Read JSON data from the filePath

            // Load the JSON data from the filePath path


            // _newFileName = Path.GetFileNameWithoutExtension(_dataFilePath) + "_3d_cods.json"; //TODO folder C10

            // Optionally, set the initial target record
            // _records = _targetRecord
            // _targetRecord = FindRecordById(_targetID, _records.detection_list);


            Vector3 recordRotation = targetRecord.rotation;

            if (recordRotation != null)
            {
                Vector3 targetRecordRotation = recordRotation;
                transform.rotation = Quaternion.Euler(targetRecordRotation);
            }
        }

        static string DeleteLastWord(string input)
        {
            int lastSpaceIndex = input.LastIndexOf(' ');

            // If there is no space or the string is empty, return the input as is
            if (lastSpaceIndex == -1 || string.IsNullOrWhiteSpace(input))
                return input;

            // Remove the last word and any subsequent whitespace characters
            string modifiedString = input.Substring(0, lastSpaceIndex);

            return modifiedString;
        }
    }
}