using UnityEngine;

namespace SafeDadta
{
    public  class SafeAndLoadData : MonoBehaviour, ISafe
    {
      private readonly string _currentId = "CurrentId";

 
        public void SafeCurrentId(int currentId )
        {
            PlayerPrefs.SetInt(this._currentId, currentId);
            PlayerPrefs.Save();
        }

        public  int LoadCurrentId()
        {
            return PlayerPrefs.GetInt(this._currentId);
        }
    }
}