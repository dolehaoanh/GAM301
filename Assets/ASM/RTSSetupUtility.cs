#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RTSSetupUtility
{
    [MenuItem("RTS Game/Setup Buildings, Enemies & Trees")]
    public static void SetupScene()
    {
        // 1. Tìm TownCenter phe ta trong Scene (phải tìm theo tên để tránh nhầm với Enemy_TownCenter)
        GameObject playerTCGo = GameObject.Find("TownCenter");
        if (playerTCGo == null)
        {
            Debug.LogError("[RTS Setup] Không tìm thấy GameObject 'TownCenter' (phe ta) trong Scene!");
            return;
        }
        TownCenter playerTC = playerTCGo.GetComponent<TownCenter>();
        if (playerTC == null)
        {
            Debug.LogError("[RTS Setup] 'TownCenter' không có component TownCenter!");
            return;
        }

        // Tạo Undo Group để Ctrl+Z được
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("RTS Setup GameObjects");
        int undoGroup = Undo.GetCurrentGroup();

        // 1.5 Tự động dọn dẹp các vật thể cũ để đảm bảo cập nhật vị trí, góc xoay và màu sắc chuẩn nhất
        GameObject oldBarracks = GameObject.Find("Barracks_Player");
        if (oldBarracks != null) Undo.DestroyObjectImmediate(oldBarracks);

        GameObject oldEnemyTC = GameObject.Find("Enemy_TownCenter");
        if (oldEnemyTC != null) Undo.DestroyObjectImmediate(oldEnemyTC);

        GameObject oldEnemyBarracks = GameObject.Find("Enemy_Barracks");
        if (oldEnemyBarracks != null) Undo.DestroyObjectImmediate(oldEnemyBarracks);

        for (int i = 1; i <= 5; i++)
        {
            GameObject oldTree = GameObject.Find($"Special_Harvestable_Tree_{i}");
            if (oldTree != null) Undo.DestroyObjectImmediate(oldTree);
        }

        foreach (var go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go != null && (go.name.StartsWith("Enemy_Farmer_Init_") || go.name.StartsWith("Enemy_Soldier_Init_")))
            {
                Undo.DestroyObjectImmediate(go);
            }
        }

        // Áp dụng màu sắc cho Nhà Chính phe ta trong Editor
        SetFactionColorInEditor(playerTC.gameObject, false);

        // 2. Tạo Nhà Lính (Barracks) phe ta bằng cách nhân bản TownCenter
        GameObject barracksGo = GameObject.Find("Barracks_Player");
        if (barracksGo == null)
        {
            barracksGo = Object.Instantiate(playerTC.gameObject, playerTC.transform.position + new Vector3(15f, 0f, 0f), playerTC.transform.rotation);
            barracksGo.name = "Barracks_Player";
            
            // Xóa TownCenter component, thay bằng Barracks component
            var oldTC = barracksGo.GetComponent<TownCenter>();
            if (oldTC != null) Object.DestroyImmediate(oldTC);

            Barracks barracksComp = barracksGo.AddComponent<Barracks>();
            barracksComp.isEnemy = false;
            barracksComp.soldierCost = 80;
            barracksComp.trainingDuration = 6f;
            
            // Tìm và gán Soldier Prefab
            GameObject soldierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Soldier.prefab");
            if (soldierPrefab != null)
            {
                barracksComp.soldierPrefab = soldierPrefab;
            }

            // Gán spawnPoint
            var oldSpawnPoint = barracksGo.transform.Find("SpawnPoint");
            if (oldSpawnPoint != null)
            {
                barracksComp.spawnPoint = oldSpawnPoint;
            }

            // Nhuộm màu xanh lam nhạt cho Barracks phe ta trong Editor ngay lập tức
            SetFactionColorInEditor(barracksGo, false);

            Undo.RegisterCreatedObjectUndo(barracksGo, "Create Player Barracks");
            Debug.Log("[RTS Setup] Đã tạo thành công Nhà Lính (Barracks) phe ta!");
        }

        // Gán Farmer Prefab cho TownCenter nếu chưa gán
        if (playerTC.farmerPrefab == null)
        {
            GameObject farmerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Farmer.prefab");
            if (farmerPrefab != null)
            {
                playerTC.farmerPrefab = farmerPrefab;
                EditorUtility.SetDirty(playerTC);
            }
        }

        // 3. Tạo 5 cây gỗ đặc biệt (Special Harvestable Trees) gần Nông Dân
        // Tìm vị trí của các Farmer hiện tại trong Scene làm điểm tham chiếu
        Vector3 basePosition = playerTC.transform.position;
        RTSUnit[] allUnits = GameObject.FindObjectsByType<RTSUnit>(FindObjectsSortMode.None);
        foreach (var unit in allUnits)
        {
            if (unit != null && unit.unitType == RTSUnitType.Farmer && !unit.isEnemy)
            {
                basePosition = unit.transform.position;
                break;
            }
        }

        // Tải prefab cây gốc
        GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/ResourceTree1_Upright.prefab");
        Vector3[] treeOffsets = new Vector3[]
        {
            new Vector3(-6f, 0f, 6f),
            new Vector3(-9f, 0f, 2f),
            new Vector3(-5f, 0f, -6f),
            new Vector3(-2f, 0f, -9f),
            new Vector3(-8f, 0f, -3f)
        };

        for (int i = 0; i < treeOffsets.Length; i++)
        {
            string treeName = $"Special_Harvestable_Tree_{i + 1}";
            GameObject existingTree = GameObject.Find(treeName);
            if (existingTree != null) continue;

            Vector3 spawnPos = basePosition + treeOffsets[i];
            
            // Tìm cao độ mặt đất (Terrain)
            if (Terrain.activeTerrain != null)
            {
                spawnPos.y = Terrain.activeTerrain.SampleHeight(spawnPos) + Terrain.activeTerrain.transform.position.y;
            }

            GameObject treeGo;
            // Luôn tạo hình Cylinder làm mô hình đại diện cho cây gỗ khai thác đặc biệt
            treeGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            treeGo.name = treeName;

            // Thiết lập tỷ lệ phóng to 1.8 lần (cả 3 trục) để cây to lớn và nổi bật hẳn so với cây thường
            treeGo.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

            // Vì Pivot của Cylinder nằm ở giữa, ta tịnh tiến lên trên 1.8 đơn vị để đáy cây đứng trên mặt đất chuẩn xác
            treeGo.transform.position = spawnPos + Vector3.up * 1.8f;
            treeGo.transform.rotation = Quaternion.identity;

            // Đặt màu xanh lá rừng đẹp mắt cho cây
            var renderer = treeGo.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.12f, 0.42f, 0.16f); // Forest green
            }

            // Thêm/cấu hình CapsuleCollider của Cylinder để hoạt động click và khoanh chọn chuẩn xác
            CapsuleCollider col = treeGo.GetComponent<CapsuleCollider>();
            if (col == null)
            {
                col = treeGo.AddComponent<CapsuleCollider>();
            }
            col.center = Vector3.zero;
            col.radius = 0.5f;
            col.height = 2.0f;

            // Thêm ResourceNode component của Wood
            ResourceNode node = treeGo.GetComponent<ResourceNode>();
            if (node == null)
            {
                node = treeGo.AddComponent<ResourceNode>();
            }
            node.resourceType = RTSResourceType.Wood;
            node.remainingResources = 300;
            node.harvestRange = 2.5f;

            Undo.RegisterCreatedObjectUndo(treeGo, $"Create {treeName}");
        }
        Debug.Log("[RTS Setup] Đã tạo thành công 5 cây gỗ Đặc Biệt (Special Harvestable Trees) Cylinder 1.8x!");

        // 4. Tạo Căn Cứ Địch (Enemy Base) ở góc dưới bên phải bản đồ (X = 95f, Z = 35f) - Đường chéo đối diện nhà ta (X ≈ 34f, Z ≈ 88f)
        Vector3 enemyBasePos = new Vector3(95f, 0f, 35f);
        if (Terrain.activeTerrain != null)
        {
            enemyBasePos.y = Terrain.activeTerrain.SampleHeight(enemyBasePos) + Terrain.activeTerrain.transform.position.y;
        }

        GameObject enemyTCGo = GameObject.Find("Enemy_TownCenter");
        if (enemyTCGo == null)
        {
            // Nhân bản Nhà Chính của ta làm Nhà Chính địch, đồng thời sao chép góc xoay chuẩn xác (270f, 90f, 0f)
            enemyTCGo = Object.Instantiate(playerTC.gameObject, enemyBasePos, playerTC.transform.rotation);
            enemyTCGo.name = "Enemy_TownCenter";
            
            TownCenter enemyTC = enemyTCGo.GetComponent<TownCenter>();
            if (enemyTC != null)
            {
                enemyTC.isEnemy = true;
            }

            // Nhuộm đỏ pastel cho Nhà Chính phe địch trong Editor ngay lập tức
            SetFactionColorInEditor(enemyTCGo, true);

            // Tạo Nhà Lính địch cạnh Nhà Chính địch, sao chép góc xoay chuẩn xác
            GameObject enemyBarracksGo = Object.Instantiate(barracksGo, enemyBasePos + new Vector3(15f, 0f, 0f), playerTC.transform.rotation);
            enemyBarracksGo.name = "Enemy_Barracks";
            
            Barracks enemyB = enemyBarracksGo.GetComponent<Barracks>();
            if (enemyB != null)
            {
                enemyB.isEnemy = true;
            }

            // Nhuộm đỏ pastel cho Nhà Lính phe địch trong Editor ngay lập tức
            SetFactionColorInEditor(enemyBarracksGo, true);

            // Sinh sẵn một số lính và nông dân phe địch
            GameObject farmerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Farmer.prefab");
            GameObject soldierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Soldier.prefab");

            // Nông dân địch
            if (farmerPrefab != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 pos = enemyBasePos + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
                    GameObject enemyFarmer = Object.Instantiate(farmerPrefab, pos, Quaternion.identity);
                    enemyFarmer.name = $"Enemy_Farmer_Init_{i + 1}";
                    
                    RTSUnit unit = enemyFarmer.GetComponent<RTSUnit>();
                    if (unit != null)
                    {
                        unit.isEnemy = true;
                    }

                    // Áp dụng màu đỏ pastel ngay trong Editor
                    SetFactionColorInEditor(enemyFarmer, true);
                }
            }

            // Lính địch
            if (soldierPrefab != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 pos = enemyBasePos + new Vector3(Random.Range(5f, 15f), 0f, Random.Range(5f, 15f));
                    GameObject enemySoldier = Object.Instantiate(soldierPrefab, pos, Quaternion.identity);
                    enemySoldier.name = $"Enemy_Soldier_Init_{i + 1}";
                    
                    RTSUnit unit = enemySoldier.GetComponent<RTSUnit>();
                    if (unit != null)
                    {
                        unit.isEnemy = true;
                    }

                    // Áp dụng màu đỏ pastel ngay trong Editor
                    SetFactionColorInEditor(enemySoldier, true);
                }
            }

            Undo.RegisterCreatedObjectUndo(enemyTCGo, "Create Enemy Camp");
            Debug.Log("[RTS Setup] Đã tạo thành công Căn Cứ Địch (Enemy Base) gồm Nhà Chính, Nhà Lính và quân địch!");
        }

        // Refresh Editor
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("<color=lime><b>[RTS Setup] HOÀN THÀNH SETUP THÀNH CÔNG!</b></color> Hãy nhấn nút 'Play' trong Editor và kiểm tra game ngay!");
    }

    private static void SetFactionColorInEditor(GameObject go, bool isEnemy)
    {
        if (go == null) return;
        var renderers = go.GetComponentsInChildren<Renderer>();
        Color factionColor;
        if (isEnemy)
        {
            factionColor = new Color(1.0f, 0.6f, 0.6f, 1f); // Soft Red
        }
        else
        {
            RTSUnit unit = go.GetComponent<RTSUnit>();
            if (unit != null && unit.unitType == RTSUnitType.Farmer)
            {
                factionColor = new Color(0.6f, 0.9f, 0.7f, 1f); // Soft Green
            }
            else
            {
                factionColor = new Color(0.55f, 0.75f, 1.0f, 1f); // Soft Blue
            }
        }

        foreach (var r in renderers)
        {
            if (r == null || r is LineRenderer) continue;
            r.material.color = factionColor;
            EditorUtility.SetDirty(r);
        }
        EditorUtility.SetDirty(go);
    }
}
#endif
