using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _lastPos = new Vector3(0, 0, 0);

    void Update() {
        // Set the target position to follow the frog on the y-axis
        var transform1 = transform;
        var position1 = transform1.position;
        Vector3 targetPosition = position1;
        targetPosition.y = Mathf.SmoothDamp(position1.y, target.position.y, ref _velocity.y, smoothTime);

        // Set the x and z components of the target position to the current position of the camera
        var transform2 = transform;
        var position = transform2.position;
        targetPosition.x = position.x;
        targetPosition.z = position.z;

        // Move the camera to the target position
        
        position = targetPosition;
        if (position.y >= _lastPos.y)
        {
            _lastPos = position;
            transform2.position = position;
        }
    }
}
