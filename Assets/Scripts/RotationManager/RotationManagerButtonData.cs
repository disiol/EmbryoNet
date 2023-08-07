using System.Collections.Generic;
using Models;
using SafeDadta;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationManagerButtonData : MonoBehaviour
    {
        public int targetID;


        // Set the path to the JSON filePath in the Inspector
        private RotationManager _rotationManager;
        private FrameManager _frameManager;
        private Transform _transformParentFrame;
        private SafeAndLoadData _safeAndLoadData;

        private void Start()
        {
            _safeAndLoadData = GameObject.Find("Canvas/Panel").GetComponent<SafeAndLoadData>();


            _rotationManager = transform.GetComponent<RotationManager>();
            transform.GetComponent<Button>().onClick.AddListener(OnButtonClick);
            _transformParentFrame = transform.parent;
            _frameManager = _transformParentFrame.transform.parent.GetComponent<FrameManager>();
        }


        private void OnButtonClick()
        {
            _safeAndLoadData.SafeCurrentId(targetID);
            _frameManager.DrawFrames();
            _rotationManager.LoadData();
        }
    }
}