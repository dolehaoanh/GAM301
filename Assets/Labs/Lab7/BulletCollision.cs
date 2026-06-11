using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public GameObject explosionEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (explosionEffect != null)
            {
                GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(effect, 0.111f);
            }

            // Trả đạn về Pool nếu có ObjectPoolManager hoạt động (Lab 7),
            // ngược lại thì tự hủy (để tương thích ngược với Lab 5)
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
}