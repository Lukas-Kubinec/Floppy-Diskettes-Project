using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Bottom Panel
    [Header("Player Info")]
    public TextMeshProUGUI PlayerHealth;
    public TextMeshProUGUI PlayerDeaths;
    public TextMeshProUGUI PlayerLives;
    public TextMeshProUGUI PlayerAmmo;

    // Upper Panel
    [Header("Zombie Info")]
    public TextMeshProUGUI ZombiesSpawned;
    public TextMeshProUGUI ZombiesKilled;

    [Header("Wave Info")]
    public TextMeshProUGUI WaveNumber;
    public TextMeshProUGUI WaveCompletion;

    [Header("Accuracy Info")]
    public TextMeshProUGUI ShotsAccuracy;
    public TextMeshProUGUI TotalShots;

    // Loading UI
    [Header("Loading UI")]
    public GameObject LoadingUI;

    // LOADING UI
    public void ChangeStateOfLoadingUI(bool state)
    {
        LoadingUI.SetActive(state);
    }

    // TOP PANEL
    // Zombies
    public void UpdateZombiesUI(int zombiesSpawned, int zombiesKilled)
    {
            ZombiesSpawned.text = "Spawned: " + zombiesSpawned.ToString();
            ZombiesKilled.text = "Total kills: " + zombiesKilled.ToString();
    }

    // Wave
    public void UpdateWaveUI(int waveNum, int completion)
    {
        WaveNumber.text = "Wave number: " + waveNum.ToString();
        WaveCompletion.text = "Completion: " + completion.ToString();
    }

    // Accuracy
    public void UpdateAccuracyUI(float accuracy, int total)
    {
        ShotsAccuracy.text = "Accuracy: " + accuracy.ToString() + "%";
        TotalShots.text = "Total shots: " + total.ToString();
    }

    // BOTTOM PANEL
    // Health
    public void UpdateHealthUI(int health)
    {
        PlayerHealth.text = "Health: " + health.ToString();
    }

    // Deaths
    public void UpdateDeathsUI(int deaths)
    {
        PlayerDeaths.text = "Deaths: " + deaths.ToString();
    }

    // Lives
    public void UpdateLivesUI(int lives)
    {
        PlayerLives.text = "Lives: " + lives.ToString();
    }

    // Ammo
    public void UpdateAmmoUI(int ammo)
    {
        PlayerAmmo.text = "Ammo: " + ammo.ToString();
    }
}
