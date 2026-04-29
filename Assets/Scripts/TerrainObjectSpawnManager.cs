using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TerrainObjectSpawnManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Obstacles")]
    public GameObject RockPrefab;
    public GameObject CactusPrefab;
    public GameObject TreePrefab;
    private GameObject obstacle;
    private GameObject ObstaclesParent;

    [Header("Spawn settings")]
    public int obstaclesToSpawn = 5;
    public List<GameObject> obstacles = new();
    public List<GameObject> pickups = new();
    public int obstaclesSpawned = 0;
    public int healthPacksSpawned = 0;
    public int ammoPacksSpawned = 0;
    private bool objectSpawnEnabled = false;

    [Header("PickUps")]
    public GameObject AmmoBox;
    public GameObject HealthBox;
    public float heightOffset = 1.0f;
    public LayerMask checkLayerMaskCollision;
    private GameObject PickUpsParent;


    // Height level settings - taken from World Generator settings
    private float waterLevel; // deepest
    private float sandLevel;
    private float grassLevel;
    private float mountainLevel; // tallest

    private void Start()
    {
        gameManager = GameManager.instance;
        ObstaclesParent = new GameObject("ObstaclesParent");
        PickUpsParent = new GameObject("PickUpsParent");

        // Gets layer height levels
        waterLevel = gameManager.worldGenerator.waterHeightLevel;
        sandLevel = gameManager.worldGenerator.sandHeightLevel;
        grassLevel = gameManager.worldGenerator.grassHeightLevel;
        mountainLevel = gameManager.worldGenerator.mountainHeightLevel;
    }

    public void SetEnableObjectSpawn(bool state)
    {
        objectSpawnEnabled = state;
    }

    public bool AllObstaclesSpawned()
    {
        return obstaclesToSpawn <= obstaclesSpawned;
    }

    private void Update()
    {
        
        if (objectSpawnEnabled)
        {
            StartSpawningObstacles();
        }

        if (healthPacksSpawned < gameManager.skillManager.healthPackDropAmount || ammoPacksSpawned < gameManager.skillManager.ammoPackDropAmount)
        {
            SpawnPickUps();
        }
    }

    // Pickups
    private void SpawnPickUps()
    {
        // Assigns random x & y axis
        var worldSize = gameManager.worldGenerator.GetTerrainObject().terrainData.size;
        // Amounts to spawn
        var healthPacks = gameManager.skillManager.healthPackDropAmount;
        var ammoPacks = gameManager.skillManager.ammoPackDropAmount;
        var spawnSuccess = false;

        while (healthPacksSpawned < healthPacks)
        {
            spawnSuccess = SpawnPickUp(HealthBox, worldSize);
            if (spawnSuccess)
            {
                healthPacksSpawned++;
            }
        }
        
        while (ammoPacksSpawned < ammoPacks)
        {
            spawnSuccess = SpawnPickUp(AmmoBox, worldSize);
            if (spawnSuccess)
            {
                ammoPacksSpawned++;
            }
        }
    }

    private bool SpawnPickUp(GameObject SpawnObject, Vector3 worldSize)
    {
        var randomX = Random.Range(0, worldSize.x);
        var randomZ = Random.Range(0, worldSize.z);
        var height = gameManager.worldGenerator.GetTerrainObject().SampleHeight(new Vector3(randomX, 0, randomZ));

        var spawnLocation = new Vector3(randomX, height + heightOffset, randomZ);

        if (Physics.CheckSphere(spawnLocation,0.5f, checkLayerMaskCollision))
        {
            // Spawn location is touching another object, so it cannot be spawned here
            return false;
        } else
        {
            var pickup = Instantiate(SpawnObject, spawnLocation, Quaternion.identity);

            pickups.Add(pickup);
            pickup.transform.SetParent(PickUpsParent.transform, true);
            return true;
        }
    }

    // Obstacles
    private void StartSpawningObstacles()
    {
        // Spawn location
        var spawnLocations = gameManager.worldGenerator.spawnObstacleLocations;
        int randomLocationIndex = UnityEngine.Random.Range(0, spawnLocations.Count);

        // Calculation of the spawn height
        float heightAtPoint = gameManager.worldGenerator.GetTerrainObject().SampleHeight(spawnLocations[randomLocationIndex]);
        float heightMaximum = gameManager.worldGenerator.GetTerrainObject().terrainData.size.y;
        heightAtPoint /= (heightMaximum);

        if (AllObstaclesSpawned())
        {
            // Disables obstacle spawn when enough is reached
            objectSpawnEnabled = false;
        }
        else
        {
            SpawnObstacle(spawnLocations[randomLocationIndex], heightAtPoint);
        }
    }

    void SpawnObstacle(Vector3 spawnLocation, float heightAtPoint)
    {
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
            // Only Trees spawn on top of the mountains
            obstacle = Instantiate(TreePrefab, spawnLocation, Quaternion.identity);
        }

        // Rotates object in line with terrain surface
        if (Physics.Raycast(obstacle.transform.position + Vector3.up, Vector3.down, out RaycastHit hit))
        {
            obstacle.transform.up = hit.normal;
        }

        obstacles.Add(obstacle);
        obstacle.transform.SetParent(ObstaclesParent.transform, true);
        obstaclesSpawned++;
    }
}
