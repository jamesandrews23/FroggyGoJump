using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTongue : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private SpringJoint2D _springJoint;
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _springJoint = GetComponent<SpringJoint2D>();

        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.1f;
    }

    private void LateUpdate()
    {
        if (_springJoint.enabled)
        {
            _lineRenderer.enabled = true;
            
            Vector3 pos1 = _springJoint.connectedBody.transform.position;
            Vector3 pos2 = _springJoint.anchor;
    
            _lineRenderer.SetPosition(0, pos1);
            _lineRenderer.SetPosition(1, pos2 + new Vector3(0, -1, 0));
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }
}
