using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    private GameObject platform;
    private GameObject wheel;
    private bool isTouchHeld;
    private float touchStartTime;
    private float touchHoldDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        platform = transform.GetChild(0).gameObject;
        wheel = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.touchCount > 0){
        //     Touch touch = Input.GetTouch(0);

        //     if (touch.phase == TouchPhase.Began)
        //     {
        //         touchStartTime = Time.time;
        //         isTouchHeld = true;
        //     }
        //     else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        //     {
        //         isTouchHeld = false;
        //     }
        // }

        // if (isTouchHeld){
        //     float touchDuration = Time.time - touchStartTime;

        //     if (touchDuration >= touchHoldDuration)
        //     {
        //         // Touch and hold gesture detected, perform actions here
        //     }
        // }
    }
}
