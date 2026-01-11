using System.Collections;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class PlatformBounce : MonoBehaviour
    {
        [Header("Bounce (physics)")]
        [SerializeField] private float bounceUpForce = 12f;
        [SerializeField] private float bounceSideForce = 0f; // optional, set to 0 for none

        [Header("Bounce (visual)")]
        public float squashAmount = 0.85f;
        public float stretchAmount = 1.05f;
        public float bounceHoldTime = 0.08f;

        [Header("Landing detection (Effector-safe)")]
        public float topPointTolerance = 0.05f;
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

            // ---- PHYSICS BOUNCE (calls your Controls script) ----
            var controls = playerRoot.GetComponent<_Scripts.Game.InputControl.Controls>();
            if (controls != null)
            {
                controls.PlatformBounce(
                    bounceUpForce,
                    bounceSideForce,
                    transform.position.x
                );
            }
            else if (playerRb != null)
            {
                // Fallback if Controls isn't found
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
                playerRb.AddForce(Vector2.up * bounceUpForce, ForceMode2D.Impulse);
            }

            // ---- VISUAL SQUISH ----
            if (!_isBouncing)
                StartCoroutine(BounceVisual());
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

            float cloudTopY = _cloudCollider.bounds.max.y;

            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D cp = collision.GetContact(i);
                if (cp.point.y >= cloudTopY - topPointTolerance)
                    return true;
            }

            return false;
        }

        private IEnumerator BounceVisual()
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
