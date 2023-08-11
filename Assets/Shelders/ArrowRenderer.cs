using UnityEngine;

namespace Shelders
{
    public class ArrowRenderer : MonoBehaviour
    {
        public ComputeShader arrowComputeShader;
        public Material arrowMaterial;
        public Texture2D arrowTexture;
        public RenderTexture renderTexture;


        private Vector2[] arrowPositions = new Vector2[]
        {
            new Vector2(Screen.width / 2, Screen.height / 2),
            new Vector2(10.0f, 10.0f),
            new Vector2(Screen.width - 10.0f, Screen.height - 10.0f)
        };

        private Vector3[] arrowRotations = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, 30.0f),
            new Vector3(30.0f, 0.0f, 0.0f),
            new Vector3(0.0f, -30.0f, 0.0f)
        };


        private void Start()
        {
            RenderArrowsToTexture();
        }

        private void RenderArrowsToTexture()
        {
             renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            Graphics.SetRenderTarget(renderTexture);
            GL.Clear(true, true, Color.clear);
            

            foreach (var arrowPosition in arrowPositions)
            {
                for (int i = 0; i < arrowRotations.Length; i++)
                {
                    arrowComputeShader.SetVector("_Position", arrowPosition);
                    arrowComputeShader.SetVector("_Rotation", arrowRotations[i]);
                    
                    Graphics.Blit(arrowTexture, renderTexture, arrowMaterial);
                }
            }

            Graphics.SetRenderTarget(null);

            // Display the rendered texture on a quad
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.position = new Vector3(0.0f, 0.0f, 5.0f);
            quad.transform.localScale = new Vector3(Screen.width, Screen.height, 1.0f);
            quad.GetComponent<Renderer>().material.mainTexture = renderTexture;
        }
    }
}