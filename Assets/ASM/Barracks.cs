using UnityEngine;
using System.Collections.Generic;

public class Barracks : MonoBehaviour
{
    public static List<Barracks> AllBarracks = new List<Barracks>();

    public bool isEnemy = false; // Phân biệt Nhà Lính của người chơi và địch

    [Header("Training Settings")]
    public GameObject soldierPrefab; // Prefab của Binh Sĩ
    public float trainingDuration = 6f; // Thời gian huấn luyện (6 giây)
    public int soldierCost = 80; // Giá mua binh sĩ (80 Vàng)
    public Transform spawnPoint; // Điểm xuất hiện của binh sĩ
    public float deliverRange = 3.5f;

    [System.NonSerialized]
    public bool isTraining = false;
    [System.NonSerialized]
    public float trainingTimer = 0f;

    private void OnEnable()
    {
        if (!AllBarracks.Contains(this))
        {
            AllBarracks.Add(this);
        }
    }

    private void OnDisable()
    {
        AllBarracks.Remove(this);
    }

    public bool StartTraining()
    {
        if (isTraining) return false;

        if (PlayerResourceManager.Instance == null) return false;

        // Kiểm tra giới hạn Lương thực (Food) trước khi huấn luyện (Chiến binh tốn 2 Lương thực)
        int currentFood = PlayerResourceManager.Instance.GetCurrentFoodUsed();
        if (currentFood + 2 > PlayerResourceManager.Instance.maxFood)
        {
            Debug.LogWarning("[Barracks] Không đủ giới hạn Lương thực (Food) để mua Chiến Binh!");
            return false;
        }

        // Tiêu hao 80 Vàng của người chơi
        if (PlayerResourceManager.Instance.SpendResources(soldierCost, 0))
        {
            isTraining = true;
            trainingTimer = trainingDuration;
            Debug.Log($"[Barracks] Bắt đầu huấn luyện Chiến Binh! Chi phí: {soldierCost} Vàng.");
            return true;
        }
        else
        {
            Debug.LogWarning("[Barracks] Không đủ Vàng hoặc PlayerResourceManager chưa sẵn sàng!");
            return false;
        }
    }

    private void Start()
    {
        ApplyFactionColors();
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
                SpawnSoldier();
            }
        }
    }

    private void SpawnSoldier()
    {
        if (soldierPrefab == null)
        {
            #if UNITY_EDITOR
            soldierPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Soldier.prefab");
            #endif
        }

        if (soldierPrefab != null)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * deliverRange;
            
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            GameObject soldierGo = Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
            soldierGo.name = isEnemy ? $"Enemy_Soldier_{Random.Range(100, 999)}" : $"Soldier_Trained_{Random.Range(100, 999)}";
            
            // Đồng bộ faction isEnemy cho Chiến binh
            RTSUnit unit = soldierGo.GetComponent<RTSUnit>();
            if (unit != null)
            {
                unit.isEnemy = this.isEnemy;
            }

            Debug.Log($"[Barracks] Huấn luyện thành công Chiến Binh: {soldierGo.name} tại vị trí {spawnPos}!");
        }
        else
        {
            Debug.LogError("[Barracks] Không thể huấn luyện vì không tìm thấy Soldier Prefab!");
        }
    }
}
