using UnityEngine;

public class CharacterTriggerCollisionController : MonoBehaviour
{
    // Triggers


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
