using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 1. Deduct 1 HP from the player via the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage();
            }

            // 2. Destroy the monster immediately so it doesn't trigger again
            Destroy(other.gameObject);
        }
    }
}