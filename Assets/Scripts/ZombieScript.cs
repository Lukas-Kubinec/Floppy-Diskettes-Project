using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class ZombieScript : MonoBehaviour
{
    [Header("Zombie stats")]
    public float attacksEverySeconds = 3.0f;
    //public float zombieHealth;
    //public float zombieSpeed;
    //public float zombieDamage;

    [Header("Other settings")]
    public ZombieData thisZombieData;
    public NavMeshAgent agent;

    //[Header("Other components")]
    //public GameManager gameManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        thisZombieData.SetStats();
        agent.speed = thisZombieData.zombieSpeed;
    }

    void OnDestroy()
    {
        ZombieSpawnManager.instance.zombies.Remove(thisZombieData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private IEnumerator AttackPlayer()
    {
        agent.speed = 0; // Stops the zombie
        GameManager.instance.healthManager.TakeDamage(thisZombieData.zombieDamage);
        yield return new WaitForSeconds(attacksEverySeconds);
        agent.speed = thisZombieData.zombieSpeed; // Zombie is moving
    }
}
