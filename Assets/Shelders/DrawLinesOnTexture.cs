using UnityEngine;
using UnityEngine.UI;

namespace Shelders
{
    public class DrawLinesOnTexture : MonoBehaviour
    {
        public int textureWidth = 256;
        public int textureHeight = 256;
        public Color lineColor = Color.red;

        private Texture2D texture;

        void Start()
        {
            texture = new Texture2D(textureWidth, textureHeight);
            GetComponent<Image>().material.mainTexture = texture;

            DrawLine(textureWidth / 2, 0, textureWidth / 2, textureHeight, lineColor); // Draw a vertical line
            DrawLine(0, textureHeight / 2, textureWidth, textureHeight / 2, lineColor); // Draw a horizontal line

            texture.Apply();
        }

        void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            // Bresenham's line algorithm
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                texture.SetPixel(x0, y0, color);

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}