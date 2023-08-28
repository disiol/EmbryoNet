using UnityEngine;

public class ArrowRendering : MonoBehaviour
{
    public Material arrowMaterial; // Assign your arrow material in the Inspector

    Vector3[] vertices = new Vector3[]
    {
        new Vector3(-1.0f, -1.0f, 0.0f),
        new Vector3(1.0f, -1.0f, 0.0f),
        new Vector3(-1.0f,  1.0f, 0.0f),
        new Vector3(1.0f,  1.0f, 0.0f)
    };

    Vector2[] arrowPositions = new Vector2[]
    {
        new Vector2(320, 240),
        new Vector2(10.0f, 10.0f),
        new Vector2(630.0f, 470.0f)
    };

    Vector3[] arrowRotations = new Vector3[]
    {
        new Vector3(0.0f, 0.0f, 205.0f),
        new Vector3(60.0f, 0.0f, 0.0f),
        new Vector3(0.0f, -80.0f, 0.0f)
    };

    private void Start()
    {
        RenderArrowImage();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }

    private void RenderArrowImage()
    {
        if (arrowMaterial != null)
        {
            arrowMaterial.SetPass(0);

            RenderTexture activeRT = RenderTexture.active;

            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            Graphics.Blit(null, renderTexture, arrowMaterial);

            RenderTexture.active = renderTexture;

            GL.PushMatrix();
            GL.LoadOrtho();

            for (int i = 0; i < arrowPositions.Length; i++)
            {
                arrowMaterial.SetVector("_Position", arrowPositions[i]);
                arrowMaterial.SetVector("_Rotation", arrowRotations[i]);
                arrowMaterial.SetVector("_Resolution", new Vector2(Screen.width, Screen.height));

                GL.Begin(GL.QUADS);

                for (int j = 0; j < vertices.Length; j++)
                {
                    GL.Vertex(vertices[j]);
                }

                GL.End();
            }

            GL.PopMatrix();

            RenderTexture.active = activeRT;
            RenderTexture.active = null;
            Destroy(renderTexture);
        }
    }
}
