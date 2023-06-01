/* 
    ==============================================
    This script was written by Kevin and Jason
    ==============================================
*/ 

using UnityEngine;

public class CharacterShooting : MonoBehaviour
{
    public Gun gun;
    public EnemyManager enemyManager;

    public int shootButton;
    public KeyCode reloadKey;

    void Start()
    {
        // Subscribe to the OnEnemyHit event
        enemyManager.OnEnemyHit += HandleEnemyHit;
        // Subscribe to the OnEnemyDeath event
        enemyManager.OnEnemyDeath += HandleEnemyDeath;
    }

    void Update()
    {
        if (Input.GetMouseButton(shootButton))
        {
            gun.Shoot();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            gun.Reload();
        }
    }

    void HandleEnemyHit()
    {
        // Perform actions when enemy is hit
        Debug.Log("Enemy Hit!");
    }

    void HandleEnemyDeath()
    {
        // Perform actions when enemy dies
        Debug.Log("Enemy Died!");
    }

    private void OnDestroy()
    {
        // Unsubscribe from the events when the script is destroyed
        enemyManager.OnEnemyHit -= HandleEnemyHit;
        enemyManager.OnEnemyDeath -= HandleEnemyDeath;
    }
}
