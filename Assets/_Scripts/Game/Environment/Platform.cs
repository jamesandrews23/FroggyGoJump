using System;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class Platform : LevelPartBase
    {
        

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Frog"))
            {
                Rigidbody2D frog = col.GetComponent<Rigidbody2D>();

                if (frog.velocity.y > 0)
                {
                    var box2d = gameObject.GetComponent<BoxCollider2D>();
                    box2d.enabled = false;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Frog"))
            {
                var box2d = gameObject.GetComponent<BoxCollider2D>();
                box2d.enabled = true;
            }
        }
    }
}
