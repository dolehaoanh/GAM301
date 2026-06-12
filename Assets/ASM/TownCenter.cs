using UnityEngine;
using System.Collections.Generic;

public class TownCenter : MonoBehaviour
{
    // Danh sách toàn bộ Nhà Chính/Điểm nhận tài nguyên trong Scene
    public static List<TownCenter> AllTownCenters = new List<TownCenter>();

    public float deliverRange = 3.5f; // Khoảng cách nông dân cần tiếp cận để giao hàng
    public bool isEnemy = false; // Phân biệt Nhà Chính người chơi và địch

    [Header("Building Stats")]
    public float maxHP = 1000f;
    public float currentHP = 1000f;

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

    private void Start()
    {
        if (isEnemy)
        {
            maxHP = 165f;
            currentHP = 165f;
        }
        else
        {
            maxHP = 1000f;
            currentHP = 1000f;
        }
        ApplyFactionColors();
        var obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (obstacle == null)
        {
            obstacle = gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
        }
        if (obstacle != null)
        {
            obstacle.carving = true;
            var boxCol = GetComponent<BoxCollider>();
            if (boxCol != null)
            {
                obstacle.center = boxCol.center;
                obstacle.size = boxCol.size;
            }
        }
    }

    private void ApplyFactionColors()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r == null || r is LineRenderer) continue;
            Material mat = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject)) continue;
                if (r.sharedMaterial == null) continue;

                if (!r.sharedMaterial.name.Contains("(Instance)"))
                {
                    Material instantiatedMat = new Material(r.sharedMaterial);
                    instantiatedMat.name = r.sharedMaterial.name + " (Instance)";
                    r.sharedMaterial = instantiatedMat;
                }
                mat = r.sharedMaterial;
            }
            else
#endif
            {
                mat = r.material;
            }

            if (mat != null)
            {
                // Kiểm tra xem renderer này có phải là Quad hiển thị Icon trên Minimap không
                bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") ||
                                     mat.name.Contains("MinimapIcon") || mat.name.Contains("Icon");

                if (isMinimapIcon)
                {
                    mat.color = isEnemy ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 0f, 1f);
                }
                else
                {
                    mat.color = isEnemy ? new Color(1.0f, 0.6f, 0.6f, 1f) : new Color(0.55f, 0.75f, 1.0f, 1f);
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(r);
                }
#endif
            }
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
#endif
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyFactionColors();
        }
    }
#endif

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
#if UNITY_EDITOR
            farmerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Farmer.prefab");
#endif
        }

        if (farmerPrefab != null)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * deliverRange;

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            GameObject farmerGo;
            if (UnitPoolManager.Instance != null)
            {
                farmerGo = UnitPoolManager.Instance.SpawnFarmer(spawnPos, Quaternion.identity, this.isEnemy);
            }
            else
            {
                farmerGo = Instantiate(farmerPrefab, spawnPos, Quaternion.identity);
                RTSUnit unit = farmerGo.GetComponent<RTSUnit>();
                if (unit != null)
                {
                    unit.isEnemy = this.isEnemy;
                }
            }

            farmerGo.name = isEnemy ? $"Enemy_Farmer_{Random.Range(100, 999)}" : $"Farmer_Trained_{Random.Range(100, 999)}";

            // WARP AND MOVE COMMAND:
            UnityEngine.AI.NavMeshAgent agent = farmerGo.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(spawnPos); // Binds the agent to NavMesh instantly
                Vector3 exitTarget = spawnPos + transform.forward * 4.0f; // Walk 4 units forward
                agent.SetDestination(exitTarget);
            }

            Debug.Log($"[TownCenter] Huấn luyện thành công Nông dân: {farmerGo.name} tại vị trí {spawnPos}!");
        }
        else
        {
            Debug.LogError("[TownCenter] Không thể huấn luyện vì không tìm thấy Farmer Prefab!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHP <= 0f) return;
        currentHP -= damage;
        if (currentHP < 0f) currentHP = 0f;

        Debug.Log($"[TownCenter] {gameObject.name} (isEnemy: {isEnemy}) took {damage} damage. HP: {currentHP}/{maxHP}");

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[TownCenter] {gameObject.name} destroyed!");
        if (isEnemy)
        {
            RTSHUDController hud = FindAnyObjectByType<RTSHUDController>();
            if (hud != null)
            {
                hud.ShowVictoryScreen();
            }
        }
        Destroy(gameObject);
    }
}
