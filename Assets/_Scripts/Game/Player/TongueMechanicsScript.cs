using _Scripts.Game.Environment;
using UnityEngine;

namespace _Scripts.Game.Player
{
    public class TongueMechanicsScript : MonoBehaviour
    {
        public GameObject tongue;
        // Start is called before the first frame update
        private SpringJoint2D _tongueJoint;

        private Vector2 _connectedTonguePoint;

        public float tongueAnchorPointOffset = 1;

        public float bottomOfScreenThreshold = 2;

        public float distanceToHookThreshold = 5;

        public Camera mainCamera;
        
        private Hook connectedHook;
        private Consumable food;
        private LineRenderer line;

        private bool touchedConsumable = false;

        private Collider2D tongueCollision;

        public float tongueSpeed = 5f;

        void Start()
        {
            _tongueJoint = tongue.GetComponent<SpringJoint2D>();
            line = tongue.GetComponent<LineRenderer>();
            mainCamera = Camera.main;

            line.startWidth = 0.2f;
            line.endWidth = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            var rayCast = GetTouchHit();
            if (rayCast.collider != null && rayCast.collider.gameObject.CompareTag("Hook"))
            {
                var hitHook = rayCast.collider.GetComponent<Hook>();
                if (connectedHook != null && connectedHook == hitHook) //if touched hook is connected to tongue already, detach tongue from touched hook
                {
                    _tongueJoint.enabled = false;
                    connectedHook = null;
                }
                else if (IsHookInRange(rayCast)) //otherwise, check if touched hook is in range. If so, attach tongue to hook
                {
                    connectedHook = hitHook;
                    _connectedTonguePoint = rayCast.point;
                    _tongueJoint.enabled = true;
                    _tongueJoint.anchor = _connectedTonguePoint + new Vector2(0, tongueAnchorPointOffset);
                }
            } 
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
            } else if (rayCast.collider != null && rayCast.collider.gameObject.CompareTag("Consumable")) {
                float distance = Vector2.Distance(transform.position, rayCast.collider.transform.position);
                if(distance <= distanceToHookThreshold){
                    touchedConsumable = true;
                    tongueCollision = rayCast.collider;
                    line.enabled = true;
                    DrawTongueValues(transform.position, tongueCollision.transform.position);
                }
            } else {
                line.enabled = false;
            }

            if(touchedConsumable && tongueCollision != null){
                tongueCollision.transform.position = Vector2.MoveTowards(tongueCollision.transform.position, transform.position, tongueSpeed * Time.deltaTime);
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
            return connectedHook != null && _tongueJoint.enabled;
        }

        public bool CompareConnectedHook(Hook toCompare)
        {
            return toCompare == connectedHook;
        }

        private bool IsHookInRange(RaycastHit2D hit)
        {
            float cameraBottomY = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).y;
            var hookPos = hit.collider.gameObject.transform.position.y;
            Vector2 posA = new Vector2(0, cameraBottomY);
            Vector2 posB = new Vector2(0, hookPos);
            float distanceBetweenHookAndScreen = Vector2.Distance(posA, posB);

            Rigidbody2D frog = tongue.GetComponent<SpringJoint2D>().connectedBody;
            Vector3 frogPos = frog.transform.position;

            float distanceBetweenHookAndFrog = Vector2.Distance(frogPos, posB);
            
            return distanceBetweenHookAndScreen >= bottomOfScreenThreshold && distanceBetweenHookAndFrog <= distanceToHookThreshold;
        }

        private RaycastHit2D GetTouchHit()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 touchPosition = touch.position;
                    Ray ray = mainCamera.ScreenPointToRay(touchPosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                    return hit;
                }
            }

            return new RaycastHit2D();
        }

        private void DrawTongueValues(Vector3 pos1, Vector3 pos2){
            line.SetPosition(0, pos1);
            line.SetPosition(1, pos2);    
        }
    }
}
