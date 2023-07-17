using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;

public class FileExplorer : MonoBehaviour
{
    public TMP_Text statusText;
    public Button loadButton;
    public TMP_Text folderPathText;
    public ScrollRect folderListScrollRect;
    public GameObject folderButtonPrefab;
    public GameObject jsonButtonPrefab;
    public Transform contentTransform;

    private const int MAX_PATH = 260;

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct BROWSEINFO
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public IntPtr pszDisplayName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszTitle;
        public uint ulFlags;
        public IntPtr lpfn;
        public IntPtr lParam;
        public int iImage;
    }

    private void Start()
    {
        loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    private void OnLoadButtonClicked()
    {
        string folderPath = ShowFolderPickerDialog();
        if (!string.IsNullOrEmpty(folderPath))
        {
            ShowFoldersAndJSONFiles(folderPath);
        }
    }

    private string ShowFolderPickerDialog()
    {
        BROWSEINFO bi = new BROWSEINFO();
        bi.hwndOwner = IntPtr.Zero;
        bi.pidlRoot = IntPtr.Zero;
        bi.lpszTitle = "Select a folder";
        bi.ulFlags = 0x0001 | 0x0010; // BIF_NEWDIALOGSTYLE | BIF_RETURNONLYFSDIRS

        IntPtr pidl = SHBrowseForFolder(ref bi);
        if (pidl != IntPtr.Zero)
        {
            StringBuilder buffer = new StringBuilder(MAX_PATH);
            if (SHGetPathFromIDList(pidl, buffer))
            {
                return buffer.ToString();
            }
        }

        return null;
    }

    private void ShowFoldersAndJSONFiles(string folderPath)
    {
        folderPathText.text = "Selected Folder: " + folderPath;

        string imageFolderPath = Path.Combine(folderPath, "image");
        if (!Directory.Exists(imageFolderPath))
        {
            statusText.text = "Image directory not found.";
            ClearButtons();
            return;
        }

        statusText.text = "";
        ClearButtons();

        string[] subFolders = Directory.GetDirectories(imageFolderPath);
        foreach (string folder in subFolders)
        {
            GameObject folderButton = Instantiate(folderButtonPrefab, contentTransform);
            TMP_Text buttonText = folderButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = Path.GetFileName(folder);
            Button folderBtn = folderButton.GetComponent<Button>();
            folderBtn.onClick.AddListener(() => ShowJSONFilesInFolder(folder));
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
        string folderPath = folderPathText.text.Substring("Selected Folder: ".Length);
        string imageFolderPath = Path.Combine(folderPath, "image");

        string imagePath = Path.Combine(imageFolderPath, imageName + ".png");
        if (File.Exists(imagePath))
        {
            // Open the image or do something with it
        }
        else
        {
            statusText.text = "Image not found: " + imageName;
        }
    }

    private void ClearButtons()
    {
        foreach (Transform button in contentTransform)
        {
            Destroy(button.gameObject);
        }
    }
}
