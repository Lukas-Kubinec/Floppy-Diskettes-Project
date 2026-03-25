using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class ZombieScript : MonoBehaviour
{
    [Header("Zombie stats")]
    public float zombieHealth;
    public float zombieSpeed;

    public ZombieData thisZombieData;
    public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnDestroy()
    {
        ZombieSpawnManager.instance.zombies.Remove(thisZombieData);
    }
}
