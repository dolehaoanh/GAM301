using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Cài đặt Bộ tạo Quái")]
    public GameObject monsterPrefab; // Kéo MonsterPrefab của bạn vào đây
    public Transform endPortalTarget; // Kéo Cổng kết thúc (EndPortal) vào đây

    [Header("Cài đặt Sinh sản")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    void Start()
    {
        if (monsterPrefab == null || endPortalTarget == null)
        {
            Debug.LogError("Bộ tạo quái thiếu các trường thiết lập!");
            return;
        }

        // Bắt đầu vòng lặp sinh quái vô hạn
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 1. Tạo một quái vật mới tại vị trí của bộ tạo quái
            GameObject newMonster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);

            // 2. ⚠️ CẬP NHẬT: Tìm kiếm thành phần MonsterFSM thay vì EnemyAI!
            MonsterFSM aiScript = newMonster.GetComponent<MonsterFSM>();
            if (aiScript != null)
            {
                aiScript.InitializeDestination(endPortalTarget);
            }

            // 3. Chờ một khoảng thời gian ngẫu nhiên trước khi sinh quái vật tiếp theo
            float randomWait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomWait);
        }
    }
}