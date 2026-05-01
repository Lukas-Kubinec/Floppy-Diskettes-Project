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
    private bool isAttacking;
    private bool isWalking;

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
            animPlayer.clip = attackAnim;
        } else if (isWalking)
        {
            animPlayer.clip = walkingAnim;
        }
    }

    private void LateUpdate()
    {
        HandleAnimations();




        if (zombieIsSlowed && slowedDownTimer < slowedForSecondsWhenShot)
        {
            // Zombie is slowed down
            slowedDownTimer += Time.deltaTime;
            agent.speed = agent.speed / 2f;
        } 
        else
        {
            // Zombie is set to full speed
            slowedDownTimer = 0f;
            zombieIsSlowed = false;
            agent.speed = normalSpeed;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AttackPlayer());
        }
    }

    public void SlowDownZombieWhenShot()
    {
        zombieIsSlowed = true;
    }

    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtClip);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        isWalking = false;
        agent.speed = 0; // Stops the zombie
        GameManager.instance.healthManager.TakeDamage(thisZombieData.zombieDamage);
        yield return new WaitForSeconds(attacksEverySeconds);
        agent.speed = thisZombieData.zombieSpeed; // Zombie is moving
        isAttacking = false;
        isWalking = true;
    }
}
