using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class TongueMechanicsScript : MonoBehaviour
{
    public GameObject frog;

    public GameObject tongue;
    // Start is called before the first frame update
    private HingeJoint2D _frogHingeJoint2D;

    private Vector2 connectedTonguePoint;

    private bool tongueShouldBeAnchored;
    void Start()
    {
        _frogHingeJoint2D = tongue.GetComponent<HingeJoint2D>();
        //move the anchorpoint to match the frog's position on screen
        // Vector2 frogPos = frog.transform.position;
        // Vector2 tonguePos = _frogHingeJoint2D.anchor;
        // tonguePos.x = frogPos.x;
        // tonguePos.y = frogPos.y;
        // Vector2 connector = _frogHingeJoint2D.connectedAnchor;
        // connector.x = frogPos.x;
        // connector.y = frogPos.y;
    }

    // Update is called once per frame
    void Update()
    {
        var rayCast = getTouchHit();
        if (rayCast.collider != null)
        {
            connectedTonguePoint = rayCast.point;
            _frogHingeJoint2D.enabled = true;
            tongueShouldBeAnchored = true;
            tongue.transform.position = connectedTonguePoint;
            _frogHingeJoint2D.anchor = Vector2.zero;
        }
    }

    private RaycastHit2D getTouchHit()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                return hit;
            }
        }

        return new RaycastHit2D();
    }
}
