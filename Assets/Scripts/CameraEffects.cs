using Unity.VisualScripting;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    // Shooting Effects
    [Header("Shooting effect")]
    public LineRenderer shotLine;
    public GameObject gunStartPoint;
    public float shotLineLenghth;
    public float shotLineVisibleForTime;
    private float shotLineCurrentTimer;
    private bool shotIsVisible = false;

    // Screen Effects
    [Header("Normal settings")]
    private Color NormalFogColour;
    private float NormalFogDensity;

    [Header("Dead Screen")]
    public Color DeathScreenColour;
    public float DeathFogDensity;
    private bool isDead = false;

    [Header("UnderWater effect")]
    public Color UnderWaterColour;
    public float UnderWaterFogDensity;
    private bool isUnderWater = false;

    [Header("Loading Screen")]
    public Color LoadingScreenColour;
    public float LoadingFogDensity;
    private bool isLoading = false;

    private void Awake()
    {
        // Gets the default fog colour & density
        NormalFogColour = RenderSettings.fogColor;
        NormalFogDensity = RenderSettings.fogDensity;
    }

    private void FixedUpdate()
    {
        HandleScreenEffects();
        HandleShotLine();
    }

    void HandleShotLine()
    {
        if (shotIsVisible)
        {
            shotLineCurrentTimer += Time.fixedDeltaTime;
        }

        if (shotLineCurrentTimer >= shotLineVisibleForTime)
        {
            shotIsVisible = false;
            shotLine.gameObject.SetActive(false);
            shotLineCurrentTimer = 0;
        }
    }

    public void ShowShootingEffect()
    {
        shotIsVisible = true;
        shotLine.gameObject.SetActive(true);
        shotLine.SetPosition(0, gunStartPoint.transform.position);

        Vector3 endPoint = transform.position + transform.forward * shotLineLenghth;
        shotLine.SetPosition(1, endPoint);
    }

    private void SetScreenEffect(Color effectColour, float fogDensity)
    {
        RenderSettings.fogColor = effectColour;
        RenderSettings.fogDensity = fogDensity;
    }

    private void HandleScreenEffects()
    {
        if (isLoading)
        {
            SetScreenEffect(LoadingScreenColour, LoadingFogDensity); // Adjusts the colour and density of fog
        }
        else if (isDead)
        {
            SetScreenEffect(DeathScreenColour, DeathFogDensity);
        }
        else if (isUnderWater)
        {
            SetScreenEffect(UnderWaterColour, UnderWaterFogDensity);
        }
        else
        {
            SetScreenEffect(NormalFogColour, NormalFogDensity); // Default settings
        }
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

    // UnderWater Screen
    public void SetUnderWaterScreen(bool state)
    {
        isUnderWater = state;
    }

    public bool GetUnderWaterScreen()
    {
        return isUnderWater;
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
}
