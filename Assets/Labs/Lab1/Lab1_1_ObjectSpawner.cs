using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Prefab của object Cube sẽ sinh
    public float spawnInterval = 2f; // chờ 2s giữa mỗi lần sinh
    public int maxSpawns = 10;        // spawn tối đa 10

    public Vector3 spawnRangeMin;    // Tọa độ XYZ min của khu vực sinh
    public Vector3 spawnRangeMax;    // Tọa max

    private int currentSpawnCount = 0; // đếm số cube hiện tại

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // lặp đến khi sinh đủ số lượng
        while (currentSpawnCount < maxSpawns)
        {
            // 1 - random vị trí
            float randomX = Random.Range(spawnRangeMin.x, spawnRangeMax.x);
            float randomY = Random.Range(spawnRangeMin.y, spawnRangeMax.y);
            float randomZ = Random.Range(spawnRangeMin.z, spawnRangeMax.z);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // 2 - sinh Cube tại toạ độ đã tính
            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);

            // 3 - tăng biến đếm
            currentSpawnCount++;

            // 4 - tạm dùng 2s rồi lặp tiếp
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
