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

    private TMP_Text _statusText;

    [SerializeField] private Button loadButton;

    // public TMP_Text folderPathText;
    [SerializeField] private GameObject folderButtonPrefab;
    [SerializeField] private GameObject jsonButtonPrefab;
    [SerializeField] private Transform contentTransform;
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
        _statusText = _popUpWindow.transform.Find("StatusText").gameObject.GetComponent<TextMeshProUGUI>();

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
        GameObject folderButton = Instantiate(folderButtonPrefab, contentTransform);
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
        _statusText.text = text;
    }

    private IEnumerator ShowJsonFilesInFolder(string folderPath)
    {
        panelProgressBar.SetActive(true);
        _safeAndLoadData.SafeCurrentId(0);
        ClearButtons();
        string[] jsonFiles = Directory.GetFiles(folderPath);

        for (var index = 0; index < jsonFiles.Length; index++)
        {
            var file = jsonFiles[index];
            GameObject jsonButton = Instantiate(jsonButtonPrefab, contentTransform);
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


    private void FindImageByName(int opderJsonFile)

    {
        List<JasonFilePathAndDataModel> jasonManagerDataList = _jasonManager.dataList;
        ParserModel.Root records = jasonManagerDataList[opderJsonFile].data;

        string imageName = records.source_name;
        string imagePath =
            Path.Combine(_imageFolderPath,
                imageName);

        if (File.Exists(imagePath))
        {
            _jasonManager.currentOrderJsonFile = opderJsonFile;
            // Open the image or do something with it
            StartCoroutine(OpenImage(records, imagePath));
        }
        else
        {
            _statusText.text = "Image not found: " + imageName;
            Debug.Log("Image not found: " + imagePath);
        }
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

            frameManager.detectionData = jsonfileDadta;
            frameManager.dataFilePath = _jasonManager.dataFilePath;

            CrateSprite(texture);
            frameManager.DrawFrames();
        }

        panelProgressBar.SetActive(false);
        yield return null;
    }

    private void CrateSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        imageObject.sprite = sprite;
    }

    private void ClearButtons()
    {
        foreach (Transform button in contentTransform)
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