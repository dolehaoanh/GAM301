using UnityEngine;
using System.Collections.Generic;

public class TerrainSpawner : MonoBehaviour
{
    [Header("Cấu hình Spawner")]
    public GameObject[] prefabsToSpawn; // Danh sách Prefab cây hoặc vật phẩm
    public int spawnCount = 150; // Số lượng đối tượng cần sinh ra

    [Header("Cấu hình Bộ lọc Độ cao")]
    public float minHeight = 0f; // Độ cao tối thiểu để sinh vật thể
    public float maxHeight = 15f; // Độ cao tối đa để sinh vật thể (tránh đỉnh núi)

    [Header("Quản lý Bản sao (Tự động cập nhật)")]
    [SerializeField] private List<GameObject> spawnedObjects = new List<GameObject>();

    // Nút kích hoạt Sinh vật thể ngẫu nhiên trên Editor
    [ContextMenu("Spawn Random Objects")]
    public void SpawnRandomObjects()
    {
        ClearSpawnedObjects();

        Terrain terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Không tìm thấy component Terrain trên GameObject này!");
            return;
        }

        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogError("Vui lòng kéo thả ít nhất 1 Prefab vào mảng Prefabs To Spawn!");
            return;
        }

        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;
        Vector3 terrainPos = terrain.transform.position;

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = spawnCount * 5;

        while (spawned < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            // 1. Lấy tọa độ X, Z ngẫu nhiên theo tọa độ cục bộ (Local) của Terrain
            float localX = Random.Range(0f, terrainWidth);
            float localZ = Random.Range(0f, terrainLength);

            // 2. Chuyển sang tọa độ thế giới (World Space) để truyền vào SampleHeight
            float worldX = terrainPos.x + localX;
            float worldZ = terrainPos.z + localZ;

            // Lấy độ cao (Y) thực tế tại tọa độ thế giới đó
            float height = terrain.SampleHeight(new Vector3(worldX, 0f, worldZ));

            // 3. Kiểm tra xem độ cao có nằm trong khoảng cho phép không
            if (height >= minHeight && height <= maxHeight)
            {
                // Tọa độ thế giới chính xác (Y thế giới = Y cục bộ + Y gốc của Terrain)
                Vector3 spawnPosition = new Vector3(worldX, height + terrainPos.y, worldZ);

                GameObject selectedPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
                Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                GameObject newObj = Instantiate(selectedPrefab, spawnPosition, randomRotation, this.transform);

                spawnedObjects.Add(newObj);
                spawned++;
            }
        }

        Debug.Log($"Đã sinh thành công {spawned} đối tượng trên địa hình sau {attempts} lượt thử.");
    }

    // Nút dọn dẹp nhanh các vật thể đã sinh trên Editor
    [ContextMenu("Clear Spawned Objects")]
    public void ClearSpawnedObjects()
    {
        // Xóa sạch các đối tượng trong danh sách
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();

        // Tìm và xóa thêm các đối tượng con còn sót lại trong Hierarchy (nếu có)
        List<GameObject> childrenToDelete = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            childrenToDelete.Add(transform.GetChild(i).gameObject);
        }
        foreach (GameObject child in childrenToDelete)
        {
            DestroyImmediate(child);
        }

        Debug.Log("Đã dọn dẹp sạch toàn bộ các đối tượng đã sinh.");
    }
}