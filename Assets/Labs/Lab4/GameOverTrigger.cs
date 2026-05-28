using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 1. Trừ 1 HP của người chơi thông qua GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage();
            }

            // 2. Hủy quái vật ngay lập tức để không kích hoạt lại
            Destroy(other.gameObject);
        }
    }
}