using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class Consumable : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Frog"))
            {
                Destroy(gameObject);
            }
        }
    }
}
