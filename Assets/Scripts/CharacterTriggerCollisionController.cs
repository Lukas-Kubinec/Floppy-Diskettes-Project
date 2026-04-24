using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterTriggerCollisionController : MonoBehaviour
{
    // Triggers
    [Header("Ammo Box")]
    public string ammoTag;
    public int ammoBoxAmount = 25;

    [Header("Health Box")]
    public string healthTag;
    public int healthBoxAmount = 25;

    // Colliders


    // Game Objects

    [Header("Water Plane")]
    public float DamageAfterDelay = 3f;
    public float waterDamage = 0.1f;
    public GameObject WaterPlane;
    private float DamageCurrentTimer = 0f;


    private void Awake()
    {
        if (WaterPlane == null)
        {
            WaterPlane = GameObject.FindGameObjectWithTag("Water");
        }
    }

    private void FixedUpdate()
    {
        CheckIfUnderWater();
    }

    // Trigger check
    private void OnTriggerEnter(Collider other)
    {
        // Checks for ammo tags
        if (other.CompareTag(ammoTag)) {
            Destroy(other.gameObject); // deles the ammo box
            GameManager.instance.weaponManager.ReloadAmmo(ammoBoxAmount);
            GameManager.instance.weaponManager.UpdateAmmo();
            GameManager.instance.objectSpawnManager.ammoPacksSpawned--; // Allows new pack to be spawned
        }
        else if (other.CompareTag(healthTag))
        {
            Destroy(other.gameObject); // deles the health box
            GameManager.instance.healthManager.HealDamage(healthBoxAmount);
            GameManager.instance.objectSpawnManager.healthPacksSpawned--; // Allows new pack to be spawned
        }
    }

    // Under Water check
    private void CheckIfUnderWater()
    {
        // Turns on/off the underwater effect
        if (Camera.main.transform.position.y < WaterPlane.transform.position.y)
        {
            GameManager.instance.cameraEffects.SetUnderWaterScreen(true);

            DamageCurrentTimer += Time.fixedDeltaTime;
            if (DamageCurrentTimer > DamageAfterDelay)
            {
                WaterDamage();
            }
        } 
        else
        {
            GameManager.instance.cameraEffects.SetUnderWaterScreen(false);
            DamageCurrentTimer = 0f;
        }
    }

    private void WaterDamage()
    {
        GameManager.instance.healthManager.TakeDamage(waterDamage);
    }
}
