using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Text;
using SFB;
using TMPro;

public class FileExplorer : MonoBehaviour
{
    [SerializeField] private string imagesFolderName;

    public TMP_Text statusText;
    public Button loadButton;
    // public TMP_Text folderPathText;
    [SerializeField] private ScrollRect folderListScrollRect;
    [SerializeField] private GameObject folderButtonPrefab;
    [SerializeField] private GameObject jsonButtonPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Image imageObject;

    private string _path;
    private string _imageFolderPath;

    private const int MAX_PATH = 260;
    
   

    private void Start()
    {
        loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    private void OnLoadButtonClicked()
    {
        
        StandaloneFileBrowser.OpenFolderPanelAsync("Select Folder", "", true, (string[] paths) =>
        {
            WriteResult(paths);
            if (!string.IsNullOrEmpty(_path))
            {
                ShowFoldersAndJSONFiles(_path);
            }
            
        });
        
    }

  
    private void ShowFoldersAndJSONFiles(string folderPath)
    {
        Debug.Log("Selected Folder: " + folderPath);
        statusText.text = "Selected Folder: " + folderPath;

        _imageFolderPath = Path.Combine(folderPath, imagesFolderName);
        if (!Directory.Exists(_imageFolderPath))
        {
            Debug.Log("_imageFolderPath: " + _imageFolderPath);

            statusText.text = "Image directory not found.";
            ClearButtons();
            return;
        }

        statusText.text = "";
        ClearButtons();

        string[] subFolders = Directory.GetDirectories(folderPath);
        foreach (string folder in subFolders)
        {
            if (!folderPath.Contains(imagesFolderName))
            {
                GameObject folderButton = Instantiate(folderButtonPrefab, contentTransform);
                folderButton.GetComponent<PatchContainer>().patch = folder;

            
                TMP_Text buttonText = folderButton.GetComponentInChildren<TMP_Text>();
                buttonText.text = Path.GetFileName(folder);
              
                Button folderBtn = folderButton.GetComponent<Button>();
                folderBtn.onClick.AddListener(() => ShowJSONFilesInFolder(folderButton.GetComponent<PatchContainer>().patch));
            }
        }
    }

    private void ShowJSONFilesInFolder(string folderPath)
    {
        ClearButtons();

        string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
        foreach (string file in jsonFiles)
        {
            GameObject jsonButton = Instantiate(jsonButtonPrefab, contentTransform);
            TMP_Text buttonText = jsonButton.GetComponentInChildren<TMP_Text>();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            buttonText.text = fileNameWithoutExtension;
            Button jsonBtn = jsonButton.GetComponent<Button>();
            jsonBtn.onClick.AddListener(() => FindImageByName(fileNameWithoutExtension));
        }
    }

    private void FindImageByName(string imageName)
    {

        string imagePath = Path.Combine(_imageFolderPath, imageName + ".jpg");
        if (File.Exists(imagePath))
        {
            OpenImage(imagePath);
            // Open the image or do something with it
        }
        else
        {
            statusText.text = "Image not found: " + imageName;
            Debug.Log("Image not found: " + imagePath);

        }
    }

    private void OpenImage(string imagePath)
    {
        Debug.Log("Open the image: " + imagePath);

        if (!string.IsNullOrEmpty(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            imageObject.sprite = sprite;
        }
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
