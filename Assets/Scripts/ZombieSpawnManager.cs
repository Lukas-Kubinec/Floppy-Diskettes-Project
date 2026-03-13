using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("ZombiePrefab")]
    public GameObject zombiePrefab;

    public float currentNumberOfZombiesSpawned;
    public float currentSpawnTimer;
    public float maxSpawnTimer = 3;
    public bool waveOver;


    private void Start()
    {
        gameManager = GameManager.instance;
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
                    SpawnZombie(Vector3.zero);
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
    }

    public void SpawnZombie(Vector3 spawnlocation)
    {
        Instantiate(zombiePrefab, spawnlocation, Quaternion.identity);
        currentNumberOfZombiesSpawned++;
    }

}
