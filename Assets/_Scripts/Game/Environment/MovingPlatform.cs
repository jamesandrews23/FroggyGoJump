using UnityEngine;

namespace _Scripts.Game.Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingPlatform2D : MonoBehaviour
    {
        [SerializeField] private float amplitude = 1.5f;
        [SerializeField] private float speed = 1.2f;

        private Rigidbody2D rb;
        private Vector2 startPos;
        private Vector2 lastPos;

        public Vector2 Velocity { get; private set; }

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            startPos = rb.position;
            lastPos = rb.position;
        }

        void FixedUpdate()
        {
            float x = startPos.x + Mathf.Sin(Time.time * speed) * amplitude;
            Vector2 target = new Vector2(x, startPos.y);

            rb.MovePosition(target);

            // Estimate platform velocity for rider support if needed
            Velocity = (rb.position - lastPos) / Time.fixedDeltaTime;
            lastPos = rb.position;
        }
    }
}
