using System;
using UnityEngine;

namespace _Scripts.Game
{
    public class Controls : MonoBehaviour
    {
        public float jumpForce = 10f;
        private Rigidbody2D _rigidbody2D;
        private bool _isJumping;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.freezeRotation = true; //adding this to prevent the frog's body from rotating while tongue is attached
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Platform"))
            {
                _isJumping = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = 0;
                if (touch.phase == TouchPhase.Began && !_isJumping)
                {
                    if (touchPosition.x > transform.position.x)
                    {
                        _rigidbody2D.AddForce(new Vector2(jumpForce, jumpForce), ForceMode2D.Impulse);
                        _isJumping = true;
                    }
                    else if (touchPosition.x < transform.position.x)
                    {
                        _rigidbody2D.AddForce(new Vector2(-jumpForce, jumpForce), ForceMode2D.Impulse);
                        _isJumping = true;
                    }
                }
            }
        }
    }
}
