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
    }

    public void HealDamage(float damageHealed)
    {
        currentPlayerHealth += damageHealed;
    }

    public void CheckHealth()
    {
        if (currentPlayerHealth < 0)
        {
            GameManager.instance.skillManager.deaths++;
        }
    }

    public void ResetHealth()
    {
        currentPlayerHealth = maxHealth;
    }
}
