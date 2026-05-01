using UnityEngine;
using UnityEngine.Audio;

public class HealthManager : MonoBehaviour
{
    [Header("Player Health")]
    public float currentPlayerHealth;
    public float maxHealth = 100;
    public int lives = 10;

    [Header("Sound Effects")]
    public AudioClip hurtClip;
    private AudioSource audioSource;

    public void Start()
    {
        ResetHealth();
        audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
    }

    public void TakeDamage(float damageTaken)
    {
        PlayHurtSoundEffect();
        currentPlayerHealth -= damageTaken;
        GameManager.instance.uiManager.UpdateHealthUI((int)currentPlayerHealth); // Updates Health UI
        CheckHealth();// Checks if players health is not zero
    }

    private void PlayHurtSoundEffect()
    {
        if (audioSource.isPlaying) { 
                audioSource.PlayOneShot(hurtClip);
        }
    }

    public void HealDamage(float damageHealed)
    {
        currentPlayerHealth += damageHealed;
        GameManager.instance.uiManager.UpdateHealthUI((int)currentPlayerHealth); // Updates Health UI
    }

    public void CheckHealth()
    {
        if (currentPlayerHealth < 0 && lives >= 1)
        {
            // Takes one live off
            lives--;
            GameManager.instance.uiManager.UpdateLivesUI(lives);

            // Resets dead player 
            GameManager.instance.skillManager.deaths++;
            GameManager.instance.uiManager.UpdateDeathsUI(GameManager.instance.skillManager.deaths);

            // Checks if player has lost all their lives
            if (0 >= lives)
            {
                // Player lost all their lives
                GameManager.instance.uiManager.ChangeStateOfGameOverUI(true); // Displays the Game Over UI
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Time.timeScale = 0.0f; // freezes time
            } 
            else
            {
                // Resets the player if they still have lives remaining
                // Gives player more ammo
                GameManager.instance.weaponManager.ammo += GameManager.instance.characterTriggerCollisionController.ammoBoxAmount;
                GameManager.instance.uiManager.UpdateAmmoUI(GameManager.instance.weaponManager.ammo); // Updates the Ammo UI
                // Gives player full health
                ResetHealth();
                GameManager.instance.uiManager.UpdateHealthUI((int)currentPlayerHealth); // Updates Health UI
                // Resets player on their spawn point
                GameManager.instance.skillManager.RespawnPlayerCentred();
            }
        }

    }

    public void ResetHealth()
    {
        currentPlayerHealth = maxHealth;

    }
}
