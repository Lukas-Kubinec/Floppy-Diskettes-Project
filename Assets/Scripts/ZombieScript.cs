using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class ZombieScript : MonoBehaviour
{
    [Header("Zombie stats")]
    //public float zombieHealth;
    //public float zombieSpeed;
    //public float zombieDamage;

    [Header("Other settings")]
    public ZombieData thisZombieData;
    public NavMeshAgent agent;

    [Header("Other components")]
    public GameManager gameManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        thisZombieData.SetStats();
    }

    void OnDestroy()
    {
        ZombieSpawnManager.instance.zombies.Remove(thisZombieData);
    }
}
