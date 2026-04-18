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
    public int bullets;
    private float shotDelay;
    private bool canShoot;

    [Header("Weapon object")]
    public GameObject Weapon;
    private Vector3 startPos;


    private void Start()
    {
        gameManager = GameManager.instance;
        canShoot = true;
        startPos = Weapon.transform.localPosition;
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

    private void ShakeGunRecoil()
    {
        var newZ = startPos.z - 0.2f;
        Weapon.transform.localPosition = new Vector3(startPos.x, startPos.y, newZ);
    }

    private void CheckGunPosition()
    {
        if (Weapon.transform.localPosition != startPos)
        {
            Weapon.transform.localPosition = Vector3.Lerp(Weapon.transform.localPosition, startPos, 0.1f);
        }
    }

    private void HandleShooting()
    {
        RaycastHit hit;

        if (gameManager.inputManager.GetAttackInput() && canShoot)
        {
            // Disables shooting for X seconds
            canShoot = false;
            shotDelay = 0;

            gameManager.cameraEffects.ShowShootingEffect(); // Shows the gun shot trails/line
            ShakeGunRecoil(); // Moves gun when shooting

            // Checks if something is hit
            if (Physics.Raycast(CinemaCam.transform.position, CinemaCam.transform.forward, out hit, weaponRange, ZombieLayerMask))
            {
                var zombieScript = hit.collider.GetComponentInParent<ZombieScript>();
                if (zombieScript != null)
                {
                    zombieScript.zombieHealth -= 30;

                    if (zombieScript.zombieHealth <= 0)
                    {
                        Destroy(zombieScript.gameObject);
                        gameManager.skillManager.killsThisWave++;
                        gameManager.skillManager.totalkills++;
                    }
                }
            }
        }

        if (gameManager.skillManager.killsThisWave == gameManager.skillManager.numberOfZombiesToSpawn)
        {
            gameManager.skillManager.CalcNextWave();
        }
    }
}
