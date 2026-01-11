using UnityEngine;

namespace _Scripts.Game.Player
{
    /// <summary>
    /// Add this to ANY 2D object with a Collider2D to make it tongue-attachable.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class TongueAttachable : MonoBehaviour, ITongueAttachable
    {
        public enum AttachPointMode
        {
            UseHitPoint,     // attach where you tapped
            UseTransform,    // attach to this object's transform.position (+ offset)
            UseCustomAnchor  // attach to a specific child/anchor transform (+ offset)
        }

        [Header("Attach Point")]
        [SerializeField] private AttachPointMode mode = AttachPointMode.UseHitPoint;

        [Tooltip("Optional: a child transform to use as the anchor when Mode = UseCustomAnchor")]
        [SerializeField] private Transform customAnchor;

        [Tooltip("World-space offset applied to the chosen attach point")]
        [SerializeField] private Vector2 worldOffset = Vector2.zero;

        public Vector2 GetAttachPoint(Vector2 hitPoint)
        {
            Vector2 basePoint = mode switch
            {
                AttachPointMode.UseTransform => (Vector2)transform.position,
                AttachPointMode.UseCustomAnchor => customAnchor != null ? (Vector2)customAnchor.position : (Vector2)transform.position,
                _ => hitPoint
            };

            return basePoint + worldOffset;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Show where the attach point will be (approx; hit point varies in UseHitPoint mode)
            Gizmos.color = Color.green;

            Vector2 p = mode switch
            {
                AttachPointMode.UseTransform => (Vector2)transform.position + worldOffset,
                AttachPointMode.UseCustomAnchor => (customAnchor != null ? (Vector2)customAnchor.position : (Vector2)transform.position) + worldOffset,
                _ => (Vector2)transform.position + worldOffset
            };

            Gizmos.DrawWireSphere(p, 0.15f);
        }
#endif
    }
}
