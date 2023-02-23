using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelSpawner : MonoBehaviour
{
    public List<Transform> levelParts;
    public GameObject player;
    private Vector3 _lastLevelPartPos;
    private const float PlayerDistSpawnLevelPart = 200f;
    private float _leftBorder;
    private float _rightBorder;
    public float maxHeight = 3f;
    public float minHeight = 1f;
    public float maxWidth = 3f;
    public float minWidth = 1f;
    private void Awake()
    {
        _leftBorder = FindLeftScreenBorder();
        _rightBorder = FindRightScreenBorder();
        
        _lastLevelPartPos = SpawnLevelPart(player.transform.position + new Vector3(0, -.5f, 0), levelParts[0]).position;

        SpawnLevelPart();
        // for (int i = 0; i < 5; i++)
        // {
        //     SpawnLevelPart();
        // }
    }

    private void Update()
    {
        //todo where magnitude
        if (Vector3.Distance(player.transform.position, _lastLevelPartPos) < PlayerDistSpawnLevelPart)
        {
            SpawnLevelPart();
        }
    }

    private void SpawnLevelPart()
    {
        Vector3 newPos = _lastLevelPartPos;
        //if x was closer to the left, then move to the right, and vice-a-versa
        Transform chosenPart = levelParts[Random.Range(0, levelParts.Count)];
        float nextPos = Random.Range(_leftBorder, _rightBorder);
        newPos.x = nextPos - chosenPart.GetComponent<BoxCollider2D>().bounds.size.x;
        Transform spawnedPart = SpawnLevelPart(newPos, chosenPart);
        _lastLevelPartPos = spawnedPart.Find("EndPos").position;
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
