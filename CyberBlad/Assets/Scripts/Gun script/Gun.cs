/* 
    ==============================================
    This script was written by Jason
    ==============================================
*/ 

using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    public enum ShootState
    {
        Ready,
        Shooting,
        Reloading
    }

    private float muzzleOffset;

    [Header("Magazine")]
    public int ammunition;
    [Range(0.5f, 10)] public float reloadTime;

    private int remainingAmmunition;

    [Header("Shooting")]
    [Range(0.25f, 25)] public float fireRate;
    public int roundsPerShot;
    [Range(0, 45)] public float maxRoundVariation;

    private ShootState shootState = ShootState.Ready;
    private float nextShootTime = 0;

    [Header("UI")]
    public TMP_Text ammoText;

    [Header("Raycast")]
    public float hitScanRange; // Define the hit scan range

    void Start()
    {
        muzzleOffset = GetComponent<Renderer>().bounds.extents.z;
        remainingAmmunition = ammunition;
        UpdateAmmoText();
    }

    void Update()
    {
        switch (shootState)
        {
            case ShootState.Shooting:
                if (Time.time > nextShootTime)
                {
                    shootState = ShootState.Ready;
                }
                break;
            case ShootState.Reloading:
                if (Time.time > nextShootTime)
                {
                    remainingAmmunition = ammunition;
                    shootState = ShootState.Ready;
                    UpdateAmmoText();
                }
                break;
        }
    }

    public void Shoot()
    {
        if (shootState == ShootState.Ready)
        {
            for (int i = 0; i < roundsPerShot; i++)
            {
                // Perform hitscan logic
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, hitScanRange))
                {
                    // Check if the hit object has an EnemyManager component
                    EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
                    if (enemyManager != null)
                    {
                        bool enemyIsDead = enemyManager.TakeHit();
                        if (enemyIsDead)
                        {
                            // Enemy is dead, handle accordingly
                            // For example, you can destroy the enemy or give points to the player
                        }
                    }
                }
            }

            remainingAmmunition--;
            if (remainingAmmunition > 0)
            {
                nextShootTime = Time.time + (1 / fireRate);
                shootState = ShootState.Shooting;
            }
            else
            {
                Reload();
            }

            UpdateAmmoText();
        }
    }

    public void Reload()
    {
        if (shootState == ShootState.Ready)
        {
            nextShootTime = Time.time + reloadTime;
            shootState = ShootState.Reloading;
        }
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + remainingAmmunition.ToString();
        }
    }

    void DebugDrawRay()
    {
        // Get the direction based on the gun's forward vector
        Vector3 direction = transform.forward;

        // Calculate the end position of the ray
        Vector3 endPoint = transform.position + direction * hitScanRange;

        // Draw the ray in the scene view
        Debug.DrawRay(transform.position, direction * hitScanRange, Color.red);
    }
}