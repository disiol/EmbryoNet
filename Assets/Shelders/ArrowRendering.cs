using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRendering : MonoBehaviour
{
    public ComputeShader arrowComputeShader;
    public RenderTexture arrowRenderTexture;
    public Material displayMaterial; // Material to display the computed result on a quad
    [SerializeField] private Vector4 rotation;
    [SerializeField] private Vector4 position;

    private void Start()
    {
        InitializeComputeShader();
    }

    private void Update()
    {
     
        arrowComputeShader.SetVector("_Rotation", rotation);
        arrowComputeShader.SetVector("_Position", position);
        arrowComputeShader.SetVector("_Resolution", new Vector2(arrowRenderTexture.width, arrowRenderTexture.height));
        int kernelIndex = arrowComputeShader.FindKernel("CSMain"); 

        int threadGroupsX = Mathf.CeilToInt(arrowRenderTexture.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(arrowRenderTexture.height / 8.0f);

        arrowComputeShader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, 1);

        // Display the computed result on a quad using the displayMaterial
        Graphics.Blit(arrowRenderTexture, null, displayMaterial);
    }

    private void OnDestroy()
    {
        if (arrowRenderTexture != null)
            arrowRenderTexture.Release();
    }

    private void InitializeComputeShader()
    {
        arrowRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        arrowRenderTexture.enableRandomWrite = true;
        arrowRenderTexture.Create();

        arrowComputeShader.SetTexture(0, "Result", arrowRenderTexture);
    }
}