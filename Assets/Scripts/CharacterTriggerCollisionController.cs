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
    public GameObject WaterPlane;


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
        }
        else if (other.CompareTag(healthTag))
        {
            Destroy(other.gameObject); // deles the health box
            GameManager.instance.healthManager.HealDamage(healthBoxAmount);

        }
    }

    // Under Water check
    private void CheckIfUnderWater()
    {
        // Turns on/off the underwater effect
        if (Camera.main.transform.position.y < WaterPlane.transform.position.y)
        {
            GameManager.instance.cameraEffects.SetUnderWaterScreen(true);
        } else
        {
            GameManager.instance.cameraEffects.SetUnderWaterScreen(false);
        }
    }
}
