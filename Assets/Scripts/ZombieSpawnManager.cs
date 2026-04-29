using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieData
{
    public NavMeshAgent nav;
    public float zombieHealth;
    public float zombieSpeed;
    public float zombieDamage;
    public GameManager gameManager;

    public void SetStats()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameManager>();
        zombieSpeed = gameManager.skillManager.zombieSpeed;
        zombieHealth = gameManager.skillManager.zombieHealth;
        zombieDamage = gameManager.skillManager.zombieDamage;
    }


}

public class ZombieSpawnManager : MonoBehaviour
{
    public GameManager gameManager;
    public static ZombieSpawnManager instance;

    [Header("Game Objects")]
    public GameObject zombiePrefab;
    private GameObject player;
    private GameObject zombiesParent;

    [Header("Settings")]
    public float currentNumberOfZombiesSpawned;
    public float currentSpawnTimer;
    public float maxSpawnTimer = 3;
    public bool waveOver = false;
    public LayerMask checkLayerMaskCollision;
    public float zombieSpawnDistanceFromPlayer = 10.0f;
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
        zombiesParent = new GameObject("ZombiesParent");
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
        var spawnLocations = gameManager.worldGenerator.spawnZombieLocations;
        int spawnLocationsLenght = UnityEngine.Random.Range(0, spawnLocations.Count);

        // Ensures zombie is not spawned near the player
        if (!Physics.CheckSphere(spawnLocations[spawnLocationsLenght], zombieSpawnDistanceFromPlayer, checkLayerMaskCollision))
        {
            SpawnZombie(spawnLocations[spawnLocationsLenght]);
        }
    }

    public void SpawnZombie(Vector3 spawnLocation)
    {
        var currentZombie = Instantiate(zombiePrefab, spawnLocation, Quaternion.identity);
        var zombieScript = currentZombie.GetComponent<ZombieScript>();
        //ZombieData data = new() { nav = zombieScript.agent, zombieSpeed = 10f };
        ZombieData data = new() { nav = zombieScript.agent };
        zombieScript.thisZombieData = data;
        zombies.Add(data);
        currentZombie.transform.SetParent(zombiesParent.transform, true);
        currentNumberOfZombiesSpawned++;
    }
}
