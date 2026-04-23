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


    private void Start()
    {
        gameManager = GameManager.instance;
        canShoot = true;
        weaponStartingPosition = Weapon.transform.localPosition;
        weaponAudio = Weapon.GetComponent<AudioSource>();
    }

    private void Update()
    {
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

    private void ShakeGunRecoil()
    {
        var newZ = weaponStartingPosition.z - 0.2f;
        Weapon.transform.localPosition = new Vector3(weaponStartingPosition.x, weaponStartingPosition.y, newZ);
    }

    private void CheckGunPosition()
    {
        if (Weapon.transform.localPosition != weaponStartingPosition)
        {
            Weapon.transform.localPosition = Vector3.Lerp(Weapon.transform.localPosition, weaponStartingPosition, 0.1f);
        }
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
                    zombieScript.thisZombieData.zombieHealth -= 30;
                    gameManager.skillManager.UpdateAccuracy(true); // Updates accuracy by increasing accuracy rate

                    if (zombieScript.thisZombieData.zombieHealth <= 0)
                    {
                        Destroy(zombieScript.gameObject);
                        gameManager.skillManager.killsThisWave++;
                        gameManager.skillManager.totalkills++;
                        gameManager.uiManager.UpdateZombiesUI(gameManager.skillManager.numberOfZombiesToSpawn, gameManager.skillManager.totalkills);
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
