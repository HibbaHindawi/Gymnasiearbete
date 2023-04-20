/* 
    ==============================================
    
    This script was written by Kevin

    Additioanl information:
        Currently in use for the player to test 
        stuff, however this is meant to be an 
        Enemy Manager.

    ==============================================
*/ 

using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private int _healthpoints;

    private void Awake() {
        _healthpoints = 30;
    }

    private void OnCollisionEnter(Collision projectile) {
        if (projectile.gameObject.tag == "Projectile") {
            TakeHit();
        }
    }

    public bool TakeHit() {
        _healthpoints -= 10;
        bool isDead = _healthpoints <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die() {
        Destroy(obj: gameObject);
    }
}
