using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Game.Environment;
using UnityEngine;

public class HingePlatform : Platform
{    
    public float dropDelay = 1f;

    private void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.CompareTag("Frog")){
            StartCoroutine(DelayedDrop());
        }
    }

    private IEnumerator DelayedDrop(){
        yield return new WaitForSeconds(dropDelay);
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
