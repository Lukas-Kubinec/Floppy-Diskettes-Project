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
    public CameraEffects cameraEffects;

    // Game states variables
    bool worldGenerated = false;
    bool NavigationBaked = false;
    bool PlayerIsSpawned = false;
    bool gameIsReady = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!gameIsReady)
        {
            GameStartInOrder();
        }
    }

    private void GameStartInOrder()
    {
        // The first initial script that starts all the necessary components in one order, in order to minimise the conflicts.
        if (!worldGenerated)
        {
            cameraEffects.SetLoadingScreen(true);
            worldGenerator.BeginGenerateTerrainAndSpawns();
            if (worldGenerator.GetWorldIsReady()) { worldGenerated = true; } // Continues only if the world is built
        } else if (worldGenerated && !obstacleSpawnManager.AllObstaclesSpawned())
        {
            obstacleSpawnManager.SetObstacleSpawn(true);
        }
        else if (obstacleSpawnManager.AllObstaclesSpawned() && !NavigationBaked)
        {
            worldGenerator.BakeNavigation();
            NavigationBaked = true;
        }
        else if (NavigationBaked && !PlayerIsSpawned)
        {
            skillManager.RespawnPlayerCentred();
            PlayerIsSpawned = true;
            cameraEffects.SetLoadingScreen(false);
        }
        else if (PlayerIsSpawned)
        {
            zombieSpawnManager.SetZombieSpawn(true);
            gameIsReady = true; // World initiation is complete
        } 
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
    }

    // Cross-script used methods
    public int GetRandomIntWithProbability(int probability, int returnTrue, int returnFalse)
    {
        // Probability must be between 1 and 99 %
        int randomInt = Random.Range(1, 99);

        if (randomInt < probability)
        {
            return returnTrue;
        }
        else
        {
            return returnFalse;
        }
    }
}
