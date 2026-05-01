using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class ZombieScript : MonoBehaviour
{
    [Header("Zombie settings")]
    public float attacksEverySeconds = 3.0f;
    public float slowedForSecondsWhenShot = 0.5f;
    private bool zombieIsSlowed = false;
    private float slowedDownTimer = 0f;
    private float normalSpeed;
    private bool isAttacking = false;
    private bool isWalking = true;

    [Header("Components")]
    public ZombieData thisZombieData;
    public NavMeshAgent agent;

    [Header("Sound Effects")]
    public AudioClip hurtClip;
    public AudioClip deathClip;
    private AudioSource audioSource;

    [Header("Animations")]
    public Animation animPlayer;
    public AnimationClip walkingAnim;
    public AnimationClip attackAnim;
    private float animationTimer = 0f;
    private bool isInPlayersTrigger = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        thisZombieData.SetStats();
        agent.speed = thisZombieData.zombieSpeed;
        normalSpeed = agent.speed;
        audioSource = GetComponent<AudioSource>();
    }

    private void HandleAnimations()
    {
        if (isAttacking)
        {
            //animPlayer.clip = attackAnim;
            animPlayer.Play("Zombie|Attack");
        } 
        else if (isWalking)
        {
            //animPlayer.clip = walkingAnim;
            animPlayer.Play("Zombie|Walk");
        }
    }

    private void HandleSlowDown()
    {
        if (zombieIsSlowed && slowedDownTimer < slowedForSecondsWhenShot)
        {
            // Zombie is slowed down
            slowedDownTimer += Time.deltaTime;
            agent.speed = agent.speed / 2f;
        }
        else if (zombieIsSlowed)
        {
            // Zombie is set to full speed
            slowedDownTimer = 0f;
            zombieIsSlowed = false;
            agent.speed = normalSpeed;
        }
    }

    private void Update()
    {
        HandleSlowDown();
        HandleAnimations();

        if (isAttacking)
        {
            CheckIfNearPlayer();
        }
    }

    void OnDestroy()
    {
        ZombieSpawnManager.instance.zombies.Remove(thisZombieData);
    }

    public void PlayDeathSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deathClip);
    }

    public void SlowDownZombieWhenShot()
    {
        zombieIsSlowed = true;
    }

    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtClip);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayersTrigger = true;
            animationTimer += Time.deltaTime;

            if (isWalking)
            {
                StartAttack();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayersTrigger = false;
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        agent.speed = 0; // Stops the zombie
        isWalking = false;
        HandleAnimations();
    }

    private void CheckIfNearPlayer()
    {
        if (isInPlayersTrigger && animationTimer >= attackAnim.length)
        {
            AttackPlayer();
        } 
        else if (!isInPlayersTrigger)
        {
            ResetZombieToNormalState();
        }
    }

    private void AttackPlayer()
    {
        GameManager.instance.healthManager.TakeDamage(thisZombieData.zombieDamage);
        ResetZombieToNormalState();
    }

    private void ResetZombieToNormalState()
    {
        animationTimer = 0f;
        isAttacking = false;
        isWalking = true;
        agent.speed = thisZombieData.zombieSpeed; // Zombie is moving
    }
}
