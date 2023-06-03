using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Frog")){
            AddToCollection();
            Destroy(gameObject);
        }
    }

    public virtual void AddToCollection(){
        //override this
    }
}
