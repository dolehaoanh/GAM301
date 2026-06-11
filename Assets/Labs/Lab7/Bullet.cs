// THAY TOAN BO CODE CU BANG CODE MOI SU DUNG OBJECT POOLING CUA UNITY ENGINE (YC LAB 7)
// using UnityEngine;

// public class Bullet : MonoBehaviour
// {
//     public float speed = 20f;
//     public float lifeTime = 3f;

//     [Header("Hiệu ứng Hình ảnh")]
//     public GameObject hitEffectPrefab; // Kéo Prefab Tia lửa nổ vào đây!

//     void Start()
//     {
//         Destroy(gameObject, lifeTime);
//     }

//     void Update()
//     {
//         transform.Translate(Vector3.forward * speed * Time.deltaTime);
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Enemy"))
//         {
//             // 1. Tính toán điểm chính xác trên bề mặt quái vật mà đạn bắn trúng
//             Vector3 hitPoint = other.ClosestPoint(transform.position);

//             // 2. Tạo hiệu ứng hình ảnh tại điểm va chạm
//             if (hitEffectPrefab != null)
//             {
//                 Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
//             }

//             // 3. Gây 1 sát thương
//             MonsterHP monsterHealth = other.GetComponent<MonsterHP>();
//             if (monsterHealth != null)
//             {
//                 monsterHealth.TakeDamage(1);
//             }

//             Destroy(gameObject); // Hủy viên đạn
//         }
//     }
// }
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    private Coroutine deactivateCoroutine;

    void OnEnable()
    {
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime(lifeTime));
    }

    void OnDisable()
    {
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Chỉ xử lý gây sát thương
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonsterHP monsterHealth = other.GetComponent<MonsterHP>();
            if (monsterHealth != null)
            {
                monsterHealth.TakeDamage(1);
            }
            // Không gọi ReturnToPool hay Destroy ở đây nữa!
        }
    }

    IEnumerator DeactivateAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnBullet(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}