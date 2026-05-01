using Unity.Cinemachine;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameManager gameManager;
    public CinemachineCamera CinemaCam;
    public LayerMask ZombieLayerMask;

    [Header("Weapon settings")]
    public float weaponRange;
    public float timebetweenBullets;
    public int ammo;
    private float shotDelay;
    private bool canShoot;

    [Header("Weapon object")]
    public GameObject Weapon;
    public AudioClip[] weaponAudioClips;
    public float weaponAudioRandomMin; // Min limit used for audio adjustment
    public float weaponAudioRandomMax; // Max limit used for audio adjustment
    private Vector3 weaponStartingPosition;
    private AudioSource weaponAudio;
    private float recoilInterpolation = 0f; // Used to control the value of gun recoil movement
    private float bobInterpolation = 0f; // Used to control the value of gun bob movement
    private bool bobMovingLeft = true;
    private bool PlayerIsMoving = false; // Used to check if there is movement input

    [Header("Weapon Movement Offsets")]
    public float recoilOffset = 0.2f;
    public float bobOffset = 0.2f;



    private void Start()
    {
        gameManager = GameManager.instance;
        canShoot = true;
        weaponStartingPosition = Weapon.transform.localPosition;
        weaponAudio = Weapon.GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Checks if there is any movement input applied
        PlayerIsMoving = (gameManager.inputManager.GetMovementInput() != Vector2.zero);

        ShotDelayTimer();
        HandleShooting();
        CheckGunPosition();
    }
    void ShotDelayTimer()
    {
        if(shotDelay < timebetweenBullets && !canShoot)
        {
            shotDelay += Time.deltaTime;
        }
        else
        {
            canShoot = true;
        }
    }

    private void PlayShotAudio()
    {
        var chosenIndex = Random.Range(0, weaponAudioClips.Length);
        var randomPitch = Random.Range(weaponAudioRandomMin, weaponAudioRandomMax);
        weaponAudio.pitch = randomPitch;
        weaponAudio.PlayOneShot(weaponAudioClips[chosenIndex]);
    }


    // Gun shooting recoil
    private void ShakeGunRecoil()
    {
        var newZ = weaponStartingPosition.z - recoilOffset;
        Weapon.transform.localPosition = new Vector3(Weapon.transform.localPosition.x, Weapon.transform.localPosition.y, newZ);
    }

    // Moves gun back
    private void CheckGunPosition()
    {
        Vector3 newPos = new Vector3(0f ,Weapon.transform.localPosition.y, 0f);
        float newX, newZ;
        // Checks Bob movement
        if (PlayerIsMoving)
        {
            // Moves the gun left/right (weapon bob effect)
            if (bobMovingLeft)
            {
                bobInterpolation += Time.deltaTime;
                newX = Mathf.Lerp(weaponStartingPosition.x, weaponStartingPosition.x - bobOffset, Mathf.SmoothStep(0, 1, bobInterpolation));
                if (bobInterpolation > 1)
                {
                    bobMovingLeft = false;
                }
            } 
            else
            {
                bobInterpolation -= Time.deltaTime;
                newX = Mathf.Lerp(weaponStartingPosition.x - bobOffset, weaponStartingPosition.x, Mathf.SmoothStep(1, 0, bobInterpolation));
                if (bobInterpolation < 0)
                {
                    bobMovingLeft = true;
                }
            }
            newPos.x = newX;
        }
        else
        {
            bobInterpolation += Time.deltaTime;
            newX = Mathf.Lerp(Weapon.transform.localPosition.x, weaponStartingPosition.x, Mathf.SmoothStep(0, 1, bobInterpolation));
            newPos.x = newX;
        }

        // Checks Recoild movement
        if (Weapon.transform.localPosition.z != weaponStartingPosition.z)
        {
            recoilInterpolation += Time.deltaTime;
            newZ = Mathf.Lerp(Weapon.transform.localPosition.z, weaponStartingPosition.z, Mathf.SmoothStep(0,1,recoilInterpolation));
            newPos.z = newZ;
        } else
        {
            recoilInterpolation = 0;
        }

        // Moves gun to new position every frame
        Weapon.transform.localPosition = Vector3.Lerp(Weapon.transform.localPosition, newPos, 0.1f);
    }

    public void UpdateAmmo()
    {
        ammo--; // Removes one bullet
        gameManager.uiManager.UpdateAmmoUI(ammo);
    }

    public void ReloadAmmo(int amount)
    {
        ammo += amount;
    }

    private void HandleShooting()
    {
        RaycastHit hit;

        if (gameManager.inputManager.GetAttackInput() && canShoot && ammo > 0)
        {
            // Disables shooting for X seconds
            canShoot = false;
            shotDelay = 0;

            UpdateAmmo(); // removes one bullet

            PlayShotAudio(); // Plays the weapon audio clip
            gameManager.cameraEffects.ShowShootingEffect(); // Shows the gun shot trails/line
            ShakeGunRecoil(); // Moves gun when shooting

            // Checks if something is hit
            if (Physics.Raycast(CinemaCam.transform.position, CinemaCam.transform.forward, out hit, weaponRange, ZombieLayerMask))
            {
                var zombieScript = hit.collider.GetComponentInParent<ZombieScript>();
                if (zombieScript != null)
                {
                    // Takes health from zombie
                    zombieScript.thisZombieData.zombieHealth -= 30;
                    gameManager.skillManager.UpdateAccuracy(true); // Updates accuracy by increasing accuracy rate


                    if (zombieScript.thisZombieData.zombieHealth <= 0)
                    {
                        zombieScript.PlayDeathSound();
                        Destroy(zombieScript.gameObject);
                        gameManager.skillManager.killsThisWave++;
                        gameManager.skillManager.totalkills++;
                        gameManager.uiManager.UpdateZombiesUI(gameManager.skillManager.numberOfZombiesToSpawn, gameManager.skillManager.totalkills);
                    } else
                    {
                        // Plays the hurt sound when shot
                        zombieScript.PlayHurtSound();
                        // Slows down the zombie while it still has health
                        zombieScript.SlowDownZombieWhenShot();
                    }

                    gameManager.skillManager.CalculateWaveCompletion(); // Updates wave completion 
                }
            } else
            {
                // Zombie missed
                gameManager.skillManager.UpdateAccuracy(false); // Updates accuracy without increasing accuracy rate
            }
        }

        // Checks if all zombies were killed
        if (gameManager.skillManager.killsThisWave >= gameManager.skillManager.numberOfZombiesToSpawn)
        {
            gameManager.skillManager.CalcNextWave();
        }
    }
}
