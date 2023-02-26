using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Level
{
    public class LevelSpawner : MonoBehaviour
    {
        public List<Transform> levelParts;
        public GameObject player;
        private Transform _lastLevelPartPos;
        private const float PlayerDistSpawnLevelPart = 15f;
        private float _leftBorder;
        private float _rightBorder;
        private void Awake()
        {
            _leftBorder = FindLeftScreenBorder();
            _rightBorder = FindRightScreenBorder();
        
            _lastLevelPartPos = SpawnLevelPart(player.transform.position + new Vector3(0, -.5f, 0), levelParts[0]);

            SpawnLevelPart();
        }

        private void Update()
        {
            //todo where magnitude
            if (Vector3.Distance(player.transform.position, _lastLevelPartPos.position) < PlayerDistSpawnLevelPart)
            {
                SpawnLevelPart();
            }
        }

        private void SpawnLevelPart()
        {
            Vector3 newPos = _lastLevelPartPos.Find("EndPos").position;
            Transform chosenPart = levelParts[Random.Range(0, levelParts.Count)];

            if (IsNewPosOutOfBounds(newPos.x)) //outside of the screen area
            {
                float randomPosBetweenBounds = Random.Range(_leftBorder, _rightBorder);
                newPos.x = randomPosBetweenBounds;
            }
            _lastLevelPartPos = SpawnLevelPart(newPos, chosenPart);
        }

        private Transform SpawnLevelPart(Vector3 position, Transform levelPart)
        {
            return Instantiate(levelPart, position, Quaternion.identity);
        }

        private float FindRightScreenBorder()
        {
            Camera mainCamera = Camera.main;
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            return mainCamera.transform.position.x + cameraWidth / 2f;
        }
    
        private float FindLeftScreenBorder()
        {
            Camera mainCamera = Camera.main;
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            return mainCamera.transform.position.x - cameraWidth / 2f;
        }

        private bool IsNewPosOutOfBounds(float newPos)
        {
            return newPos < _leftBorder || newPos > _rightBorder;
        }
    }
}
