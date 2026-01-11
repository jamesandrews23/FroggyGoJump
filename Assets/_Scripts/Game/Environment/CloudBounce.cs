using System.Collections;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class CloudBounce : MonoBehaviour
    {
        [Header("Bounce (visual)")]
        public float squashAmount = 0.85f;
        public float stretchAmount = 1.05f;
        public float bounceHoldTime = 0.08f;

        [Header("Landing detection (Effector-safe)")]
        [Tooltip("How close the contact point must be to the top of the cloud to count as a landing.")]
        public float topPointTolerance = 0.05f;

        [Tooltip("Player must be falling or near-zero vertical speed to count as landing.")]
        public float maxUpwardVelocityToCount = 0.05f;

        private Vector3 _originalScale;
        private bool _isBouncing;

        private Transform _playerRootOnCloud;

        private Collider2D _cloudCollider;

        void Awake()
        {
            _cloudCollider = GetComponent<Collider2D>();
            _originalScale = transform.localScale;
        }

        void OnEnable()
        {
            // Reset state for pooling / re-enable scenarios
            _isBouncing = false;
            _playerRootOnCloud = null;

            transform.localScale = _originalScale;

            if (_cloudCollider != null)
                _cloudCollider.enabled = true;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Frog")) return;

            Transform playerRoot = collision.collider.transform.root;

            // Prevent double-counting if player has multiple colliders
            if (_playerRootOnCloud == playerRoot) return;

            Rigidbody2D playerRb = collision.collider.attachedRigidbody;
            if (!IsValidTopLanding(collision, playerRb)) return;

            _playerRootOnCloud = playerRoot;

            if (!_isBouncing)
                StartCoroutine(Bounce());
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Frog")) return;

            Transform playerRoot = collision.collider.transform.root;
            if (_playerRootOnCloud == playerRoot)
                _playerRootOnCloud = null;
        }

        private bool IsValidTopLanding(Collision2D collision, Rigidbody2D playerRb)
        {
            // Must be falling or basically not moving upward
            if (playerRb != null && playerRb.linearVelocity.y > maxUpwardVelocityToCount)
                return false;

            // Contact point vs top of cloud bounds (stable with EdgeCollider + PlatformEffector2D)
            float cloudTopY = _cloudCollider.bounds.max.y;

            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D cp = collision.GetContact(i);
                if (cp.point.y >= cloudTopY - topPointTolerance)
                    return true;
            }

            return false;
        }

        private IEnumerator Bounce()
        {
            _isBouncing = true;

            transform.localScale = new Vector3(
                _originalScale.x * stretchAmount,
                _originalScale.y * squashAmount,
                _originalScale.z
            );

            yield return new WaitForSeconds(bounceHoldTime);

            transform.localScale = _originalScale;
            _isBouncing = false;
        }
    }
}
