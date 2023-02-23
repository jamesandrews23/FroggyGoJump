using System;
using UnityEngine;

namespace _Scripts.Game.Environment
{
    public class LevelPartBase : MonoBehaviour
    {
        private const float DeadZone = -50f;
        private Transform _player;
        private void Start()
        {
            _player = GameObject.FindWithTag("Frog").transform;
        }
        
        private void Update()
        {
            if (transform.position.y < _player.transform.position.y + DeadZone)
            {
                Destroy(gameObject);
            }
        }
    }
}