using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueMechanicsScript : MonoBehaviour
{
    public GameObject frog;

    public GameObject tongue;
    // Start is called before the first frame update
    private HingeJoint2D _frogHingeJoint2D;
    void Start()
    {
        _frogHingeJoint2D = tongue.GetComponent<HingeJoint2D>();
        //move the anchorpoint to match the frog's position on screen
        Vector2 frogPos = frog.transform.position;
        Vector2 tonguePos = _frogHingeJoint2D.anchor;
        tonguePos.x = frogPos.x;
        tonguePos.y = frogPos.y;
        Vector2 connector = _frogHingeJoint2D.connectedAnchor;
        connector.x = frogPos.x;
        connector.y = frogPos.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 touchPos = getTouchPos();
        

    }

    private Vector3 getTouchPos()
    {
        Vector3 vector3 = new Vector3();
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    return hit.transform.position;
                }
            }
        }
    }
}
