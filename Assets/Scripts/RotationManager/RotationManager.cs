using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Models;
using SafeDadta;
using TMPro;
using Tolls;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationManager : MonoBehaviour
    {
        private GameObject _rotationCameraArrows;

        private ParserModel.Root _records;
        public string dataFilePath;

        private List<JasonFilePathAndDataModel> _dataList;


        private GameObject _rotationMenu;

        private TMP_InputField _menuXInput;
        private TMP_InputField _menuYInput;
        private TMP_InputField _menuZInput;

        private Button _saveChangesButton;


        private string _newFolderName = "_new_Data_with_rotations";

        private string _fileName;
        private string _newFileName;

        private bool _isMenuVisible = false;
        public ParserModel.DetectionList targetRecord;
        private int _targetID;


        // private string _newFilePath;

        private TMP_InputField _inputFieldEnterFolderNameForSafeNewData;

        private Transform _panel;
        private Transform _rightSide;
        private Transform _infoPanel;

        private JasonManager _jasonManager;
        private SafeAndLoadData _safeAndLoadData;
        private GameObject _popSafeUpWindow;
        private GameObject _popUpWindow;
        private string _inputFieldEnterFolderNameForSafeNewDataText;
        private string _folderPath;
        private GameObject _panelProgressBar;
        private int _jasonManagerCurrentOrderJsonFile;

        private void Start()
        {
            ShowMenu();
        }

        private void ShowMenu()
        {
            GameObject canvens = GameObject.Find("Canvas");
            _panel = canvens.transform.Find("Panel");
            _rightSide = _panel.transform.Find("RightSide");
            _infoPanel = _rightSide.transform.Find("InfoPanel");

            _rotationMenu = _infoPanel.Find("RotationMenu").GameObject();

            _rotationMenu.GetComponent<RotationController>().rotationManager =
                transform.GetComponent<RotationManager>();

            _popSafeUpWindow = canvens.transform.Find("PopSafeUpWindow").GameObject();
            _popUpWindow = canvens.transform.Find("PopUpWindow").GameObject();

            _panelProgressBar = canvens.transform.Find("PanelProgressBar").GameObject();


            _isMenuVisible = !_isMenuVisible;
            _rotationMenu.SetActive(_isMenuVisible);
            ButtonSafe();
            GetRotationMenuFileds();
        }


        private void ButtonSafe()
        {
            _saveChangesButton = _rotationMenu.transform.Find("ButtonSave").GetComponent<Button>();
            _saveChangesButton.onClick.AddListener(ShowPopSafeUpWindow);
        }

        private void ShowCurrentRotationValuesInTheMenu()
        {
            try
            {
                GetRotationMenuFileds();

                Vector3 targetRecordRotation = targetRecord.rotation;
                float x = targetRecordRotation.x;

                _menuXInput.text = x.ToString();


                _menuYInput.text = targetRecordRotation.y.ToString();


                _menuZInput.text = targetRecordRotation.z.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                PopUpWindowStatusShow("Please select an embryo");
            }
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

        public IEnumerator UpdateRotation()
        {
            _panelProgressBar.SetActive(true);
            //TODO refactoring
            _safeAndLoadData = gameObject.AddComponent<SafeAndLoadData>();

            _safeAndLoadData = GameObject.Find("/Canvas/Panel").GetComponent<SafeAndLoadData>();
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
                _jasonManagerCurrentOrderJsonFile = _jasonManager.currentOrderJsonFile;


                float x = float.Parse(_menuXInput.text);
                float y = float.Parse(_menuYInput.text);
                float z = float.Parse(_menuZInput.text);


                UpdateRotationInDataList(x, y, z);


                UpdateTargetRecord();
                ShowCurrentRotationValuesInTheMenu();
                SetNewRotation(x, y, z);
                _jasonManager.dataList = _dataList;

                _panelProgressBar.SetActive(false);

                // Hide the menu after saving changes
                // HideMenu();
            }
            else
            {
                _panelProgressBar.SetActive(false);

                //TODO exephen show
                Debug.Log("_exephen targetRecord = " + targetRecord);
            }

            yield return null;
        }

        private void GetCurentRotationArrows()
        {
            Debug.Log("GetCurentRotationArrows");

            Transform findCamersArrows = GameObject.Find("Arrows").transform;

            for (int i = 0; i < findCamersArrows.childCount; i++)
            {
                Transform rotationCameraArrows = findCamersArrows.GetChild(i);

                String currentCameraArrowsName = rotationCameraArrows.name;
                String targetCameraArrowsName = "Arrows_" + _targetID;

                if (targetCameraArrowsName == currentCameraArrowsName)
                {
                    _rotationCameraArrows = rotationCameraArrows.gameObject;
                }
            }
        }

        private void SetNewRotation(float x, float y, float z)
        {
            Debug.Log("SetNewRotation");
            GetCurentRotationArrows();
            Vector3 newTargetRecordRotation = new Vector3(x, y, z);
            try
            {
                _rotationCameraArrows.transform.rotation = Quaternion.Euler(newTargetRecordRotation);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                PopUpWindowStatusShow("Please select an embryo");
            }
        }

        private void UpdateRotationInDataList(float x, float y, float z)
        {
            Debug.Log("UpdateRotationInDataList");

            for (int i = _jasonManagerCurrentOrderJsonFile; i < _dataList.Count; i++)
            {
                List<ParserModel.DetectionList> detectionList = _dataList[i].data.detection_list;

                if (_dataList.Count > 0)
                {
                    targetRecord = FindRecordById(_targetID, detectionList);


                    if (targetRecord != null)
                    {
                        Vector3 recordOldRotation = targetRecord.rotation;

                        var deltaAngleX = Mathf.DeltaAngle(recordOldRotation.x, x);
                        var deltaAngleY = Mathf.DeltaAngle(recordOldRotation.y, y);
                        var deltaAngleZ = Mathf.DeltaAngle(recordOldRotation.z, z);


                        Vector3 newTargetRecordRotation = new Vector3(recordOldRotation.x + deltaAngleX,
                            recordOldRotation.y + deltaAngleY,
                            recordOldRotation.z + deltaAngleZ);
                        targetRecord.rotation = newTargetRecordRotation;

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
                List<ParserModel.DetectionList> detectionList = data.data.detection_list;

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


            // Save the modified data back to the JSON filePath

            StartCoroutine(CrateFilesFromData());


            string changesSavedToFilepath = "Changes saved to filePath: " + _folderPath;
            Debug.Log(changesSavedToFilepath);

            PopUpWindowStatusShow(changesSavedToFilepath);


            // OpenNewFile();
        }

        private IEnumerator CrateFilesFromData()
        {
            //TODO ad adata ty failes
            foreach (var root in _jasonManager.dataList)
            {
                _fileName = Path.GetFileNameWithoutExtension(root.jsonFilePath);

                _newFileName = _fileName + "_3d_cods.json";
                _folderPath = CrateNewDataFilePathAndDirectory();


                string updatedJsonString = JsonUtility.ToJson(root);
                string filePath = Path.Combine(_folderPath, _newFileName);
                File.WriteAllText(filePath, updatedJsonString);
            }

            yield return null;
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


            if (targetRecord != null)
            {
                Vector3 recordRotation = targetRecord.rotation;

                if (recordRotation != null)
                {
                    Vector3 targetRecordRotation = recordRotation;
                    transform.rotation = Quaternion.Euler(targetRecordRotation);
                }
            }
        }

        private void ShowPopSafeUpWindow()
        {
            _popSafeUpWindow.SetActive(true);
            _inputFieldEnterFolderNameForSafeNewData = _popSafeUpWindow.transform
                .Find("InputFieldEnterFolderNameForSafeNewData")
                .GetComponent<TMP_InputField>();


            _inputFieldEnterFolderNameForSafeNewDataText = _inputFieldEnterFolderNameForSafeNewData.text;


            Button buttonOk = _popSafeUpWindow.transform.Find("ButtonOk").GetComponent<Button>();
            buttonOk.onClick.AddListener(SaveDataToFile);
        }


        private void PopUpWindowStatusShow(string text)
        {
            GameObject canvens = GameObject.Find("Canvas");
            _popUpWindow = canvens.transform.Find("PopUpWindow").GameObject();
            _popUpWindow.SetActive(true);
            TextMeshProUGUI statusText = _popUpWindow.transform
                .Find("StatusText")
                .GetComponent<TextMeshProUGUI>();

            statusText.text = text;
        }

        private string CrateNewDataFilePathAndDirectory()
        {
            string folderPath = "";

            folderPath = SetFolderName();


            Debug.Log(" SaveDataToFile folderPath" + folderPath); // Output: "This is a phrase to remove."

            // _newFilePath = folderPath;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        private string SetFolderName()
        {
            string folderPath;
            if (!_inputFieldEnterFolderNameForSafeNewDataText.Equals(""))
            {
                _newFolderName = _inputFieldEnterFolderNameForSafeNewDataText;

                folderPath = DeleteLastWord(dataFilePath) + _newFolderName;
            }
            else
            {
                folderPath = dataFilePath + _newFolderName;
            }

            return folderPath;
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