using System;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class LevelPartBase : MonoBehaviour
    {
        protected const float DeadZone = -10f;
        private Transform _player;
        void Start()
        {
            _player = GameObject.FindWithTag("Frog").transform;
        }
        
        void Update()
        {
            if (transform != null && transform.position.y < _player.transform.position.y + DeadZone)
            {
                Destroy(gameObject);
            }
        }
    }
}