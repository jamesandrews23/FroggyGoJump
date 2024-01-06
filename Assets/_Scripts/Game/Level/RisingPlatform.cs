using System.Collections;
using UnityEngine;

namespace _Scripts.Game.Level
{
    public class RisingPlatform : MonoBehaviour
    {
        private GameObject platform;
        private GameObject wheel;
        private bool isTouchHeld;
        private float touchStartTime;
        private RotatingWheel rotatingWheelScript;
        // Start is called before the first frame update
        private bool isWheelRotating;
        public float maxPlatformHeight;
        public AnimationCurve heightCurve;
        private bool isAscentCoroutineRunning;
        private bool isDescentCoroutineRunning;
        public float ascendDuration = 5f;
        public Transform player;
        private Coroutine ascentCoroutine;
        private Coroutine descentCoroutine;
        void Start()
        {
            platform = transform.GetChild(1).gameObject;
            wheel = transform.GetChild(0).gameObject;
            rotatingWheelScript = GetComponentInChildren<RotatingWheel>();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("isWheelRotating: " + rotatingWheelScript.isWheelSpinning);
            if (rotatingWheelScript.isWheelSpinning)
            {
                if (!isAscentCoroutineRunning || isDescentCoroutineRunning && ascentCoroutine == null)
                {
                    if(isDescentCoroutineRunning && descentCoroutine != null){
                        StopCoroutine(descentCoroutine);
                        isDescentCoroutineRunning = false;
                    }
                    ascentCoroutine = StartCoroutine(MovePlatform(ascendDuration, true));
                    isAscentCoroutineRunning = true;
                }
            }
            else
            {
                if (isAscentCoroutineRunning || !isDescentCoroutineRunning && ascentCoroutine != null && descentCoroutine == null)
                {
                    if(isAscentCoroutineRunning && ascentCoroutine != null){
                        StopCoroutine(ascentCoroutine);
                        isAscentCoroutineRunning = false;
                    }
                    descentCoroutine = StartCoroutine(MovePlatform(ascendDuration, false));
                    isDescentCoroutineRunning = true;
                }
            }
        }

        IEnumerator MovePlatform(float duration, bool isAscending)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = isAscending ? startPosition + Vector3.up * maxPlatformHeight : startPosition - Vector3.up * maxPlatformHeight;
            float timer = 0f;

            while (timer < duration)
            {
                float normalizedTime = timer / duration;
                Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
                transform.position = newPosition;
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
        }
    }
}
