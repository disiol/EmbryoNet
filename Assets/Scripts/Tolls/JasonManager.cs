using System.Collections.Generic;
using System.IO;
using Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tolls
{
    public class JasonManager : MonoBehaviour
    {
        public string dataFilePath;

        [HideInInspector] public List<JasonFilePathAndDataModel> dataList = new List<JasonFilePathAndDataModel>();
         public int currentOrderJsonFile;

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