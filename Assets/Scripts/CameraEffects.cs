using Unity.VisualScripting;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("Normal settings")]
    public Color NormalScreenColour;

    [Header("Dead Screen")]
    public Color DeathScreenColour;
    private bool isDead = false;

    [Header("UnderWater effect")]
    public Color UnderWaterColour;
    public GameObject WaterPlane;
    private bool isUnderWater = false;

    [Header("Loading Screen")]
    public Color LoadingScreenColour;
    private bool isLoading = false;

    private void Awake()
    {
        if (NormalScreenColour != null)
        {
            // Gets the default fog colour if not set in inspector
            NormalScreenColour = RenderSettings.fogColor;
        }
    }

    private void FixedUpdate()
    {
        IsLoading();
        IsDead();
        CheckIfUnderWater();
        IsUnderWater();
    }

    private void SetNormalScreen()
    {
        RenderSettings.fogColor = NormalScreenColour;
        RenderSettings.fogDensity = 0.01f;
    }

    // Death Screen
    public void SetDeadScreen(bool state)
    {
        isDead = state;
    }

    public bool GetDeadScreen ()
    {
        return isDead;
    }

    private void IsDead()
    {
        if (isDead)
        {
            RenderSettings.fogColor = DeathScreenColour;
        } else
        {
            SetNormalScreen();        
        }
    }

    // UnderWater Screen
    public void SetUnderWaterScreen(bool state)
    {
        isUnderWater = state;
    }

    public bool GetUnderWaterScreen()
    {
        return isUnderWater;
    }

    private void CheckIfUnderWater()
    {
        if (transform.position.y < WaterPlane.transform.position.y )
        {
            // Checks if camera is under water
            isUnderWater = true;
        } else
        {
            isUnderWater = false;
        }
    }

    private void IsUnderWater()
    {
        if (isUnderWater)
        {
            RenderSettings.fogColor = UnderWaterColour;
            RenderSettings.fogDensity = 0.5f;
        }
        else
        {
            SetNormalScreen();
        }
    }

    // Loading Screen
    public void SetLoadingScreen(bool state)
    {
        isLoading = state;
    }

    public bool GeLoadingScreen()
    {
        return isLoading;
    }

    private void IsLoading()
    {
        if (isLoading)
        {
            RenderSettings.fogColor = LoadingScreenColour;
            RenderSettings.fogDensity = 1.0f;
        }
        else
        {
            SetNormalScreen();
        }
    }
}
