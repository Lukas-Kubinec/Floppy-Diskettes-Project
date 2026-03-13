using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SkillManager skillManager;
    public HealthManager healthManager;
    public ZombieSpawnManager zombieSpawnManager;
    public InputManager inputManager;


    private void Awake()
    {
        instance = this;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
    }
}
