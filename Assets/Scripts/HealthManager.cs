using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Player Health")]
    public float currentPlayerHealth;
    public float maxHealth = 100;

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
        if (currentPlayerHealth < 0)
        {
            // Resets dead player 
            GameManager.instance.skillManager.deaths++;
            GameManager.instance.weaponManager.ammo += GameManager.instance.characterTriggerCollisionController.ammoBoxAmount; 
            ResetHealth();
            GameManager.instance.uiManager.UpdateHealthUI((int)currentPlayerHealth); // Updates Health UI
            GameManager.instance.uiManager.UpdateAmmoUI(GameManager.instance.weaponManager.ammo); // Updates the Ammo UI
            GameManager.instance.skillManager.RespawnPlayerCentred();
        }
    }

    public void ResetHealth()
    {
        currentPlayerHealth = maxHealth;
    }
}
