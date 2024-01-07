using TMPro.Examples;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class Consumable : MonoBehaviour
    {
        private AudioManager _audioManager;

        private void Start()
        {
            _audioManager = GameObject.Find("AudioManagerObject").GetComponent<AudioManager>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Frog"))
            {
                _audioManager.PlaySource(3);
                Destroy(gameObject);
            }
        }
    }
}
