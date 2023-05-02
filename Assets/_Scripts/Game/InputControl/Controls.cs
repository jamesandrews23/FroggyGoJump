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

        public float launchForce = 10f;

        public bool IsDragging => _isDragging;

        private float _deltaX, _deltaY;

        private bool _moveAllowed = false;

        private float _tongueLength = 0;

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

                switch(touch.phase){
                    case TouchPhase.Began:
                        if(_tongueSpringJoint2D.enabled){
                            if(GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos)){
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
                            Debug.Log("Launch Frog");
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
            EnableAllColliders();

            Vector2 direction = (_tongueSpringJoint2D.transform.position - transform.position).normalized;
            direction = new Vector2(direction.x, Mathf.Abs(direction.y));
            float launchPower = launchForce + _tongueLength;
            _rigidbody2D.AddForce(direction * launchPower, ForceMode2D.Impulse);
            Debug.Log("Direction: " + direction * launchPower);
            
            // Add an additional upward force to the frog
            // Vector2 upwardForce = Vector2.up * launchForce;
            // _rigidbody2D.AddForce(upwardForce, ForceMode2D.Impulse);
            // Debug.Log("Upward Force: " + upwardForce);

            _tongueSpringJoint2D.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Frog");
            _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("Default");   
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
