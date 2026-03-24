using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SkillManager skillManager;
    public HealthManager healthManager;
    public ZombieSpawnManager zombieSpawnManager;
    public InputManager inputManager;
    public WorldGenerator worldGenerator;
    public ObstacleSpawnManager obstacleSpawnManager;

    // Game states variables
    bool worldGenerated = false;
    bool NavigationBaked = false;
    bool PlayerIsSpawned = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        GameStartInOrder();
    }

    private void GameStartInOrder()
    {
        // The first initial script that starts all the necessary components in one order, in order to minimise the conflicts.
        if (!worldGenerated)
        {
            worldGenerator.BeginGenerateTerrainAndSpawns();
            worldGenerated = true;
        } else if (worldGenerated && !obstacleSpawnManager.AllObstaclesSpawned())
        {
            obstacleSpawnManager.SetObstacleSpawn(true);
        } else if (obstacleSpawnManager.AllObstaclesSpawned() && !NavigationBaked)
        {
            worldGenerator.BakeNavigation();
            NavigationBaked = true;
        } else if (NavigationBaked && !PlayerIsSpawned)
        {
            skillManager.RespawnPlayerCentred();
            PlayerIsSpawned = true;
        } else if (PlayerIsSpawned)
        {
            zombieSpawnManager.SetZombieSpawn(true);
        }
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
    }
}
