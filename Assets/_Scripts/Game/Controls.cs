using System;
using UnityEngine;

namespace _Scripts.Game
{
    public class Controls : MonoBehaviour
    {
        public float jumpForce = 10f;
        private Rigidbody2D _rigidbody2D;
        private bool _isJumping;
        private Touch _touch;
        public GameObject frogTongue;
        private bool _isDragging;
        public float jumpHeight = 5f;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.freezeRotation = true; //adding this to prevent the frog's body from rotating while tongue is attached
            _isDragging = false;
        }

        // Update is called once per frame
        void Update()
        {
            _isJumping = _rigidbody2D.velocity.y != 0;
            
            if (Input.touchCount > 0)
            {
                _touch = Input.GetTouch(0);
                Debug.Log("Touch Phase: " + _touch.phase);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(_touch.position);
                touchPosition.z = 0;

                RaycastHit2D ray = GetTouchHit();
                SpringJoint2D tongue = frogTongue.GetComponent<SpringJoint2D>();
                bool isTargetFrogAndAttached = ray.collider && ray.collider.gameObject == gameObject && tongue.enabled;
                
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
                    tongue.enabled = false;
                }
            }
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
            transform.position = draggingPos;
        }
        
        private RaycastHit2D GetTouchHit()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended)
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
