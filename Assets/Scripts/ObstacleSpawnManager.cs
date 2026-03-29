using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("GameObjects")]
    public GameObject RockPrefab;
    public GameObject LowRockPrefab;
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
        // Size ratio between Heights and Spawns map
        var sizeRatio = gameManager.worldGenerator.GetSizeRatio();

        // Spawn location
        var spawnLocations = gameManager.worldGenerator.spawnObstacleLocations;
        int randomLocationIndex = UnityEngine.Random.Range(0, spawnLocations.Count);
        var heightmapLocations = gameManager.worldGenerator.generatedTerrainHeightsValues;
        float mapX = spawnLocations[randomLocationIndex].x*sizeRatio;
        float mapY = spawnLocations[randomLocationIndex].y*sizeRatio;

        // Calculation of the spawn height
        float heightAtPoint = gameManager.worldGenerator.GetTerrainObject().SampleHeight(spawnLocations[randomLocationIndex]);
        float heightMaximum = gameManager.worldGenerator.GetTerrainObject().terrainData.size.y;
        float heightAdjustment = gameManager.worldGenerator.heightAdjustment;
        heightAtPoint = heightAtPoint / (heightMaximum * heightAdjustment);

        if (AllObstaclesSpawned())
        {
            // Disables obstacle spawn when enough is reached
            obstacleSpawnEnabled = false;
        }
        else
        {
            SpawnObstacle(spawnLocations[randomLocationIndex], heightAtPoint);
        }
    }

    void SpawnObstacle(Vector3 spawnLocation, float heightAtPoint)
    {
        float heightAdjustment = gameManager.worldGenerator.heightAdjustment;
        var chance = gameManager.GetRandomIntWithProbability(50, 1, 0); // Gets int based on %chance probability
        if (heightAtPoint < waterLevel)
        {
            // Only rocks spawn in water
            obstacle = Instantiate(RockPrefab, spawnLocation, Quaternion.identity);
        } 
        else if (heightAtPoint < sandLevel)
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
        else if (heightAtPoint < grassLevel)
        {
            obstacle = Instantiate(TreePrefab, spawnLocation, Quaternion.identity);
        }
        else
        {
            // Only Low Rocks spawn on top of the mountains
            obstacle = Instantiate(LowRockPrefab, spawnLocation, Quaternion.identity);
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
