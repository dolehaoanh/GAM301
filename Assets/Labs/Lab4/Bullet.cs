using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    [Header("Visual Effects")]
    public GameObject hitEffectPrefab; // Drag your Spark Burst Prefab here!

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 1. Calculate the exact point on the monster's surface where the bullet hit
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            // 2. Spawn the visual effect at the hit point
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            }

            // 3. Deal 1 damage
            MonsterHP monsterHealth = other.GetComponent<MonsterHP>();
            if (monsterHealth != null)
            {
                monsterHealth.TakeDamage(1);
            }

            Destroy(gameObject); // Destroy the bullet
        }
    }
}