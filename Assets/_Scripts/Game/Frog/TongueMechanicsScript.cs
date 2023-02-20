using UnityEngine;

namespace _Scripts.Game
{
    public class TongueMechanicsScript : MonoBehaviour
    {
        public GameObject tongue;
        // Start is called before the first frame update
        private SpringJoint2D _distanceJoint2D;

        private Vector2 _connectedTonguePoint;

        public float tongueAnchorPointOffset = 1;

        public float bottomOfScreenThreshold = 2;

        public float distanceToHookThreshold = 5;

        void Start()
        {
            _distanceJoint2D = tongue.GetComponent<SpringJoint2D>();
        }

        // Update is called once per frame
        void Update()
        {
            var rayCast = GetTouchHit();
            if (rayCast.collider != null && rayCast.collider.gameObject.CompareTag("Hook"))
            {
                if (IsHookInRange(rayCast))
                {
                    _connectedTonguePoint = rayCast.point;
                    _distanceJoint2D.enabled = true;
                    _distanceJoint2D.anchor = _connectedTonguePoint + new Vector2(0, tongueAnchorPointOffset);
                }
            }
        }

        private bool IsHookInRange(RaycastHit2D hit)
        {
            
            Camera mainCamera = Camera.main;
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
                    Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                    return hit;
                }
            }

            return new RaycastHit2D();
        }
    }
}
