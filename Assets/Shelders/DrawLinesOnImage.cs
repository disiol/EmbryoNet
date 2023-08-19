using UnityEngine;
using UnityEngine.UI;

public class DrawLinesOnImage : MonoBehaviour
{
    public Image image; // Assign your UI Image in the Inspector

    void Start()
    {
        if (image == null)
        {
            Debug.LogError("Image component not assigned!");
            return;
        }

        // Get the center of the image in local coordinates
        Vector2 center = new Vector2(image.rectTransform.rect.width / 2, image.rectTransform.rect.height / 2);

        // Create a new texture to modify
        Texture2D newTexture = new Texture2D((int)image.rectTransform.rect.width, (int)image.rectTransform.rect.height);
        
        // Draw lines on the new texture
        Color lineColor = Color.red; // Change this to the desired line color
        int lineWidth = 2; // Adjust the line width as needed

        for (int x = 0; x < newTexture.width; x++)
        {
            newTexture.SetPixel(x, (int)center.y, lineColor);
        }

        for (int y = 0; y < newTexture.height; y++)
        {
            newTexture.SetPixel((int)center.x, y, lineColor);
        }

        // Apply the changes to the new texture
        newTexture.Apply();

        // Create a sprite from the new texture and assign it to the UI Image
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.one * 0.5f);
        image.sprite = newSprite;
    }
}