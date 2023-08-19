using UnityEngine;
using UnityEngine.UI;

namespace Shelders
{
    public class ObjectSizeSetter : MonoBehaviour
    {
        public Shader shader; // Assign the shader in the Inspector
        public float width; // Desired width of the image
        public float height; // Desired height of the image
        public Vector2 imageCenter;

        private void Start()
        {
            if (shader != null)
            {
                // Create a new material using the provided shader
                Material newMaterial = new Material(shader);

                // Get the size of the RectTransform
                Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
                Vector2 canvasScale = new Vector2(transform.localScale.x, transform.localScale.y);

                Vector2 finalScale = new Vector2(sizeDelta.x * canvasScale.x, sizeDelta.y * canvasScale.y);
                width = finalScale.x;
                height = finalScale.y;

                // Get the UI Image component on this GameObject
                Image imageComponent = GetComponent<Image>();

                if (imageComponent != null)
                {
                    // Assign the new material to the UI Image component
                    imageComponent.material = newMaterial;

                    // // Calculate object size and adjust line thickness based on size here
                    // float maxExtent = Mathf.Max(width, height);
                    // float lineThickness = maxExtent * lineThicknessScale;
                    // newMaterial.SetFloat("_LineThickness", lineThickness);

                    // Calculate the center of the UI Image
                    
                     imageCenter = new Vector2(width / 2, height / 2);;
                    

                    // // Adjust the alpha of the material to control visibility
                    // Color materialColor = newMaterial.color;
                    // materialColor.a = 0.5f; // Set the desired alpha value
                    // newMaterial.color = materialColor;
                }
                else
                {
                    Debug.LogWarning("UI Image component not found on this GameObject.");
                }
            }
            else
            {
                Debug.LogWarning("Shader not assigned.");
            }
        }
    }
}