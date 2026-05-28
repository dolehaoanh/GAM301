using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Setup Fields")]
    public Transform partToRotate;  // Drag the turret's head/body that spins here
    public Transform firePoint;     // Drag the tip of the gun barrel here (bullet spawn point)
    public GameObject bulletPrefab; // Drag your BulletPrefab here

    [Header("Attributes")]
    public float range = 8f;          // Shooting range
    public float fireRate = 1f;       // Speed of shooting (1 bullet per second)
    public float rotationSpeed = 10f;  // Speed of aiming/looking at target

    private Transform target;
    private float fireCountdown = 0f;

    void Start()
    {
        // Optimization: Find the nearest target 5 times a second instead of 60 times a second to save CPU!
        InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null) return;

        // 1. Rotate the turret head smoothly to look at the enemy (Y-axis only)
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // 2. Count down and shoot
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        // Instantiate the bullet at the nozzle position and facing direction
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    // Draw the range sphere centered on the turret head instead of the parent's offset pivot!
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (partToRotate != null)
        {
            Gizmos.DrawWireSphere(partToRotate.position, range);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}