using UnityEngine;

namespace _Scripts.Game
{
    public class Controls : MonoBehaviour
    {
        public float jumpForce = 10f;
        private Rigidbody2D _rigidbody2D;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.freezeRotation = true; //adding this to prevent the frog's body from rotating while tongue is attached
        }

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = 0;
                Vector2 direction = (Vector2)((touchPosition - transform.position));
                if (touch.phase == TouchPhase.Began)
                {
                    if (touchPosition.x > transform.position.x)
                    {
                        _rigidbody2D.AddForce(new Vector2(jumpForce, jumpForce), ForceMode2D.Impulse);
                    }
                    else if (touchPosition.x < transform.position.x)
                    {
                        _rigidbody2D.AddForce(new Vector2(-jumpForce, jumpForce), ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
}
