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
    public GameObject player;
    public GameObject zombiePrefab;
    public GameObject spawnPrefab;

    public float currentNumberOfZombiesSpawned;
    public float currentSpawnTimer;
    public float maxSpawnTimer = 3;
    public bool waveOver;

    public List<ZombieData> zombies = new List<ZombieData>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void CreateZombieSpawner()
    {

    }

    private void Update()
    {
        currentSpawnTimer += Time.deltaTime;

        if (currentSpawnTimer >= maxSpawnTimer && !waveOver)
        {
            for (int i = 0; i <= gameManager.skillManager.zombieSpawnRate; i++)
            {
                if (currentNumberOfZombiesSpawned < gameManager.skillManager.numberOfZombiesToSpawn)
                {
                    SpawnZombie(spawnPrefab.transform.position);
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

    public void SpawnZombie(Vector3 spawnlocation)
    {
        var currentZombie = Instantiate(zombiePrefab, spawnlocation, Quaternion.identity);
        var zombieScript = currentZombie.GetComponent<ZombieScript>();
        ZombieData data = new ZombieData { nav = zombieScript.agent};
        zombieScript.thisZombieData = data;
        zombies.Add(data);
        currentNumberOfZombiesSpawned++;
    }
}
