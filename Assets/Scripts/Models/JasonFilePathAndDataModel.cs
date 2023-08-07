namespace Models
{
    [System.Serializable]
    public class JasonFilePathAndDataModel
    {
        public string jsonFilePath;
        public ParserModel.Root data;

        public JasonFilePathAndDataModel(string jsonFilePath, ParserModel.Root data)
        {
            this.jsonFilePath = jsonFilePath;
            this.data = data;
        }
    }
}