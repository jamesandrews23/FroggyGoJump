using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource[] _audioSources;
        // Start is called before the first frame update
        void Start()
        {
            _audioSources = GetComponents<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void PlaySource(int index)
        {
            var source = _audioSources[index];
            source.Play();
        }

        public void StopSource(int index)
        {
            var source = _audioSources[index];
            source.Stop();
        }
    }
}
