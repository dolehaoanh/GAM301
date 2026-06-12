using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance { get; private set; }

    [Header("Settings")]
    public GameObject barracksPrefab;
    public Material hologramMaterial;
    public int barracksCost = 10;

    [System.NonSerialized]
    public bool IsPlacing = false;

    private GameObject previewInstance;
    private int currentCost = 0;
    private GameObject currentPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartPlacement(GameObject prefab, int goldCost)
    {
        if (IsPlacing)
        {
            CancelPlacement();
        }

        if (prefab == null)
        {
            Debug.LogError("[BuildingPlacer] Cannot place building: Prefab is null.");
            return;
        }

        currentPrefab = prefab;
        currentCost = goldCost;
        IsPlacing = true;

        // Create the preview instance
        previewInstance = Instantiate(prefab);
        
        // Remove components that shouldn't run on the preview
        var barracksScript = previewInstance.GetComponent<Barracks>();
        if (barracksScript != null) Destroy(barracksScript);

        var obstacle = previewInstance.GetComponent<NavMeshObstacle>();
        if (obstacle != null) Destroy(obstacle);

        var rb = previewInstance.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        var colliders = previewInstance.GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
        {
            c.enabled = false; // Disable physics interaction for the preview
        }

        // Apply hologram shader / translucent material to renderers
        ApplyHologramMaterial(previewInstance);
    }

    private void ApplyHologramMaterial(GameObject target)
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r == null || r is LineRenderer) continue;

            if (hologramMaterial != null)
            {
                r.material = hologramMaterial;
            }
            else
            {
                // Fallback translucent color if no material is assigned
                Material fallbackMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                fallbackMat.color = new Color(0.2f, 1.0f, 0.2f, 0.4f);
                
                // Set to transparent blend mode in URP
                fallbackMat.SetFloat("_Surface", 1); // Transparent
                fallbackMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                fallbackMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                fallbackMat.SetInt("_ZWrite", 0);
                fallbackMat.DisableKeyword("_ALPHATEST_ON");
                fallbackMat.EnableKeyword("_ALPHABLEND_ON");
                fallbackMat.renderQueue = 3000;

                r.material = fallbackMat;
            }
        }
    }

    private void Update()
    {
        if (!IsPlacing || previewInstance == null) return;

        // Position preview at mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500f, LayerMask.GetMask("Default", "Terrain") | ~0))
        {
            previewInstance.transform.position = hit.point;
        }

        // Left-click to place
        if (Input.GetMouseButtonDown(0))
        {
            // Do not place if clicking UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TryPlaceBuilding();
        }

        // Right-click or Escape to cancel
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void TryPlaceBuilding()
    {
        if (PlayerResourceManager.Instance == null) return;

        if (PlayerResourceManager.Instance.gold < currentCost)
        {
            Debug.LogWarning($"[BuildingPlacer] Không đủ Vàng! Cần: {currentCost} Vàng, Hiện có: {PlayerResourceManager.Instance.gold}");
            return;
        }

        // Spend resources
        if (PlayerResourceManager.Instance.SpendResources(currentCost, 0))
        {
            // Spawn the real building
            Vector3 spawnPos = previewInstance.transform.position;
            
            // Align with NavMesh if possible
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            GameObject building = Instantiate(currentPrefab, spawnPos, Quaternion.identity);
            building.name = $"Barracks_Placed_{Random.Range(100, 999)}";

            var barracks = building.GetComponent<Barracks>();
            if (barracks == null)
            {
                barracks = building.AddComponent<Barracks>();
            }
            barracks.isEnemy = false;

            Debug.Log($"[BuildingPlacer] Đã đặt Nhà Binh (Barracks) tại {spawnPos} với giá {currentCost} Vàng.");
            
            EndPlacement();
        }
    }

    public void CancelPlacement()
    {
        Debug.Log("[BuildingPlacer] Đã hủy chế độ đặt công trình.");
        EndPlacement();
    }

    private void EndPlacement()
    {
        IsPlacing = false;
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }
        currentPrefab = null;
    }
}
