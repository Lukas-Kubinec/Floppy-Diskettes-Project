using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("What changes")]
    public float numberOfZombiesToSpawn = 5;
    public float zombieSpeed;
    public float zombieHealth;
    public float zombieDamage;
    public float healthPackDropRate;
    public float zombieSpawnRate;


    [Header("What effects Changes")]
    public float totalkills;
    public float killsThisWave;
    public float accuracy;
    public float damageTaken;
    public float deaths;
    public float waveCount;

    [Header("Otherstuff")]
    public float numberofZombiesprevwave;
    public GameManager gameManager;
    public GameObject playerObject;

    public void Start()
    {
        gameManager = GameManager.instance;
    }

    public void RespawnPlayerCentred()
    {
        var xTerrainCentre = gameManager.worldGenerator.xSpawnSize / 2;
        var yTerrainCentre = gameManager.worldGenerator.zSpawnSize / 2;
        var worldTerrain = gameManager.worldGenerator.GetTerrainObject();
        var heightTerrainCentre = worldTerrain.SampleHeight(new Vector3(xTerrainCentre,0, yTerrainCentre)) ;

        playerObject.transform.position = new Vector3(xTerrainCentre,heightTerrainCentre + 2.0f , yTerrainCentre);
    }

    public void CalcNextWave()
    {
        waveCount++;

        numberofZombiesprevwave = numberOfZombiesToSpawn;
        numberOfZombiesToSpawn = totalkills + waveCount + (accuracy / 10) - (damageTaken / 10) - deaths;
        zombieSpawnRate = numberOfZombiesToSpawn / waveCount;
        killsThisWave = 0;

        if (numberOfZombiesToSpawn < numberofZombiesprevwave)
            gameManager.GameOver();
        else
        {
            gameManager.zombieSpawnManager.waveOver = false;
            gameManager.zombieSpawnManager.currentNumberOfZombiesSpawned = 0;
        }
    }

    public void HealthPacks(){
        healthPackDropRate = gameManager.healthManager.maxHealth - gameManager.healthManager.currentPlayerHealth;
    }

    public void CalcZombieHealth()
    {
        zombieHealth = 100 + ((numberOfZombiesToSpawn * 10 + accuracy)/2);
    }

    public void CalcZombieSpeed() 
    {
        zombieSpeed = 5 + waveCount - (Mathf.Sqrt(damageTaken) / 10 ) - deaths;
    }

    public void CalcZombieDamage()
    {
        zombieDamage = 100 / zombieSpeed;
    }
}
