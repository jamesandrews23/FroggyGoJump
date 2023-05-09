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
        public bool IsDragging => _isDragging;

        private float _deltaX, _deltaY;

        private bool _moveAllowed = false;

        private float _tongueLength = 0;

        private Vector2 _initialTouchPos;
        private Vector2 _currentTouchPos; 
        public float maxLaunchForce = 100f;
        public float defaultLaunchSpeed = 5f;

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
            // CheckFacingDirection();

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
            
            //Screen is touched
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;

                if(_tongueSpringJoint2D.enabled){
                    gameObject.layer = LayerMask.NameToLayer("WhileConnected");
                    _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("WhileConnected");   
                } else {
                    gameObject.layer = LayerMask.NameToLayer("Frog");
                    _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("Default");   
                }

                switch(touch.phase){
                    case TouchPhase.Began:
                        if(_tongueSpringJoint2D.enabled){
                            if(GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos)){
                                _initialTouchPos = touchPos;
                                Debug.Log("Initial Touch Pos: " + _initialTouchPos);
                                _deltaX = touchPos.x - transform.position.x;
                                _deltaY = touchPos.y - transform.position.y;

                                _moveAllowed = true;

                                // _rigidbody2D.freezeRotation = true;
                                _rigidbody2D.velocity = new Vector2(0, 0);
                                _rigidbody2D.gravityScale = 0;
                                _tongueSpringJoint2D.autoConfigureDistance = true;
                                DisableAllColliders();
                            }
                        } else {
                            if(!_isInAir)
                                Jump(touchPos);
                        }
                    break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if(GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos) && _moveAllowed){
                            _isDragging = true;
                            transform.position = touchPos;
                        }
                    break;
                    case TouchPhase.Ended:
                        _isDragging = false;
                        _moveAllowed = false;
                        _rigidbody2D.gravityScale = 2;
                        _tongueSpringJoint2D.autoConfigureDistance = false;
                        _tongueSpringJoint2D.distance = .5f;
                        if(_tongueSpringJoint2D.enabled && GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos)){
                            _currentTouchPos = touchPos;
                            _tongueLength = Vector2.Distance(gameObject.transform.position, _tongueSpringJoint2D.anchor);
                            LaunchFrog();
                        }
                    break;
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

        private void Jump(Vector3 touchPosition)
        {
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

        private void LaunchFrog()
        {
            _isDragging = false;
            Vector2 direction = (_initialTouchPos - _currentTouchPos).normalized;
            direction = new Vector2(direction.x, Mathf.Abs(direction.y));

            Debug.Log("Tongue Length: " + _tongueLength);
            float launchForce = Mathf.Clamp(_tongueLength * defaultLaunchSpeed, 0, maxLaunchForce);
            Debug.Log("Launch Force: " + launchForce);
            _rigidbody2D.AddForce(direction * launchForce, ForceMode2D.Impulse);

            _tongueSpringJoint2D.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Frog");
            _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("Default");   
            EnableAllColliders();
        }

        private void DisableAllColliders(){
            Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

            foreach (Collider2D collider in allColliders)
            {
                if(collider.gameObject != gameObject && (collider.gameObject.layer == LayerMask.NameToLayer("Hooks") || collider.gameObject.layer == LayerMask.NameToLayer("Platforms")))
                    collider.enabled = false;
            }
        }

        private void EnableAllColliders(){
            Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

            foreach (Collider2D collider in allColliders)
            {
                collider.enabled = true;
            }
        }
    }
}
