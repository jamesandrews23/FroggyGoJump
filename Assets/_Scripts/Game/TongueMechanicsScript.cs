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
                Camera mainCamera = Camera.main;
                float cameraBottomY = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).y;
                var hookPos = rayCast.collider.gameObject.transform.position.y;
                Vector2 posA = new Vector2(0, cameraBottomY);
                Vector2 posB = new Vector2(0, hookPos);
                float distance = Vector2.Distance(posA, posB);

                if (distance >= bottomOfScreenThreshold)
                {
                    _connectedTonguePoint = rayCast.point;
                    _distanceJoint2D.enabled = true;
                    _distanceJoint2D.anchor = _connectedTonguePoint + new Vector2(0, tongueAnchorPointOffset);
                }
            }
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
