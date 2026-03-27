using Unity.Cinemachine;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameManager gameManager;
    public CinemachineCamera CinemaCam;
    public LayerMask ZombieLayerMask;

    [Header("Weapon settings")]
    public float weaponRange;
    private float shotDelay;
    public float timebetweenBullets;
    private bool canShoot;
    public int bullets;

    private void Start()
    {
        gameManager = GameManager.instance;
        canShoot = true;
    }

    private void Update()
    {
        ShotDelayTimer();
        HandleShooting();
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

    private void HandleShooting()
    {
        RaycastHit hit;

        if (gameManager.inputManager.GetAttackInput())
        {
            if (Physics.Raycast(CinemaCam.transform.position, CinemaCam.transform.forward, out hit, weaponRange, ZombieLayerMask) && canShoot)
            {
                canShoot = false;
                shotDelay = 0;


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
