using UnityEngine;
using UnityEngine.UI;

namespace Tolls
{
    public class ArrowKeyScrolling : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public float scrollSpeed = 10f;

        private void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector2 scrollDelta = new Vector2(horizontalInput, verticalInput) * scrollSpeed * Time.deltaTime;

            scrollRect.normalizedPosition += scrollDelta;

            // Clamp the scroll position to be within [0, 1]
            scrollRect.normalizedPosition = new Vector2(
                Mathf.Clamp01(scrollRect.normalizedPosition.x),
                Mathf.Clamp01(scrollRect.normalizedPosition.y)
            );
        }
    }
}