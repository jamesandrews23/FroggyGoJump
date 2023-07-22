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
        private Transform _lastWallPosRight;
        private Transform _lastWallPosLeft;
        public Transform wallElement;
        private Vector3 _leftWallPos;
        private Vector3 _rightWallPos;
        private float _nearWallCheck;
        private float _wallBuffer;
        public GameObject consumable;
        public float consumableBuffer = 5f;
        private void Awake()
        {
            _leftWallPos = new Vector3(FindLeftScreenBorder(), 5, 0);
            _rightWallPos = new Vector3(FindRightScreenBorder(), 5, 0);

            _nearWallCheck = _rightWallPos.x * 0.25f;
            _wallBuffer = _rightWallPos.x * 0.75f;
        
            _lastLevelPartPos = SpawnLevelPart(player.transform.position + new Vector3(0, -.5f, 0), levelParts[0]);
            _lastWallPosLeft = SpawnLevelPart(_leftWallPos, wallElement);
            _lastWallPosRight = SpawnLevelPart(_rightWallPos, wallElement);

            SpawnLevelPart();
        }

        private void Update()
        {
            //todo where magnitude
            if (Vector3.Distance(player.transform.position, _lastLevelPartPos.position) < PlayerDistSpawnLevelPart)
            {
                SpawnLevelPart();
                SpawnWallPart();
                SpawnCollectable();
            }
        }

        private void SpawnConsumable()
        {

        }

        private void SpawnCollectable()
        {
            Vector3 position = new Vector3(Random.Range(FindRightScreenBorder(), FindLeftScreenBorder()), player.transform.position.y * consumableBuffer, 0);
            GameObject newCoin = Instantiate(consumable);
            newCoin.transform.position = position;
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
            Debug.Log("Screen Left Border: " + FindLeftScreenBorder());
            Debug.Log("Screen Right Border: " + FindRightScreenBorder());
            Debug.Log("New Position: " + newPos);


            if (newPos.x >= _rightWallPos.x - _nearWallCheck) //outside of the screen area
            {
                newPos.x = _rightWallPos.x - _wallBuffer;
                Debug.Log("Modified Position: " + newPos);
            } else if(newPos.x <= _leftWallPos.x + _nearWallCheck){
                newPos.x = _leftWallPos.x + _wallBuffer;
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
            return newPos < _leftWallPos.x || newPos > _rightWallPos.x;
        }
    }
}
