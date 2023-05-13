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
        private Transform _lastWallPosRight;
        private Transform _lastWallPosLeft;
        public Transform wallElement;
        public Vector3 leftWallPos = new Vector3(-2, 5, 0);
        public Vector3 rightWallPos = new Vector3(2, 5, 0);
        public float withinWallCheck = 0.25f;
        public float wallBuffer = 0.75f;
        private void Awake()
        {
            _leftBorder = FindLeftScreenBorder();
            _rightBorder = FindRightScreenBorder();
        
            _lastLevelPartPos = SpawnLevelPart(player.transform.position + new Vector3(0, -.5f, 0), levelParts[0]);
            _lastWallPosLeft = SpawnLevelPart(leftWallPos, wallElement);
            _lastWallPosRight = SpawnLevelPart(rightWallPos, wallElement);

            SpawnLevelPart();
        }

        private void Update()
        {
            //todo where magnitude
            if (Vector3.Distance(player.transform.position, _lastLevelPartPos.position) < PlayerDistSpawnLevelPart)
            {
                SpawnLevelPart();
                SpawnWallPart();
            }
        }

        private void SpawnWallPart()
        {
            Vector3 newPosRight = _lastWallPosRight.Find("NewPos").position;
            Vector3 newPosLeft = _lastWallPosLeft.Find("NewPos").position;
            _lastWallPosRight = SpawnLevelPart(newPosRight, wallElement);
            _lastWallPosLeft = SpawnLevelPart(newPosLeft, wallElement);
        }

        private void SpawnLevelPart()
        {
            Vector3 newPos = _lastLevelPartPos.Find("EndPos").position;
            newPos.z = 0;
            Transform chosenPart = levelParts[Random.Range(0, levelParts.Count)];
            // Bounds bounds = chosenPart.GetComponent<Renderer>().bounds;
            Debug.Log("New Position: " + newPos);

            if (newPos.x >= rightWallPos.x - withinWallCheck) //outside of the screen area
            {
                newPos.x = rightWallPos.x - wallBuffer;
                Debug.Log("Modified Position: " + newPos);
            } else if(newPos.x <= leftWallPos.x + withinWallCheck){
                newPos.x = leftWallPos.x + wallBuffer;
                Debug.Log("Modified Position: " + newPos);
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
            return newPos < leftWallPos.x || newPos > rightWallPos.x;
        }
    }
}
