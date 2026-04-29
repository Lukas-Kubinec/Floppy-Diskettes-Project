using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Player Health")]
    public float currentPlayerHealth;
    public float maxHealth = 100;
    public int lives = 10;

    public void Start()
    {
        ResetHealth();
    }

    public void TakeDamage(float damageTaken)
    {
        currentPlayerHealth -= damageTaken;
        GameManager.instance.uiManager.UpdateHealthUI((int)currentPlayerHealth); // Updates Health UI
        CheckHealth();// Checks if players health is not zero
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
            GameManager.instance.weaponManager.ammo += GameManager.instance.characterTriggerCollisionController.ammoBoxAmount; 
            ResetHealth();
            GameManager.instance.uiManager.UpdateHealthUI((int)currentPlayerHealth); // Updates Health UI
            GameManager.instance.uiManager.UpdateAmmoUI(GameManager.instance.weaponManager.ammo); // Updates the Ammo UI
            GameManager.instance.skillManager.RespawnPlayerCentred();
        }

        if (0 >= lives)
        {
            // Player lost all their lives
            GameManager.instance.uiManager.ChangeStateOfGameOverUI(true); // Displays the Game Over UI
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Time.timeScale = 0.0f; // freezes time
        }
    }

    public void ResetHealth()
    {
        currentPlayerHealth = maxHealth;

    }
}
