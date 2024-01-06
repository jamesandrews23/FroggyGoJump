using UnityEngine;

namespace _Scripts.Game.Level
{
    public class RotatingWheel : MonoBehaviour
    {
        public float rotationSpeed;
        public bool isWheelSpinning;
        private bool isBeingTouched = false;
        private CircleCollider2D wheelCollider;
        private Vector3 previousTouchPosition;
        private Quaternion initialRotation;
        private Quaternion targetRotation;
        private Quaternion currentRotation;
        private Quaternion previousRotation;

        void Start()
        {
            wheelCollider = GetComponent<CircleCollider2D>();
            initialRotation = transform.rotation;
            currentRotation = transform.rotation;
            isWheelSpinning = false;
        }

        void Update()
        {
            // if (Input.touchCount > 0)
            // {
            //     Touch touch = Input.GetTouch(0);

            //     Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            //     touchPos.z = 0;
            //     isBeingTouched = wheelCollider.OverlapPoint(touchPos);

            //     switch (touch.phase)
            //     {
            //         case TouchPhase.Began:
            //             previousTouchPosition = touchPos;
            //             break;
            //         case TouchPhase.Moved:
            //             if (isBeingTouched)
            //             {
            //                 Vector3 touchDirection = touchPos - previousTouchPosition;
            //                 float angle = Mathf.Atan2(touchDirection.y, touchDirection.x) * Mathf.Rad2Deg;

            //                 targetRotation = initialRotation * Quaternion.Euler(0f, 0f, angle);
            //                 currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            //                 transform.rotation = currentRotation;
            //                 previousTouchPosition = touchPos;
            //                 isWheelSpinning = true;
            //             }
            //             break;
            //         case TouchPhase.Ended:
            //             isWheelSpinning = false;
            //             break;
            //     }
            // }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;
                isBeingTouched = wheelCollider.OverlapPoint(touchPos);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        previousTouchPosition = touchPos;
                        break;
                    case TouchPhase.Moved:
                        if (isBeingTouched)
                        {
                            Vector3 touchDirection = touchPos - previousTouchPosition;
                            float angle = Mathf.Atan2(touchDirection.y, touchDirection.x) * Mathf.Rad2Deg;

                            targetRotation = initialRotation * Quaternion.Euler(0f, 0f, angle);
                            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
                            transform.rotation = currentRotation;
                            previousTouchPosition = touchPos;

                            // Calculate the difference between the current and previous rotation
                            Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);

                            // Convert the delta rotation to an angle
                            float rotationAngle = Quaternion.Angle(Quaternion.identity, deltaRotation);

                            // Check if the rotation angle is above a threshold to consider it as spinning
                            float rotationThreshold = 2f;
                            isWheelSpinning = rotationAngle > rotationThreshold;

                            previousRotation = currentRotation;
                        }
                        break;
                    case TouchPhase.Ended:
                        isWheelSpinning = false;
                        break;
                }
            }
        }
    }
}
