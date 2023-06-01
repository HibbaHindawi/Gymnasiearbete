/*
    ==============================================
    This script was written by Kevin and Jason
    ==============================================
*/

using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private int healthPoints = 30;

    public delegate void EnemyHitDelegate();
    public event EnemyHitDelegate OnEnemyHit;

    public delegate void EnemyDeathDelegate();
    public event EnemyDeathDelegate OnEnemyDeath;

    private void Awake() {
        healthPoints = 30;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Projectile")) {
            TakeHit();
        }
    }

    public void TakeHit() // Changed the return type to void
    {
        healthPoints -= 10;

        // Invoke the OnEnemyHit event
        if (OnEnemyHit != null) {
            OnEnemyHit.Invoke();
        }

        if (healthPoints <= 0) {
            Die();
        }
    }

    private void Die() // Moved the method outside of TakeHit()
    {
        // Invoke the OnEnemyDeath event
        if (OnEnemyDeath != null) {
            OnEnemyDeath.Invoke();
        }

        Destroy(gameObject);
    }
}