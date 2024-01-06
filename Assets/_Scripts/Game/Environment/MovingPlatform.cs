using System;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class MovingPlatform : Platform
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public float moveSpeed;
        public float delayTime;
        private float t = 0f;
        private bool isMovingToEnd = true;
        private float leftScreenBorder;
        private float rightScreenBorder;
        public Transform player;

        private void Start(){
            leftScreenBorder = UtilityFunctions.FindLeftScreenBorder();
            rightScreenBorder = UtilityFunctions.FindRightScreenBorder();
          
            startPos = new Vector3(rightScreenBorder - 1f, transform.position.y, 0);
            endPos = new Vector3(leftScreenBorder + 1f, transform.position.y, 0);

            moveSpeed = UnityEngine.Random.Range(0f, 1f);
            delayTime = UnityEngine.Random.Range(0f, 1f);

            player = GameObject.FindWithTag("Frog").transform;
        }
        private void Update(){
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // Increment or decrement the interpolation parameter based on the move direction and speed
            if (isMovingToEnd)
                t += moveSpeed * Time.deltaTime;
            else
                t -= moveSpeed * Time.deltaTime;

            // Check if the platform has reached the end position
            if (t >= 1f && isMovingToEnd)
            {
                t = 1f;  // Ensure the platform reaches exactly the end position
                isMovingToEnd = false;  // Change the direction of movement
                StartCoroutine(DelayAtEndPosition());  // Start the delay coroutine
            }
            else if (t <= 0f && !isMovingToEnd)
            {
                t = 0f;  // Ensure the platform reaches exactly the start position
                isMovingToEnd = true;  // Change the direction of movement
                StartCoroutine(DelayAtEndPosition());  // Start the delay coroutine
            }

            if(player.transform.position.y + DeadZone >= transform.position.y){
                Destroy(gameObject);
            }
        }

        // Coroutine to add a delay at each end position
        private System.Collections.IEnumerator DelayAtEndPosition()
        {
            // Disable movement during the delay
            enabled = false;

            // Wait for the specified delay time
            yield return new WaitForSeconds(delayTime);

            // Enable movement after the delay
            enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Frog"))
            {
                collision.gameObject.transform.SetParent(transform);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Frog"))
            {
                collision.gameObject.transform.SetParent(null);
            }
        }
    }
}