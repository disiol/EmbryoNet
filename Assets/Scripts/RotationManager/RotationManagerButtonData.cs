using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationManagerButtonData : MonoBehaviour
    {
        [HideInInspector] public int targetID;
        [HideInInspector] public string jsonFilePath; // Set the path to the JSON file in the Inspector
         private  RotationManager _rotationManager;
         private FrameManager _frameManager;
         private Transform _transformParentFrame;

         private void Start()
        {
            _rotationManager = transform.GetComponent<RotationManager>();
            transform.GetComponent<Button>().onClick.AddListener(OnButtonClick);
            _transformParentFrame = transform.parent;
            _frameManager = _transformParentFrame.transform.parent.GetComponent<FrameManager>();
        }


        private void OnButtonClick()
        {
            _frameManager.selectedDetectionID = targetID;
            _rotationManager.SetTargetID(targetID);
            _frameManager.DrawFrames();
            _rotationManager.SetDataFilePath(jsonFilePath);
            _rotationManager.ShowMenu();
        }
    }
}