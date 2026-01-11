using UnityEngine;

namespace _Scripts.Game.Player
{
    /// <summary>
    /// Screen wrap for 2D games (jumper style).
    /// - Player can be visible on both sides while crossing.
    /// - Teleports only after the entire sprite leaves the screen.
    /// </summary>
    [DisallowMultipleComponent]
    public class ScreenWrap2D : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Leave empty to auto-find Camera.main")]
        public Camera targetCamera;

        [Tooltip("If the object has multiple renderers, assign the one that represents its visual bounds.")]
        public SpriteRenderer spriteRenderer;

        [Header("Wrap Axes")]
        public bool wrapHorizontal = true;
        public bool wrapVertical = false;

        [Header("Padding")]
        [Tooltip("Extra space beyond the screen edge before wrapping. Helps avoid jitter.")]
        public float padding = 0.02f;

        // Cached half sizes of the sprite in world units
        private float halfWidth;
        private float halfHeight;

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            RecalculateSpriteExtents();
        }

        private void LateUpdate()
        {
            if (targetCamera == null || spriteRenderer == null)
                return;

            // If scale changes at runtime, this keeps it accurate (cheap enough for one player)
            RecalculateSpriteExtents();

            Vector3 pos = transform.position;

            // Camera edges in world space
            float camZ = targetCamera.transform.position.z;
            float objZ = pos.z;

            // Get world-space corners at the player's Z plane
            Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0f, 0f, Mathf.Abs(objZ - camZ)));
            Vector3 topRight   = targetCamera.ViewportToWorldPoint(new Vector3(1f, 1f, Mathf.Abs(objZ - camZ)));

            float leftEdge   = bottomLeft.x;
            float rightEdge  = topRight.x;
            float bottomEdge = bottomLeft.y;
            float topEdge    = topRight.y;

            if (wrapHorizontal)
            {
                // If the entire sprite is past the right edge, move it to the left side
                if (pos.x - halfWidth > rightEdge + padding)
                    pos.x = leftEdge - halfWidth;

                // If the entire sprite is past the left edge, move it to the right side
                else if (pos.x + halfWidth < leftEdge - padding)
                    pos.x = rightEdge + halfWidth;
            }

            if (wrapVertical)
            {
                if (pos.y - halfHeight > topEdge + padding)
                    pos.y = bottomEdge - halfHeight;
                else if (pos.y + halfHeight < bottomEdge - padding)
                    pos.y = topEdge + halfHeight;
            }

            transform.position = pos;
        }

        private void RecalculateSpriteExtents()
        {
            // bounds are world-space and include lossyScale
            Bounds b = spriteRenderer.bounds;
            halfWidth = b.extents.x;
            halfHeight = b.extents.y;
        }
    }
}

