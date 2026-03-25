using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieData
{
    public NavMeshAgent nav;
}

public class ZombieSpawnManager : MonoBehaviour
{
    public GameManager gameManager;
    public static ZombieSpawnManager instance;

    [Header("GameObjects")]
    private GameObject player;
    public GameObject zombiePrefab;

    public float currentNumberOfZombiesSpawned;
    public float currentSpawnTimer;
    public float maxSpawnTimer = 3;
    public bool waveOver = false;
    private bool zombieSpawnEnabled = false;

    public List<ZombieData> zombies = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        player = gameManager.skillManager.playerObject;
    }

    public void SetZombieSpawn(bool state)
    {
        zombieSpawnEnabled = state;
    }

    private void Update()
    {
        if (zombieSpawnEnabled)
        {
            StartSpawningZombies();
        }
    }

    private void StartSpawningZombies()
    {
        currentSpawnTimer += Time.deltaTime;

        if (currentSpawnTimer >= maxSpawnTimer && !waveOver)
        {
            for (int i = 0; i <= gameManager.skillManager.zombieSpawnRate; i++)
            {
                if (currentNumberOfZombiesSpawned < gameManager.skillManager.numberOfZombiesToSpawn)
                {
                    CreateZombieSpawner();
                }
                else
                {
                    break;
                }

            }
            currentSpawnTimer = 0;

            if (currentNumberOfZombiesSpawned == gameManager.skillManager.numberOfZombiesToSpawn)
            {
                waveOver = true;
            }
        }

        for (int i = 0; i < zombies.Count; i++)
        {
            ZombieData data = zombies[i];
            data.nav.SetDestination(player.transform.position);
        }
    }

    private void CreateZombieSpawner()
    {
        var spawnLocations = gameManager.worldGenerator.spawnLocations;
        int spawnLocationsLenght = UnityEngine.Random.Range(0, spawnLocations.Count);

        SpawnZombie(spawnLocations[spawnLocationsLenght]);
    }

    public void SpawnZombie(Vector3 spawnLocation)
    {
        var currentZombie = Instantiate(zombiePrefab, spawnLocation, Quaternion.identity);
        var zombieScript = currentZombie.GetComponent<ZombieScript>();
        //ZombieData data = new() { nav = zombieScript.agent, zombieSpeed = 10f };
        ZombieData data = new ZombieData { nav = zombieScript.agent};
        zombieScript.thisZombieData = data;
        zombies.Add(data);
        currentNumberOfZombiesSpawned++;
    }
}
