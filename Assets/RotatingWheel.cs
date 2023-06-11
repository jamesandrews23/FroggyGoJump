using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        wheelCollider = GetComponent<CircleCollider2D>();
        initialRotation = transform.rotation;
        currentRotation = transform.rotation;
        isWheelSpinning = false;
    }

    void Update()
    {
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
                        isWheelSpinning = true;
                    }
                    break;
                case TouchPhase.Ended:
                    isWheelSpinning = false;
                    break;
            }
        }
    }
}
