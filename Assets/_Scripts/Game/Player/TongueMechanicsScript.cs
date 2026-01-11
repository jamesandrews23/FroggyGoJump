using _Scripts.Game.Environment;
using UnityEngine;

namespace _Scripts.Game.Player
{
    public class TongueMechanicsScript : MonoBehaviour
    {
        public GameObject tongue;

        private SpringJoint2D _tongueJoint;
        private Vector2 _connectedTonguePoint;

        public float tongueAnchorPointOffset = 1;
        public float bottomOfScreenThreshold = 2;
        public float distanceToHookThreshold = 5;

        public Camera mainCamera;

        // Replaces: private Hook connectedHook;
        private ITongueAttachable connectedAttachable;

        private Consumable food;
        private LineRenderer line;

        private bool touchedConsumable = false;
        private Collider2D tongueCollision;

        public float tongueSpeed = 5f;
        private Rigidbody2D playerRigidbody;

        void Start()
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            _tongueJoint = tongue.GetComponent<SpringJoint2D>();
            line = tongue.GetComponent<LineRenderer>();
            mainCamera = Camera.main;

            line.startWidth = 0.2f;
            line.endWidth = 0.1f;
        }

        void Update()
        {
            var rayCast = GetTouchHit();
            if (rayCast.collider == null) return;

            // NEW: anything with ITongueAttachable can be attached to
            var hitAttachable = rayCast.collider.GetComponent<TongueAttachable>();
            if (hitAttachable != null)
            {
                // If touched same object you're already attached to -> detach
                if (connectedAttachable != null && ReferenceEquals(connectedAttachable, hitAttachable))
                {
                    _tongueJoint.enabled = false;
                    connectedAttachable = null;
                }
                else if (IsAttachableInRange(rayCast)) // reuse your existing range logic
                {
                    connectedAttachable = hitAttachable;

                    // Attach point can be hit point, transform, or a custom anchor (via TongueAttachable component)
                    _connectedTonguePoint = hitAttachable.GetAttachPoint(rayCast.point);

                    _tongueJoint.enabled = true;

                    // Your existing vertical offset stays (you can move this into TongueAttachable if you prefer)
                    _tongueJoint.anchor = _connectedTonguePoint + new Vector2(0, tongueAnchorPointOffset);
                }

                return; // don't fall through to consumable handling on the same tap
            }

            // Your existing consumable logic stays the same (LateUpdate handles drawing/pulling)
        }

        void LateUpdate()
        {
            var rayCast = GetTouchHit();

            if (_tongueJoint.enabled)
            {
                Vector3 pos1 = _tongueJoint.connectedBody.transform.position;
                Vector3 pos2 = _tongueJoint.anchor;
                line.enabled = true;
                line.SetPosition(0, pos1);
                line.SetPosition(1, pos2 + new Vector3(0, -1, 0));
            }
            else if (rayCast.collider != null && rayCast.collider.gameObject.CompareTag("Consumable"))
            {
                float distance = Vector2.Distance(transform.position, rayCast.collider.transform.position);
                if (distance <= distanceToHookThreshold)
                {
                    touchedConsumable = true;
                    tongueCollision = rayCast.collider;
                    line.enabled = true;
                    DrawTongueValues(transform.position, tongueCollision.transform.position);
                }
            }
            else
            {
                line.enabled = false;
            }

            if (touchedConsumable && tongueCollision != null)
            {
                tongueCollision.transform.position = Vector2.MoveTowards(
                    tongueCollision.transform.position,
                    transform.position,
                    tongueSpeed * Time.deltaTime
                );

                line.enabled = true;
                DrawTongueValues(transform.position, tongueCollision.transform.position);
            }
        }

        public Vector3 GetAttachedTonguePoint()
        {
            return _connectedTonguePoint;
        }

        public bool AttachedToHook()
        {
            // Now means "attached to something"
            return connectedAttachable != null && _tongueJoint.enabled;
        }

        private bool IsAttachableInRange(RaycastHit2D hit)
        {
            float cameraBottomY = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).y;
            var targetPosY = hit.collider.transform.position.y;

            Vector2 posA = new Vector2(0, cameraBottomY);
            Vector2 posB = new Vector2(0, targetPosY);

            float distanceBetweenTargetAndScreen = Vector2.Distance(posA, posB);

            Vector3 frogPos = playerRigidbody.transform.position;

            float distanceBetweenTargetAndFrog = Vector2.Distance(frogPos, posB);

            return distanceBetweenTargetAndScreen >= bottomOfScreenThreshold &&
                   distanceBetweenTargetAndFrog <= distanceToHookThreshold;
        }

        [SerializeField] private LayerMask tongueAttachMask = ~0; // set in Inspector if you want

        private RaycastHit2D GetTouchHit()
        {
            // Touch on device
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Began) return new RaycastHit2D();

                Ray ray = mainCamera.ScreenPointToRay(touch.position);

                // IMPORTANT: Use GetRayIntersection for 2D colliders (EdgeCollider works!)
                return Physics2D.GetRayIntersection(ray, Mathf.Infinity, tongueAttachMask);
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            // Mouse in editor (so you can test without building)
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                return Physics2D.GetRayIntersection(ray, Mathf.Infinity, tongueAttachMask);
            }
#endif

            return new RaycastHit2D();
        }


        private void DrawTongueValues(Vector3 pos1, Vector3 pos2)
        {
            line.SetPosition(0, pos1);
            line.SetPosition(1, pos2);
        }
    }
}
