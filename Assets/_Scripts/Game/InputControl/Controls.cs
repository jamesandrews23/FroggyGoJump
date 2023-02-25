using _Scripts.Game.Environment;
using _Scripts.Game.Frog;
using UnityEngine;

namespace _Scripts.Game.InputControl
{
    public class Controls : MonoBehaviour
    {
        public float jumpForce = 10f;
        private Rigidbody2D _rigidbody2D;
        private bool _isJumping;
        private Touch _touch;
        public GameObject frogTongue;
        private SpringJoint2D _tongueSpringJoint2D;
        private bool _isDragging;
        public float jumpHeight = 5f;
        public InputController inputController;
        private bool isFlying;
        public Vector3 facingRightRotation = new Vector3(0,0,0);
        public Vector3 facingLeftRotation = new Vector3(0,180,0);
        private TongueMechanicsScript _tongueMechanicsScript;
        public float maxHeightReached = 0f;

        public bool IsDragging => _isDragging;

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

            float playerVelocityY = _rigidbody2D.velocity.y;
            
            _isJumping = !isFlying && playerVelocityY != 0;

            if (isFlying)
            {
                if (playerVelocityY <= 0)
                {
                    EndFly();
                }
            }

            float playerPosY = _rigidbody2D.gameObject.transform.position.y;
            if (playerPosY > 0)
            {
                if (playerPosY > maxHeightReached)
                {
                    maxHeightReached = playerPosY;
                    Debug.Log("Height Reached: " + maxHeightReached);
                }
            }
            
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);
                Debug.Log("Touch Phase: " + _touch.phase);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(_touch.position);
                touchPosition.z = 0;

                RaycastHit2D ray = inputController.GetTouchHit();
                bool isTargetFrogAndAttached = ray.collider && ray.collider.gameObject == gameObject && _tongueSpringJoint2D.enabled;
                
                Debug.Log("Dragging: " + _isDragging);
                Debug.Log("Frog Target: " + isTargetFrogAndAttached);

                if (_touch.phase == TouchPhase.Began && !_isJumping)
                {
                    Jump(touchPosition);
                }
                
                if (_touch.phase == TouchPhase.Began && isTargetFrogAndAttached)
                {
                    _isDragging = true;
                }

                if (_isDragging)
                {
                    Dragging();
                }

                if (_touch.phase == TouchPhase.Ended && isTargetFrogAndAttached)
                {
                    _isDragging = false;
                    _tongueSpringJoint2D.enabled = false;
                    BeginFly();
                }
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

        private void BeginFly()
        {
            isFlying = true;
            gameObject.layer = 9; //set to FrogInFlight layer
        }

        private void EndFly()
        {
            isFlying = false;
            gameObject.layer = 7; //set to regular Frog layer
        }

        private void Jump(Vector3 touchPosition)
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

        private void Dragging()
        {
            Vector3 draggingPos = Camera.main.ScreenToWorldPoint(_touch.position);
            draggingPos.z = 0f;
            float hookPosY = _tongueSpringJoint2D.anchor.y - _tongueMechanicsScript.tongueAnchorPointOffset;
            if (draggingPos.y <= hookPosY)
            {
                transform.position = draggingPos;
            }
        }
    }
}
