using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.CompareTag("Tongue")){
            transform.SetParent(col.gameObject.transform);
        }
    }
}
