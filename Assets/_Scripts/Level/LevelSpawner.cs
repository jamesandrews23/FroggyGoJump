using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelSpawner : MonoBehaviour
{
    public List<Transform> levelParts;
    public GameObject player;
    private Vector3 _lastPos;
    private const float PlayerDistSpawnLevelPart = 200f;
    private float _leftBorder;
    private float _rightBorder;
    private void Awake()
    {
        _leftBorder = FindLeftScreenBorder();
        _rightBorder = FindRightScreenBorder();
        
        _lastPos = SpawnLevelPart(player.transform.position + new Vector3(0, -.5f, 0), levelParts[0]).position;

        SpawnLevelPart();
        // for (int i = 0; i < 5; i++)
        // {
        //     SpawnLevelPart();
        // }
    }

    private void Update()
    {
        //todo where magnitude
        if (Vector3.Distance(player.transform.position, _lastPos) < PlayerDistSpawnLevelPart)
        {
            SpawnLevelPart();
        }
    }

    private void SpawnLevelPart()
    {
        Vector3 newPos = _lastPos;
        //if x was closer to the left, then move to the right, and vice-a-versa
        float nextPos = Random.Range(_leftBorder, _rightBorder);
        newPos.x = nextPos;
        Transform chosenPart = levelParts[Random.Range(0, levelParts.Count)];
        Transform spawnedPart = SpawnLevelPart(newPos, chosenPart);
        _lastPos = spawnedPart.Find("EndPos").position;
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
}
