using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Models;
using SafeDadta;
using SFB;
using TMPro;
using Tolls;
using UnityEngine.Serialization;

public class FileExplorer : MonoBehaviour
{
    [SerializeField] private string imagesFolderName;

    private TMP_Text _popUpWindowStatusText;

    [SerializeField] private Button loadButton;

    // public TMP_Text folderPathText;
    [SerializeField] private GameObject folderButtonPrefab;
    [SerializeField] private GameObject jsonButtonPrefab;
    [SerializeField] private Transform filesList;

    [SerializeField] private GameObject panelProgressBar;
    [SerializeField] private Image imageObject;

    private string _path;

    private string _imageFolderPath;
    private GameObject _popUpWindow;
    private JasonManager _jasonManager;
    private SafeAndLoadData _safeAndLoadData;


    private void Start()
    {
        loadButton.onClick.AddListener(OnLoadButtonClicked);
        _jasonManager = transform.GetComponent<JasonManager>();

        GameObject canvas = GameObject.Find("Canvas");
        _popUpWindow = canvas.transform.Find("PopUpWindow").gameObject;
        _popUpWindowStatusText = _popUpWindow.transform.Find("StatusText").gameObject.GetComponent<TextMeshProUGUI>();

        _safeAndLoadData = gameObject.GetComponent<SafeAndLoadData>();
    }

    private void OnLoadButtonClicked()
    {
        panelProgressBar.SetActive(true);

        StandaloneFileBrowser.OpenFolderPanelAsync("Select Folder", "", true, (string[] paths) =>
        {
            WriteResult(paths);
            if (!string.IsNullOrEmpty(_path))
            {
                ShowFoldersAndJsonFiles(_path);
            }
        });
    }


    private void ShowFoldersAndJsonFiles(string folderPath)
    {
        Debug.Log("Selected Folder: " + folderPath);

        _imageFolderPath = Path.Combine(folderPath, imagesFolderName);

        if (!Directory.Exists(_imageFolderPath))
        {
            Debug.Log("_imageFolderPath: " + _imageFolderPath);

            PopUpWindowShow("Image directory not found.");
            ClearButtons();
            return;
        }

        ClearButtons();

        string[] subFolders = Directory.GetDirectories(folderPath);

        foreach (string folder in subFolders)
        {
            string fileName = Path.GetFileName(folder);


            if (fileName != imagesFolderName)
            {
                StartCoroutine(ShowFolders(folder, fileName));
            }
        }
    }

    private IEnumerator ShowFolders(string folder, string fileName)
    {
        panelProgressBar.SetActive(true);
        GameObject folderButton = Instantiate(folderButtonPrefab, filesList);
        folderButton.GetComponent<PatchContainer>().folderPath = folder;

        TMP_Text buttonText = folderButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = fileName;
        Button folderBtn = folderButton.GetComponent<Button>();
        folderBtn.onClick.AddListener(() =>
            StartCoroutine(ShowJsonFilesInFolder(folderButton.GetComponent<PatchContainer>().folderPath)));
        panelProgressBar.SetActive(false);

        yield return null;
    }

    private void PopUpWindowShow(string text)
    {
        _popUpWindow.SetActive(true);
        _popUpWindowStatusText.text = text;
    }

    private IEnumerator ShowJsonFilesInFolder(string folderPath)
    {
        panelProgressBar.SetActive(true);

        ResetData();

        string[] jsonFiles = Directory.GetFiles(folderPath);

        for (var index = 0; index < jsonFiles.Length; index++)
        {
            var file = jsonFiles[index];
            GameObject jsonButton = Instantiate(jsonButtonPrefab, filesList);
            TMP_Text buttonText = jsonButton.GetComponentInChildren<TMP_Text>();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

            buttonText.text = fileNameWithoutExtension;

            PatchContainer patchContainer = jsonButton.GetComponent<PatchContainer>();
            patchContainer.filePath = file;
            patchContainer.folderPath = folderPath;
            patchContainer.fileOrder = index;

            _jasonManager.LoadDataFromJsonFiles(file);


            Button jsonBtn = jsonButton.GetComponent<Button>();
            jsonBtn.onClick.AddListener(() => FindImageByName(patchContainer.fileOrder));
        }

        _jasonManager.dataFilePath = folderPath;

        panelProgressBar.SetActive(false);

        yield return null;
    }

    private void ResetData()
    {
        _safeAndLoadData.SafeCurrentId(0);
        ClearButtons();
    }


    private void FindImageByName(int opderJsonFile)

    {
        ResetCurrentId();
        
        List<JasonFilePathAndDataModel> jasonManagerDataList = _jasonManager.dataList;
        ParserModel.Root records = jasonManagerDataList[opderJsonFile].data;

        string imageName = records.source_name;
        string imagePath =
            Path.Combine(_imageFolderPath,
                imageName);

        if (File.Exists(imagePath))
        {
            SaveCurrentOrderJsonFile(opderJsonFile);
            SetColorOfSelectedButton();

            // Open the image or do something with it
            StartCoroutine(OpenImage(records, imagePath));
        }
        else
        {
            PopUpWindowShow("Image not found: " + imageName);
            Debug.Log("Image not found: " + imagePath);
        }
    }

    private void ResetCurrentId()
    {
        _safeAndLoadData.SafeCurrentId(0);
    }

    private void SaveCurrentOrderJsonFile(int opderJsonFile)
    {
        _jasonManager.currentOrderJsonFile = opderJsonFile;
    }


    private IEnumerator OpenImage(ParserModel.Root jsonfileDadta, string imagePath)
    {
        panelProgressBar.SetActive(true);

        Debug.Log("Open the image: " + imagePath);

        if (!string.IsNullOrEmpty(imagePath))
        {
            FrameManager frameManager = imageObject.GetComponent<FrameManager>();

            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            CrateSprite(texture);

            DrawFrames(jsonfileDadta, frameManager);
        }

        panelProgressBar.SetActive(false);
        yield return null;
    }

    private void DrawFrames(ParserModel.Root jsonfileDadta, FrameManager frameManager)
    {
        frameManager.detectionData = jsonfileDadta;
        frameManager.dataFilePath = _jasonManager.dataFilePath;
        frameManager.DrawFrames();
    }

    private void CrateSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        imageObject.sprite = sprite;
    }

    private void SetColorOfSelectedButton()
    {
        for (int i = 0; i < filesList.childCount; i++)
        {
            Button button = filesList.GetChild(i).GetComponent<Button>();

            int fileOrder = button.GetComponent<PatchContainer>().fileOrder;

            Image buttonImage = button.GetComponentInChildren<Image>();

            if (fileOrder.Equals(_jasonManager.currentOrderJsonFile))
            {
                buttonImage.color = Color.red;
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }
    }

    private void ClearButtons()
    {
        foreach (Transform button in filesList)
        {
            Destroy(button.gameObject);
        }
    }


    public void WriteResult(string[] paths)
    {
        if (paths.Length == 0)
        {
            return;
        }

        _path = "";
        foreach (var p in paths)
        {
            _path += p;
        }
    }

    public void WriteResult(string path)
    {
        _path = path;
    }
}