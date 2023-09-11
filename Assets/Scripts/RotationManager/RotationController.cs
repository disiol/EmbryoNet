using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RotationManager
{
    public class RotationController : MonoBehaviour
    {
        // Initial rotation values
        [HideInInspector] public Vector3 previousRotation = Vector3.zero;

        // Current rotation values
        [HideInInspector] public Vector3 currentRotation = Vector3.zero;

        // Reference to UI buttons for increasing/decreasing rotation
        [Header("RotationX")] [SerializeField] private Button increaseXButton;
        [SerializeField] private TMP_InputField menuXInput;
        [SerializeField] private Button decreaseXButton;

        [Header("RotationY")] [SerializeField] private Button increaseYButton;
        [SerializeField] private TMP_InputField menuYInput;
        [SerializeField] private Button decreaseYButton;

        [Header("RotationZ")] [SerializeField] private Button increaseZButton;
        [SerializeField] private TMP_InputField menuZInput;
        [SerializeField] private Button decreaseZButton;

        [Header("Rotation")] [SerializeField] private float currentRotationStep;
        [SerializeField] private float minRotation;
        [SerializeField] private float maxRotation;

        [HideInInspector] public RotationManager rotationManager;

        private void Start()
        {
            increaseXButton.onClick.AddListener(IncreaseXRotation);
            decreaseXButton.onClick.AddListener(DecreaseXRotation);

            increaseYButton.onClick.AddListener(IncreaseYRotation);
            decreaseYButton.onClick.AddListener(DecreaseYRotation);

            increaseZButton.onClick.AddListener(IncreaseZRotation);
            decreaseZButton.onClick.AddListener(DecreaseZRotation);

            menuXInput.onValueChanged.AddListener(OnXValueChanged);
            menuYInput.onValueChanged.AddListener(OnYValueChanged);
            menuZInput.onValueChanged.AddListener(OnZValueChanged);
        }


        public void Update()
        {
            RotatiomByKese();
        }

        private void RotatiomByKese()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DecreaseZRotation();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                IncreaseZRotation();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                DecreaseYRotation();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                IncreaseYRotation();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                DecreaseXRotation();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                IncreaseXRotation();
            }
        }


        // Handle X, Y, and Z rotation value changes
        private void OnXValueChanged(string newValue)
        {
            float.TryParse(newValue, out float xValue);
            currentRotation.x = xValue;
            UpdateRotation();
        }

        private void OnYValueChanged(string newValue)
        {
            float.TryParse(newValue, out float yValue);
            currentRotation.y = yValue;
            UpdateRotation();
        }

        private void OnZValueChanged(string newValue)
        {
            float.TryParse(newValue, out float zValue);
            currentRotation.z = zValue;
            UpdateRotation();
        }

        // Update rotation values for each axis
        private void IncreaseXRotation()
        {
            currentRotation.x += currentRotationStep;
            UpdateRotation();
        }

        private void DecreaseXRotation()
        {
            currentRotation.x -= currentRotationStep;
            UpdateRotation();
        }

        private void IncreaseYRotation()
        {
            currentRotation.y += currentRotationStep;
            UpdateRotation();
        }

        private void DecreaseYRotation()
        {
            currentRotation.y -= currentRotationStep;
            UpdateRotation();
        }

        private void IncreaseZRotation()
        {
            currentRotation.z += currentRotationStep;
            UpdateRotation();
        }

        private void DecreaseZRotation()
        {
            currentRotation.z -= currentRotationStep;
            UpdateRotation();
        }

        public void ResetData()
        {
            menuXInput.text = "0";
            menuYInput.text = "0";
            menuZInput.text = "0";
        }


        // Apply the 0-360 degree constraint and update the object's rotation
        private void UpdateRotation()
        {
            currentRotation.x = Mathf.Clamp(currentRotation.x, minRotation, maxRotation);
            menuXInput.text = currentRotation.x.ToString();

            currentRotation.y = Mathf.Clamp(currentRotation.y, minRotation, maxRotation);
            menuYInput.text = currentRotation.y.ToString();

            currentRotation.z = Mathf.Clamp(currentRotation.z, minRotation, maxRotation);
            menuZInput.text = currentRotation.z.ToString();

            StartCoroutine(rotationManager.UpdateRotation());
        }
    }
}