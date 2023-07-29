using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Models;
using SFB;
using TMPro;
using Tolls;

public class FileExplorer : MonoBehaviour
{
    [SerializeField] private string imagesFolderName;

    private TMP_Text _statusText;

    [SerializeField] private Button loadButton;

    // public TMP_Text folderPathText;
    [SerializeField] private GameObject folderButtonPrefab;
    [SerializeField] private GameObject jsonButtonPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Image imageObject;

    private string _path;

    private string _imageFolderPath;
    private GameObject _popUpWindow;
    private JasonManager _jasonManager;


    private void Start()
    {
        loadButton.onClick.AddListener(OnLoadButtonClicked);
        _jasonManager = new JasonManager();

        GameObject canvas = GameObject.Find("Canvas");
        _popUpWindow = canvas.transform.Find("PopUpWindow").gameObject;
        _statusText = _popUpWindow.transform.Find("StatusText").gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void OnLoadButtonClicked()
    {
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
                GameObject folderButton = Instantiate(folderButtonPrefab, contentTransform);
                folderButton.GetComponent<PatchContainer>().folderPath = folder;

                TMP_Text buttonText = folderButton.GetComponentInChildren<TMP_Text>();
                buttonText.text = fileName;
                Button folderBtn = folderButton.GetComponent<Button>();
                folderBtn.onClick.AddListener(() =>
                    ShowJSONFilesInFolder(folderButton.GetComponent<PatchContainer>().folderPath));
            }
        }
    }

    private void PopUpWindowShow(string text)
    {
        _popUpWindow.SetActive(true);
        _statusText.text = text;
    }

    private void ShowJSONFilesInFolder(string folderPath)
    {
        //TODO зробити масив даних з jsonFiles, та правити їх.
        //коли нажимаеш на кноку сафе створюется нова пака з файлами
        ClearButtons();
        string[] jsonFiles = Directory.GetFiles(folderPath);

        for (var index = 0; index < jsonFiles.Length; index++)
        {
            var file = jsonFiles[index];
            GameObject jsonButton = Instantiate(jsonButtonPrefab, contentTransform);
            TMP_Text buttonText = jsonButton.GetComponentInChildren<TMP_Text>();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

            buttonText.text = fileNameWithoutExtension;

            jsonButton.GetComponent<PatchContainer>().order = index;
            jsonButton.GetComponent<PatchContainer>().folderPath = folderPath;
            _jasonManager.LoadDataFromJsonFiles(file);


            Button jsonBtn = jsonButton.GetComponent<Button>();
            jsonBtn.onClick.AddListener(() => FindImageByName(jsonButton.GetComponent<PatchContainer>().order));
        }
    }


    private void FindImageByName(int opderjsonfile)

    {
        ParserModel.Root records = _jasonManager.dataList[opderjsonfile];

        string imageName = records.source_name;
        string imagePath =
            Path.Combine(_imageFolderPath,
                imageName);

        if (File.Exists(imagePath))
        {
            OpenImage(records, imagePath);
            // Open the image or do something with it
        }
        else
        {
            _statusText.text = "Image not found: " + imageName;
            Debug.Log("Image not found: " + imagePath);
        }
    }


    private void OpenImage(ParserModel.Root jsonfileDadta, string imagePath)
    {
        Debug.Log("Open the image: " + imagePath);

        if (!string.IsNullOrEmpty(imagePath))
        {
            FrameManager frameManager = imageObject.GetComponent<FrameManager>();

            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            frameManager.detectionData = jsonfileDadta;

            CrateSprite(texture);
            frameManager.DrawFrames();
        }
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