using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("GameObjects")]
    public GameObject RockPrefab;
    public GameObject CactusPrefab;
    public GameObject TreePrefab;
    private GameObject obstacle;

    [Header("Spawn settings")]
    private bool obstacleSpawnEnabled = false;
    public int obstaclesToSpawn = 5;
    private int obstaclesSpawned = 0;
    public List<GameObject> obstacles = new();
    private GameObject ObstaclesParent;

    // Height level settings - taken from World Generator settings
    private float waterLevel; // deepest
    private float sandLevel;
    private float grassLevel;
    private float mountainLevel; // tallest

    private void Start()
    {
        gameManager = GameManager.instance;
        ObstaclesParent = new GameObject("ObstaclesParent");

        // Gets layer height levels
        waterLevel = gameManager.worldGenerator.waterHeightLevel;
        sandLevel = gameManager.worldGenerator.sandHeightLevel;
        grassLevel = gameManager.worldGenerator.grassHeightLevel;
        mountainLevel = gameManager.worldGenerator.mountainHeightLevel;
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
        var spawnLocations = gameManager.worldGenerator.spawnObstacleLocations;
        int randomLocationIndex = UnityEngine.Random.Range(0, spawnLocations.Count);

        var heightmapLocations = gameManager.worldGenerator.generatedTerrainHeightsValues;
        var heightAtPoint = heightmapLocations[(int)spawnLocations[randomLocationIndex].y, (int)spawnLocations[randomLocationIndex].x];

        if (!AllObstaclesSpawned())
        {
            SpawnObstacle(spawnLocations[randomLocationIndex], heightAtPoint);
        } else
        {
            // Disables obstacle spawn when enough is reached
            obstacleSpawnEnabled = false;
        }
    }

    void SpawnObstacle(Vector3 spawnLocation, float heightAtPoint)
    {
        // Gets int based on %chance probability
        var chance = gameManager.GetRandomIntWithProbability(50, 1, 0);

        if (heightAtPoint < waterLevel * gameManager.worldGenerator.heightAdjustment)
        {
            // Only rocks spawn in water
            obstacle = Instantiate(RockPrefab, spawnLocation, Quaternion.identity);
        } else if (heightAtPoint < sandLevel * gameManager.worldGenerator.heightAdjustment)
        {
            // Rocks and Cactus
            if (chance == 0)
            {
                obstacle = Instantiate(RockPrefab, spawnLocation, Quaternion.identity);
            } else
            {
                obstacle = Instantiate(CactusPrefab, spawnLocation, Quaternion.identity);
            }
        }
        else if (heightAtPoint < grassLevel * gameManager.worldGenerator.heightAdjustment)
        {
            // Rocks and Trees
            if (chance == 0)
            {
                obstacle = Instantiate(RockPrefab, spawnLocation, Quaternion.identity);
            }
            else
            {
                obstacle = Instantiate(TreePrefab, spawnLocation, Quaternion.identity);
            }
        }
        else if (heightAtPoint < mountainLevel * gameManager.worldGenerator.heightAdjustment)
        {
            // Only Trees on top of the mountains
            obstacle = Instantiate(TreePrefab, spawnLocation, Quaternion.identity);
        }

        // Rotates object in line with terrain surface
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit))
        {
            this.transform.up = hit.normal;
        }

        obstacles.Add(obstacle);
        obstacle.transform.SetParent(ObstaclesParent.transform, true);
        obstaclesSpawned++;
    }
}
