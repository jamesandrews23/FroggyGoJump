using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class Sound : MonoBehaviour
    {
        public AudioSource source;

        public void PlayLegendarySound()
        {
            source.Play();
        }
    }
}
