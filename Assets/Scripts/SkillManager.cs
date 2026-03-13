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
    public float kills;
    public float accuracy;
    public float damageTaken;
    public float deaths;
    public float waveCount;

    [Header("Otherstuff")]
    public float numberofZombiesprevwave;
    public GameManager gameManager;

    public void Start()
    {
        gameManager = GameManager.instance;
    }

    public void calcnextwave()
    {
        waveCount++;

        numberofZombiesprevwave = numberOfZombiesToSpawn;
        numberOfZombiesToSpawn = kills + waveCount + (accuracy / 10) - (damageTaken / 10) - deaths;
        zombieSpawnRate = numberOfZombiesToSpawn / waveCount;

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

    public void calcZombieHealth()
    {
        zombieHealth = 100 + ((numberOfZombiesToSpawn * 10 + accuracy)/2);
    }

    public void calcZombieSpeed() 
    {
        zombieSpeed = 5 + waveCount - (Mathf.Sqrt(damageTaken) / 10 ) - deaths;
    }

    public void calcZombieDamage()
    {
        zombieDamage = 100 / zombieSpeed;
    }
}
