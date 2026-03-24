using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if (gameManager.inputManager.GetAttackInput())
        {
            gameManager.skillManager.kills++;
        }

        if (gameManager.skillManager.kills == gameManager.skillManager.numberOfZombiesToSpawn)
        {
            gameManager.skillManager.CalcNextWave();
        }
    }
}
