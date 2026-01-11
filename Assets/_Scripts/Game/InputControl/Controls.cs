using _Scripts.Game.Environment;
using _Scripts.Game.Player;
using UnityEngine;

namespace _Scripts.Game.InputControl
{
    public class Controls : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private bool _isInAir;
        private Touch _touch;

        public GameObject frogTongue;
        private SpringJoint2D _tongueSpringJoint2D;

        private bool _isDragging;
        public bool IsDragging => _isDragging;

        // Recommended: flip sprite instead of rotating whole object (safer for 2D + colliders)
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator frogAnimator; // <-- your frog idle/jump animator
        public Animator legendaryAnim;                  // <-- your existing legendary trigger animator (keep)

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.12f;
        [SerializeField] private LayerMask groundLayer;

        public Vector3 facingRightRotation = new Vector3(0, 0, 0);
        public Vector3 facingLeftRotation = new Vector3(0, 180, 0);

        private TongueMechanicsScript _tongueMechanicsScript;

        public float maxHeightReached = 0f;
        public float jumpUpForce = 10f;
        public float jumpForce = 1f;

        private float _deltaX, _deltaY;
        private bool _moveAllowed = false;
        private float _tongueLength = 0;
        private Vector2 _initialTouchPos;
        private Vector2 _currentTouchPos;

        public float maxLaunchForce = 100f;
        public float defaultLaunchSpeed = 5f;

        private bool _isPlayerTouched = false;
        private bool consuming = false;

        private AudioManager _audioManager;

        // Animator parameter hashes (faster + typo-proof)
        private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
        private static readonly int YVelocityHash   = Animator.StringToHash("yVelocity");
        private static readonly int IsDraggingHash  = Animator.StringToHash("isDragging");
        private static readonly int LegendaryStartHash = Animator.StringToHash("Start");
        
        [Header("Bounce Air Control (only after platform bounce)")]
        [SerializeField] private float bounceAirControlDuration = 0.35f;   // how long steering is allowed after a bounce
        [SerializeField] private float bounceAirControlAccel = 40f;        // how fast x velocity changes toward target
        [SerializeField] private float bounceAirControlMaxSpeed = 6f;      // cap horizontal speed during bounce control

        private float _bounceAirControlTimer = 0f;


        void Reset()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _tongueMechanicsScript = GetComponent<TongueMechanicsScript>();

            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (frogAnimator == null) frogAnimator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            _tongueMechanicsScript = GetComponent<TongueMechanicsScript>();
            _tongueSpringJoint2D = frogTongue.GetComponent<SpringJoint2D>();
            _tongueSpringJoint2D.enabled = false;

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.freezeRotation = true;

            _isDragging = false;

            _audioManager = GameObject.Find("AudioManagerObject").GetComponent<AudioManager>();

            // Fallbacks if not assigned in Inspector
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (frogAnimator == null) frogAnimator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            // ---- LEGENDARY ANIM ----
            if (_rigidbody2D.linearVelocity.y >= 20f && legendaryAnim != null)
            {
                legendaryAnim.SetTrigger(LegendaryStartHash);
            }

            // ---- GROUND / AIR STATE ----
            bool isGrounded = IsGrounded();
            _isInAir = !isGrounded;
            
            if (_bounceAirControlTimer > 0f)
                _bounceAirControlTimer -= Time.deltaTime;


            // ---- SCORE HEIGHT TRACKING ----
            float playerPosY = _rigidbody2D.gameObject.transform.position.y;
            if (playerPosY > 0 && playerPosY > maxHeightReached)
                maxHeightReached = playerPosY;

            bool isPlayerFalling = _rigidbody2D.linearVelocity.y < 0;
            if (isPlayerFalling && !_isPlayerTouched)
                EnableAllColliders();

            // ---- UPDATE FROG ANIMATOR ----
            UpdateFrogAnimator(isGrounded);

            // ---- OPTIONAL: FACE DIRECTION (SPRITE FLIP) ----
            UpdateFacingDirection();

            // ---- TOUCH INPUT ----
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;

                if (_tongueSpringJoint2D.enabled)
                {
                    gameObject.layer = LayerMask.NameToLayer("WhileConnected");
                    _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("WhileConnected");
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Frog");
                    _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("Default");
                }

                _isPlayerTouched = GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (_tongueSpringJoint2D.enabled)
                        {
                            if (_isPlayerTouched)
                            {
                                _audioManager.PlaySource(4);
                                _initialTouchPos = touchPos;
                                _deltaX = touchPos.x - transform.position.x;
                                _deltaY = touchPos.y - transform.position.y;

                                _moveAllowed = true;

                                _rigidbody2D.linearVelocity = new Vector2(0, 0);
                                _rigidbody2D.gravityScale = 0;
                                _tongueSpringJoint2D.autoConfigureDistance = true;

                                DisableAllColliders();
                            }
                        }
                        else
                        {
                            // Only jump if grounded (prevents double-jump from small velocity jitter)
                            if (isGrounded)
                            {
                                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                                if (hit.collider != null && hit.collider.CompareTag("Consumable"))
                                    consuming = true;
                                else
                                    consuming = false;

                                if (!consuming)
                                    Jump(touchPos);
                            }
                        }
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        // If you're not in tongue-drag mode, allow bounce-only air steering
                        if (!_moveAllowed) 
                            ApplyBounceAirControl(touchPos);
                        
                        if (_isPlayerTouched && _moveAllowed)
                        {
                            _isDragging = true;
                            transform.position = touchPos;
                        }
                        break;
                    case TouchPhase.Ended:
                        _audioManager.StopSource(4);

                        _isDragging = false;
                        _moveAllowed = false;

                        _rigidbody2D.gravityScale = 2;
                        _tongueSpringJoint2D.autoConfigureDistance = false;
                        _tongueSpringJoint2D.distance = .5f;

                        if (_tongueSpringJoint2D.enabled && _isPlayerTouched)
                        {
                            _currentTouchPos = touchPos;
                            _tongueLength = Vector2.Distance(gameObject.transform.position, _tongueSpringJoint2D.anchor);
                            LaunchFrog();
                            _isPlayerTouched = false;
                        }
                        break;
                }
            }
        }

        private bool IsGrounded()
        {
            if (groundCheck == null)
                return Mathf.Abs(_rigidbody2D.linearVelocity.y) < 0.01f; // fallback if you forgot to assign

            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        private void UpdateFrogAnimator(bool isGrounded)
        {
            if (frogAnimator == null) return;

            frogAnimator.SetBool(IsGroundedHash, isGrounded);
            frogAnimator.SetFloat(YVelocityHash, _rigidbody2D.linearVelocity.y);
            frogAnimator.SetBool(IsDraggingHash, _isDragging);
        }

        private void UpdateFacingDirection()
        {
            if (spriteRenderer == null) return;

            // If attached to hook, face toward the hook point; otherwise face direction of motion
            if (_tongueMechanicsScript != null && _tongueMechanicsScript.AttachedToHook())
            {
                var attachedTonguePoint = _tongueMechanicsScript.GetAttachedTonguePoint();
                spriteRenderer.flipX = transform.position.x > attachedTonguePoint.x;
                return;
            }

            var xVel = _rigidbody2D.linearVelocity.x;
            if (xVel > 0.05f) spriteRenderer.flipX = false;
            else if (xVel < -0.05f) spriteRenderer.flipX = true;
        }

        private void Jump(Vector3 touchPosition)
        {
            touchPosition.z = 0;
            _audioManager.PlaySource(2);

            float angle = Vector3.Angle(touchPosition - gameObject.transform.position, Vector3.up);

            if (angle < 25)
            {
                _rigidbody2D.AddForce(Vector2.up * jumpUpForce, ForceMode2D.Impulse);
            }
            else
            {
                if (touchPosition.x > transform.position.x)
                    _rigidbody2D.AddForce(new Vector2(jumpForce, jumpUpForce), ForceMode2D.Impulse);
                else if (touchPosition.x < transform.position.x)
                    _rigidbody2D.AddForce(new Vector2(-jumpForce, jumpUpForce), ForceMode2D.Impulse);
            }
        }
        
        public void PlatformBounce(float bounceUpForce, float bounceSideForce = 0f, float fromX = 0f)
        {
            // Arm bounce-only air control window
            _bounceAirControlTimer = bounceAirControlDuration;

            // Consistent bounce: clear vertical speed before applying bounce
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, 0f);

            Vector2 force = Vector2.up * bounceUpForce;

            // Optional sideways push away from platform center
            if (Mathf.Abs(bounceSideForce) > 0.001f)
            {
                float dir = Mathf.Sign(transform.position.x - fromX);
                if (dir == 0f) dir = 1f;
                force += Vector2.right * (dir * bounceSideForce);
            }

            _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }


        private void LaunchFrog()
        {
            _isDragging = false;

            Vector2 direction = (_initialTouchPos - _currentTouchPos).normalized;
            direction = new Vector2(direction.x, Mathf.Abs(direction.y));

            float launchForce = Mathf.Clamp(_tongueLength * defaultLaunchSpeed, 0, maxLaunchForce);
            _rigidbody2D.AddForce(direction * launchForce, ForceMode2D.Impulse);

            _tongueSpringJoint2D.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Frog");
            _tongueSpringJoint2D.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        private void DisableAllColliders()
        {
            Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

            foreach (Collider2D collider in allColliders)
            {
                if (collider.gameObject != gameObject &&
                    (collider.gameObject.layer == LayerMask.NameToLayer("Hooks") ||
                     collider.gameObject.layer == LayerMask.NameToLayer("Platforms")))
                {
                    collider.enabled = false;
                }
            }
        }

        private void EnableAllColliders()
        {
            Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
            foreach (Collider2D collider in allColliders)
                collider.enabled = true;
        }

        // Optional: visualize ground check in Scene view
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        private void ApplyBounceAirControl(Vector3 touchWorldPos)
        {
            if (_bounceAirControlTimer <= 0f) return;
            if (_tongueSpringJoint2D != null && _tongueSpringJoint2D.enabled) return; // don't fight tongue system

            // Only left/right steering (no vertical changes)
            float dir = 0f;
            float dx = touchWorldPos.x - transform.position.x;

            if (dx > 0.15f) dir = 1f;
            else if (dx < -0.15f) dir = -1f;

            if (dir == 0f) return;

            float targetX = dir * bounceAirControlMaxSpeed;

            _rigidbody2D.linearVelocity = new Vector2(
                Mathf.MoveTowards(_rigidbody2D.linearVelocity.x, targetX, bounceAirControlAccel * Time.deltaTime),
                _rigidbody2D.linearVelocity.y
            );
        }

    }
}
