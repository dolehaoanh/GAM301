using UnityEngine;
using System.Collections.Generic;

public class TownCenter : MonoBehaviour
{
    // Danh sách toàn bộ Nhà Chính/Điểm nhận tài nguyên trong Scene
    public static List<TownCenter> AllTownCenters = new List<TownCenter>();

    public float deliverRange = 3.5f; // Khoảng cách nông dân cần tiếp cận để giao hàng
    public bool isEnemy = false; // Phân biệt Nhà Chính người chơi và địch

    [Header("Training Settings")]
    public GameObject farmerPrefab; // Prefab của Nông Dân
    public float trainingDuration = 5f; // Thời gian huấn luyện Nông Dân (5 giây)
    public int farmerCost = 50; // Giá mua nông dân (50 Vàng)
    public Transform spawnPoint; // Điểm xuất hiện của nông dân

    [System.NonSerialized]
    public bool isTraining = false;
    [System.NonSerialized]
    public float trainingTimer = 0f;

    private void OnEnable()
    {
        if (!AllTownCenters.Contains(this))
        {
            AllTownCenters.Add(this);
        }
    }

    private void OnDisable()
    {
        AllTownCenters.Remove(this);
    }

    // Tìm nhà chính gần nhất với nông dân
    public static TownCenter FindNearest(Vector3 position)
    {
        if (AllTownCenters.Count == 0) return null;

        TownCenter nearest = null;
        float minDistance = float.MaxValue;

        foreach (TownCenter tc in AllTownCenters)
        {
            if (tc == null) continue;
            float dist = Vector3.Distance(position, tc.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = tc;
            }
        }

        return nearest;
    }

    // Bắt đầu huấn luyện Nông Dân
    public bool StartTraining()
    {
        if (isTraining) return false;

        if (PlayerResourceManager.Instance == null) return false;

        // Kiểm tra giới hạn Lương thực (Food) trước khi huấn luyện (Nông dân tốn 1 Lương thực)
        int currentFood = PlayerResourceManager.Instance.GetCurrentFoodUsed();
        if (currentFood + 1 > PlayerResourceManager.Instance.maxFood)
        {
            Debug.LogWarning("[TownCenter] Không đủ giới hạn Lương thực (Food) để mua Nông Dân!");
            return false;
        }

        // Tiêu hao 50 Vàng của người chơi
        if (PlayerResourceManager.Instance.SpendResources(farmerCost, 0))
        {
            isTraining = true;
            trainingTimer = trainingDuration;
            Debug.Log($"[TownCenter] Bắt đầu huấn luyện Nông Dân! Chi phí: {farmerCost} Vàng.");
            return true;
        }
        else
        {
            Debug.LogWarning("[TownCenter] Không đủ Vàng hoặc PlayerResourceManager chưa sẵn sàng!");
            return false;
        }
    }

    private void Update()
    {
        if (isTraining)
        {
            trainingTimer -= Time.deltaTime;
            if (trainingTimer <= 0f)
            {
                isTraining = false;
                SpawnFarmer();
            }
        }
    }

    private void SpawnFarmer()
    {
        if (farmerPrefab == null)
        {
            // Tự động tải Farmer prefab từ thư mục Assets/ASM nếu chưa được gán trong Inspector
            #if UNITY_EDITOR
            farmerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Farmer.prefab");
            #endif
        }

        if (farmerPrefab != null)
        {
            // Vị trí xuất hiện: spawnPoint nếu có, nếu không thì đứng chếch về phía trước nhà chính
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * deliverRange;
            
            // Tìm vị trí hợp lệ trên NavMesh để tránh Farmer bị kẹt
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            GameObject farmerGo = Instantiate(farmerPrefab, spawnPos, Quaternion.identity);
            farmerGo.name = isEnemy ? $"Enemy_Farmer_{Random.Range(100, 999)}" : $"Farmer_Trained_{Random.Range(100, 999)}";
            
            // Đồng bộ faction isEnemy cho Nông dân
            RTSUnit unit = farmerGo.GetComponent<RTSUnit>();
            if (unit != null)
            {
                unit.isEnemy = this.isEnemy;
            }

            Debug.Log($"[TownCenter] Huấn luyện thành công Nông dân: {farmerGo.name} tại vị trí {spawnPos}!");
        }
        else
        {
            Debug.LogError("[TownCenter] Không thể huấn luyện vì không tìm thấy Farmer Prefab!");
        }
    }
}
