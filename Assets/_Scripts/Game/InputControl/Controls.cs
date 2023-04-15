using _Scripts.Game.Environment;
using _Scripts.Game.Frog;
using UnityEngine;

namespace _Scripts.Game.InputControl
{
    public class Controls : MonoBehaviour
    {
        public float jumpForce = 10f;
        private Rigidbody2D _rigidbody2D;
        private bool _isInAir;
        private Touch _touch;
        public GameObject frogTongue;
        private SpringJoint2D _tongueSpringJoint2D;
        private bool _isDragging;
        public float jumpHeight = 5f;
        public Vector3 facingRightRotation = new Vector3(0,0,0);
        public Vector3 facingLeftRotation = new Vector3(0,180,0);
        private TongueMechanicsScript _tongueMechanicsScript;
        public float maxHeightReached = 0f;
        public float jumpUpForce = 10f;
        public float jumpForce45 = 1f;
        public float jumpHeight45 = 7f;

        public float launchForce = 1f;

        public bool IsDragging => _isDragging;

        public float tongueLaunchForce = 10f;

        // Start is called before the first frame update
        void Start()
        {
            _tongueMechanicsScript = GetComponent<TongueMechanicsScript>();
            _tongueSpringJoint2D = frogTongue.GetComponent<SpringJoint2D>();
            _tongueSpringJoint2D.enabled = false;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.freezeRotation = true; //adding this to prevent the frog's body from rotating while tongue is attached
            _isDragging = false;
        }

        // Update is called once per frame
        void Update()
        {
            CheckFacingDirection();

            //determines if player is in the air i.e. not on a platform, falling or flying
            _isInAir = _rigidbody2D.velocity.y != 0;

            //Stores maximum height reached for score
            float playerPosY = _rigidbody2D.gameObject.transform.position.y;
            if (playerPosY > 0)
            {
                if (playerPosY > maxHeightReached)
                {
                    maxHeightReached = playerPosY;
                }
            }
            
            //handles touch event
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);
                HandleTouchEvent();
            }
        }

        private void HandleTouchEvent()
        {
            Ray ray = Camera.main.ScreenPointToRay(_touch.position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            bool isPlayerTouched = hit.collider && hit.collider.gameObject == gameObject;
            bool isPlayerConnected = _tongueSpringJoint2D.enabled;
            
            Debug.Log("Player Touched: " + isPlayerTouched);
            Debug.Log("Player Connected: " + isPlayerConnected);

            if(isPlayerConnected)
            {
                gameObject.layer = LayerMask.NameToLayer("WhileConnected");
                _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("WhileConnected");   
            } else {
                gameObject.layer = LayerMask.NameToLayer("Frog");
                _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("Default");   
            }
            
            if (_touch.phase == TouchPhase.Began && !_isInAir)
            {
                Jump();
            }

            if ((_touch.phase == TouchPhase.Moved || _touch.phase == TouchPhase.Stationary) && isPlayerTouched && isPlayerConnected)
            {
                Dragging();
            }

            if (_touch.phase == TouchPhase.Ended && isPlayerConnected && isPlayerTouched)
            {
                DragRelease();
            }
        }


        private void CheckFacingDirection()
        {
            if (!_tongueMechanicsScript.AttachedToHook())
            {
                var xVel = _rigidbody2D.velocity.x;
                if (xVel > 0)
                {
                    transform.rotation = Quaternion.Euler(facingRightRotation);
                }
                else if (xVel < 0)
                {
                    transform.rotation = Quaternion.Euler(facingLeftRotation);
                }
            }
            else
            {
                var attachedTonguePoint = _tongueMechanicsScript.GetAttachedTonguePoint();
                if (transform.position.x > attachedTonguePoint.x)
                {
                    transform.rotation = Quaternion.Euler(facingLeftRotation);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(facingRightRotation);
                }
            }
        }

        private void Jump()
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(_touch.position);
            touchPosition.z = 0;
            
            float angle = Vector3.Angle(touchPosition - gameObject.transform.position, Vector3.up);

            if (angle < 25)
            {
                _rigidbody2D.AddForce(Vector2.up * jumpUpForce, ForceMode2D.Impulse);
            } else if (angle < 45)
            {
                if (touchPosition.x > transform.position.x)
                {
                    _rigidbody2D.AddForce(new Vector2(jumpForce45, jumpHeight45), ForceMode2D.Impulse);
                }
                else if (touchPosition.x < transform.position.x)
                {
                    _rigidbody2D.AddForce(new Vector2(-jumpForce45, jumpHeight45), ForceMode2D.Impulse);
                }
            }
            else
            {
                if (touchPosition.x > transform.position.x)
                {
                    _rigidbody2D.AddForce(new Vector2(jumpForce, jumpHeight), ForceMode2D.Impulse);
                }
                else if (touchPosition.x < transform.position.x)
                {
                    _rigidbody2D.AddForce(new Vector2(-jumpForce, jumpHeight), ForceMode2D.Impulse);
                }
            }
        }

        private void Dragging()
        {
            Debug.Log("***DRAGGING***");
            // Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            // rigidbody2D.isKinematic = true;
            _isDragging = true;
            Vector3 draggingPos = Camera.main.ScreenToWorldPoint(_touch.position);
            draggingPos.z = 0f;
            transform.position = draggingPos;
        }

        private void DragRelease()
        {
            Debug.Log("***DRAG END***");
            // rigidbody2D.isKinematic = false;
            _isDragging = false;
            LaunchFrog();
        }

        private void LaunchFrog()
        {
            Debug.Log("Luanch Frog");
            Vector2 direction = (_tongueSpringJoint2D.transform.position - transform.position).normalized;
            _rigidbody2D.AddForce(direction * tongueLaunchForce, ForceMode2D.Impulse);
            _tongueSpringJoint2D.enabled = false;
        }
    }
}
