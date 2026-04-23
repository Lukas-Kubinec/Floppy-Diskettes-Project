using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("What changes")]
    public int numberOfZombiesToSpawn = 5;
    public float healthPackDropAmount;
    public float ammoPackDropAmount;
    public float zombieSpawnRate;

    [Header("Zombie initial stats")]
    public float zombieSpeed;
    public float zombieHealth;
    public float zombieDamage;

    [Header("What effects changes")]
    public int totalkills = 0;
    public int killsThisWave = 0;
    public float accuracy = 0;
    public float damageTaken = 0;
    public int deaths = 0;
    public int waveCount = 0;

    [Header("Other stuff")]
    public int numberofZombiesprevwave;
    public GameManager gameManager;
    public GameObject playerObject;

    // other Wave variables
    private float completionRate = 0f;

    // other Accuracy variables
    private int accuracySuccessfulHits = 0;
    private int accuracyTotalHits = 0;

    public void Start()
    {
        gameManager = GameManager.instance;

        // Prepares some UI elements at start of game
        gameManager.uiManager.UpdateZombiesUI(numberOfZombiesToSpawn, totalkills);
    }

    public void RespawnPlayerCentred()
    {
        var xTerrainCentre = gameManager.worldGenerator.xSpawnSize / 2;
        var yTerrainCentre = gameManager.worldGenerator.zSpawnSize / 2;
        var worldTerrain = gameManager.worldGenerator.GetTerrainObject();
        var heightTerrainCentre = worldTerrain.SampleHeight(new Vector3(xTerrainCentre,0, yTerrainCentre)) ;

        playerObject.transform.position = new Vector3(xTerrainCentre,heightTerrainCentre + 2.0f , yTerrainCentre);
    }

    public void CalculateWaveCompletion()
    {
        completionRate = (float)killsThisWave / (float)numberOfZombiesToSpawn * 100f;

        gameManager.uiManager.UpdateWaveUI(waveCount, (int)(completionRate));
    }

    public void CalcNextWave()
    {
        // Calculates stats for new wave
        waveCount++;
        numberofZombiesprevwave = numberOfZombiesToSpawn;
        numberOfZombiesToSpawn = (int)((float)totalkills + (float)waveCount + (accuracy / 10f) - (damageTaken / 10f) - (float)deaths);
        zombieSpawnRate = numberOfZombiesToSpawn / waveCount;

        // Resets variables
        completionRate = 0f;
        killsThisWave = 0;

        // Updates the UI
        gameManager.uiManager.UpdateWaveUI(waveCount, (int)(completionRate));
        gameManager.uiManager.UpdateZombiesUI(numberOfZombiesToSpawn, totalkills);

        if (numberOfZombiesToSpawn < numberofZombiesprevwave)
            // If player does realy poorly, the game is over
            gameManager.GameOver();
        else
        {
            gameManager.zombieSpawnManager.waveOver = false;
            gameManager.zombieSpawnManager.currentNumberOfZombiesSpawned = 0;
        }

        RecalculateNewZombieData();
    }


    // Recalculation of zombie data
    public void RecalculateNewZombieData()
    {
        CalcZombieHealth();
        CalcZombieSpeed();
        CalcZombieDamage();
    }

    private void CalcZombieHealth()
    {
        // Zombie health depends on how well the player is doing
        zombieHealth = 100 + ((numberOfZombiesToSpawn * 10 + accuracy)/2);
    }

    private void CalcZombieSpeed() 
    {
        // Zombie speed depends on how well the player is doing as well as current wave
        zombieSpeed = 5 + waveCount - (Mathf.Sqrt(damageTaken) / 10 ) - deaths;
    }

    private void CalcZombieDamage()
    {
        // The faster the zombie, the lower the damage
        zombieDamage = 100 / zombieSpeed;
    }

    // Handling of Accuracy
    public void UpdateAccuracy(bool successHit)
    {
        if (successHit)
        {
            accuracySuccessfulHits++;
        }
        accuracyTotalHits++;
        accuracy = (float)accuracySuccessfulHits / (float)accuracyTotalHits * 100f;

        // Updats the accuracy UI
        gameManager.uiManager.UpdateAccuracyUI(Mathf.Round(accuracy), accuracyTotalHits);
    }
}
