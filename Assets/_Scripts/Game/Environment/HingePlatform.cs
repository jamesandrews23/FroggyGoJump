using System.Collections;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class HingePlatform : LevelPartBase
    {
        public float dropDelay = 1f;
        public float returnDelay = 1f;
        public float dropAngle = -90f;

        private Quaternion originalRotation;
        private Quaternion targetRotation;
        private bool isDropping = false;

        private void Start()
        {
            originalRotation = transform.rotation;
            targetRotation = originalRotation * Quaternion.Euler(0f, 0f, dropAngle);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Frog") && !isDropping)
            {
                isDropping = true;
                StartCoroutine(DropAndReturn());
            }
        }

        private IEnumerator DropAndReturn()
        {
            yield return new WaitForSeconds(dropDelay);

            // Drop the platform
            StartCoroutine(RotatePlatform(targetRotation));

            yield return new WaitForSeconds(returnDelay);

            // Return the platform to its original position
            StartCoroutine(RotatePlatform(originalRotation));

            isDropping = false;
        }

        private IEnumerator RotatePlatform(Quaternion targetRotation)
        {
            Quaternion startRotation = transform.rotation;
            float elapsedTime = 0f;
            float rotationDuration = 1f; // Adjust as needed for the desired rotation speed

            while (elapsedTime < rotationDuration)
            {
                float t = elapsedTime / rotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the platform ends at the target rotation
            transform.rotation = targetRotation;
        }
    }
}
