using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("GameObjects")]
    public GameObject obstaclePrefab;

    // Other stuff
    private bool obstacleSpawnEnabled = false;
    public int obstaclesToSpawn = 5;
    private int obstaclesSpawned = 0;
    public List<GameObject> obstacles = new();

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public void SetObstacleSpawn(bool state)
    {
        obstacleSpawnEnabled = state;
    }

    public bool AllObstaclesSpawned()
    {
        return obstaclesToSpawn <= obstaclesSpawned;
    }

    private void Update()
    {
        if (obstacleSpawnEnabled)
        {
            StartSpawningObstacles();
        }
    }

    private void StartSpawningObstacles()
    {
        var spawnLocations = gameManager.worldGenerator.spawnLocations;
        int randomLocationIndex = UnityEngine.Random.Range(0, spawnLocations.Count);

        if (!AllObstaclesSpawned())
        {
            SpawnObstacle(spawnLocations[randomLocationIndex]);
        } else
        {
            // Disables obstacle spawn when enough is reached
            obstacleSpawnEnabled = false;
        }
    }

    void SpawnObstacle(Vector3 spawnLocation)
    {
        var obstacle = Instantiate(obstaclePrefab, spawnLocation, Quaternion.identity);
        obstacles.Add(obstacle);
        obstaclesSpawned++;
    }
}
