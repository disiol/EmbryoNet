using System.Collections.Generic;
using System.IO;
using Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tolls
{
    public class JasonManager : MonoBehaviour
    {
        [FormerlySerializedAs("dataFilePath")] public string curentFolderPath;

        public List<JasonFilePathAndDataModel> dataList = new List<JasonFilePathAndDataModel>();
        [HideInInspector] public int currentOrderJsonFile;

        private ParserModel.Root OpenJasonFile(string jsonfilePath)
        {
            string jsonFileContent = File.ReadAllText(jsonfilePath);
            ParserModel.Root records = JsonUtility.FromJson<ParserModel.Root>(jsonFileContent);
            return records;
        }

        public void LoadDataFromJsonFiles(string jsonfilePath)
        {
            ParserModel.Root data = OpenJasonFile(jsonfilePath);
            dataList.Add(new JasonFilePathAndDataModel(jsonfilePath, data));
        }
    }
}