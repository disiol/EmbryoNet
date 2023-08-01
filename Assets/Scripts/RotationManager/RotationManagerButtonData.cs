using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationManagerButtonData : MonoBehaviour
    {
        public int targetID;
        [HideInInspector] public string dataFilePath;
        [HideInInspector] public ParserModel.DetectionList detection;


        // Set the path to the JSON filePath in the Inspector
        private RotationManager _rotationManager;
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
            _frameManager.DrawFrames();


            _rotationManager.ShowMenu();
            _rotationManager.LoadData();
        }
    }
}