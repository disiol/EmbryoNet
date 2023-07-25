using Models;
using UnityEngine;
using UnityEngine.UI;

public class BoundingBoxDrawer : MonoBehaviour
{
    public Texture2D imageTexture; // Assign the image texture in the Inspector
    public ParserModel.Root imageData; // Assign the JSON data in the Inspector

    private RawImage rawImage;

    // private void Start()
    // {
    //
    //     // Load JSON data and parse bounding box coordinates
    //     DrawBoundingBoxes();
    // }

    public void DrawBoundingBoxes()
    {
        rawImage = GetComponent<RawImage>();

        foreach (var detection in imageData.detection_list)
        {
            int tlx = detection.tlx;
            int tly = detection.tly;
            int brx = detection.brx;
            int bry = detection.bry;

            // Draw bounding box on the image texture
            for (int x = tlx; x <= brx; x++)
            {
                imageTexture.SetPixel(x, tly, Color.red);
                imageTexture.SetPixel(x, bry, Color.red);
            }

            for (int y = tly; y <= bry; y++)
            {
                imageTexture.SetPixel(tlx, y, Color.red);
                imageTexture.SetPixel(brx, y, Color.red);
            }
        }

        // Apply changes to the image texture
        imageTexture.Apply();

        // Update the UI Image with the modified texture
        rawImage.texture = imageTexture;
    }
}