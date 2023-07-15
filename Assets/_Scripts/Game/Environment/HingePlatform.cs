using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Game.Environment;
using UnityEngine;

public class HingePlatform : Platform
{    
    public float dropDelay = 1f;
    public bool hasDropped = false;
    private Rigidbody2D rb;
    private HingeJoint2D joint;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        joint = gameObject.GetComponent<HingeJoint2D>();
    }

    void Update()
    {
        if(hasDropped){
            StartCoroutine(DelayedReplace());
        } 
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Frog")){
            StartCoroutine(DelayedDrop());
        }
    }

    private IEnumerator DelayedDrop()
    {
        yield return new WaitForSeconds(dropDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        hasDropped = true;
    }

    private IEnumerator DelayedReplace()
    {
        yield return new WaitForSeconds(dropDelay);
        hasDropped = false;
        joint.useMotor = true;
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = 100;
        joint.motor = motor;
    }
}
