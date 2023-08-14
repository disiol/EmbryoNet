using UnityEngine;
using UnityEngine.UI;

namespace Shelders
{
    public class ArrowShaderExample : MonoBehaviour
    {
        public Shader arrowShader; // Reference to the custom shader
        public Vector3 rotation = new Vector3(0f, 0f, 90f); // Rotation for the arrows
        public Vector3 position = new Vector3(400f, 225f, 0f); // Position for the arrows

        void Start()
        {
            // Create a new material using the custom shader
            Material arrowMaterial = new Material(arrowShader);

            // Set the shader properties
            arrowMaterial.SetVector("_Rotation", rotation);
            arrowMaterial.SetVector("_Position", position);
            arrowMaterial.SetVector("_Resolution", new Vector3(Screen.width, Screen.height, 0f));

            // Apply the material to the GameObject's renderer
            GetComponent<Image>().material = arrowMaterial;
        }
    }
}