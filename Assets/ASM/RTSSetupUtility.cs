#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RTSSetupUtility
{
    // [MenuItem("RTS Game/Setup Buildings, Enemies & Trees")]
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

        // 1.5 Tự động dọn dẹp các vật thể cũ (Nhưng GIỮ LẠI BARRACKS để bảo toàn mô hình mới của user!)
        for (int i = 1; i <= 5; i++)
        {
            GameObject oldTree = GameObject.Find($"Special_Harvestable_Tree_{i}");
            if (oldTree != null) Undo.DestroyObjectImmediate(oldTree);
        }

        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude))
        {
            if (go != null && (go.name.StartsWith("Enemy_Farmer_Init_") || go.name.StartsWith("Enemy_Soldier_Init_")))
            {
                Undo.DestroyObjectImmediate(go);
            }
        }

        // 2. Tạo hoặc Cập nhật Nhà Lính (Barracks) phe ta (Bảo toàn mô hình tùy chỉnh nếu đã có sẵn trong Scene!)
        GameObject barracksGo = GameObject.Find("Barracks_Player");
        if (barracksGo == null)
        {
            barracksGo = Object.Instantiate(playerTC.gameObject, playerTC.transform.position + new Vector3(15f, 0f, 0f), playerTC.transform.rotation);
            barracksGo.name = "Barracks_Player";
            
            // Xóa TownCenter component nếu nhân bản từ TownCenter
            var oldTC = barracksGo.GetComponent<TownCenter>();
            if (oldTC != null) Object.DestroyImmediate(oldTC);

            Undo.RegisterCreatedObjectUndo(barracksGo, "Create Player Barracks");
        }
        else
        {
            // Cập nhật góc xoay trùng khớp với TownCenter phe ta
            barracksGo.transform.rotation = playerTC.transform.rotation;
        }

        // Đảm bảo có component Barracks
        Barracks barracksComp = barracksGo.GetComponent<Barracks>();
        if (barracksComp == null)
        {
            barracksComp = barracksGo.AddComponent<Barracks>();
        }
        barracksComp.isEnemy = false;
        barracksComp.soldierCost = 80;
        barracksComp.trainingDuration = 6f;
        
        // Tìm và gán Soldier Prefab
        if (barracksComp.soldierPrefab == null)
        {
            GameObject sPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Soldier.prefab");
            if (sPrefab != null) barracksComp.soldierPrefab = sPrefab;
        }

        // Gán spawnPoint
        if (barracksComp.spawnPoint == null)
        {
            var oldSpawnPoint = barracksGo.transform.Find("SpawnPoint");
            if (oldSpawnPoint != null) barracksComp.spawnPoint = oldSpawnPoint;
        }

        // Gán Farmer Prefab cho TownCenter nếu chưa gán
        if (playerTC.farmerPrefab == null)
        {
            GameObject fPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Farmer.prefab");
            if (fPrefab != null)
            {
                playerTC.farmerPrefab = fPrefab;
                EditorUtility.SetDirty(playerTC);
            }
        }

        // 3. Cấu hình 3 cây có sẵn trong Scene (TreeMine, TreeMine (1), TreeMine (2)) để khai thác được
        string[] userTrees = new string[] { "TreeMine", "TreeMine (1)", "TreeMine (2)" };
        foreach (string treeName in userTrees)
        {
            GameObject treeGo = GameObject.Find(treeName);
            if (treeGo != null)
            {
                // Đảm bảo có Collider
                Collider col = treeGo.GetComponent<Collider>();
                if (col == null)
                {
                    CapsuleCollider capCol = treeGo.AddComponent<CapsuleCollider>();
                    capCol.center = new Vector3(0f, 1f, 0f);
                    capCol.radius = 0.5f;
                    capCol.height = 3.0f;
                }

                // Đảm bảo có ResourceNode
                ResourceNode node = treeGo.GetComponent<ResourceNode>();
                if (node == null)
                {
                    node = treeGo.AddComponent<ResourceNode>();
                    Undo.RegisterCreatedObjectUndo(node, $"Configure {treeName} ResourceNode");
                }
                node.resourceType = RTSResourceType.Wood;
                node.remainingResources = 500; // Tăng lên 500 tài nguyên
                node.harvestRange = 2.5f;
                
                EditorUtility.SetDirty(node);
            }
        }

        // 4. Tạo hoặc Cập nhật Căn Cứ Địch (Enemy Base) ở góc dưới bên phải bản đồ (X = 95f, Z = 35f)
        Vector3 enemyBasePos = new Vector3(95f, 0f, 35f);
        if (Terrain.activeTerrain != null)
        {
            enemyBasePos.y = Terrain.activeTerrain.SampleHeight(enemyBasePos) + Terrain.activeTerrain.transform.position.y;
        }

        GameObject enemyTCGo = GameObject.Find("Enemy_TownCenter");
        if (enemyTCGo == null)
        {
            enemyTCGo = Object.Instantiate(playerTC.gameObject, enemyBasePos, playerTC.transform.rotation);
            enemyTCGo.name = "Enemy_TownCenter";
            Undo.RegisterCreatedObjectUndo(enemyTCGo, "Create Enemy TownCenter");
        }
        else
        {
            enemyTCGo.transform.position = enemyBasePos;
            enemyTCGo.transform.rotation = playerTC.transform.rotation;
        }

        TownCenter enemyTC = enemyTCGo.GetComponent<TownCenter>();
        if (enemyTC == null)
        {
            enemyTC = enemyTCGo.AddComponent<TownCenter>();
        }
        enemyTC.isEnemy = true;

        // Tạo hoặc Cập nhật Nhà Lính địch cạnh Nhà Chính địch (Bảo toàn mô hình mới của user!)
        GameObject enemyBarracksGo = GameObject.Find("Enemy_Barracks");
        if (enemyBarracksGo == null)
        {
            enemyBarracksGo = Object.Instantiate(barracksGo, enemyBasePos + new Vector3(15f, 0f, 0f), playerTC.transform.rotation);
            enemyBarracksGo.name = "Enemy_Barracks";
            Undo.RegisterCreatedObjectUndo(enemyBarracksGo, "Create Enemy Barracks");
        }
        else
        {
            enemyBarracksGo.transform.position = enemyBasePos + new Vector3(15f, 0f, 0f);
            enemyBarracksGo.transform.rotation = playerTC.transform.rotation;
        }

        Barracks enemyB = enemyBarracksGo.GetComponent<Barracks>();
        if (enemyB == null)
        {
            enemyB = enemyBarracksGo.AddComponent<Barracks>();
        }
        enemyB.isEnemy = true;

        /*
        // Sinh sẵn một số lính và nông dân phe địch (Tạm thời vô hiệu hóa theo yêu cầu của user)
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
            }
        }
        */

        // 5. Cập nhật màu sắc cho TẤT CẢ các Unit và Building hiện có trong Scene trong Editor
        foreach (var tc in Object.FindObjectsByType<TownCenter>(FindObjectsInactive.Exclude))
        {
            if (tc != null) SetFactionColorInEditor(tc.gameObject, tc.isEnemy);
        }
        foreach (var b in Object.FindObjectsByType<Barracks>(FindObjectsInactive.Exclude))
        {
            if (b != null) SetFactionColorInEditor(b.gameObject, b.isEnemy);
        }
        foreach (var unit in Object.FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude))
        {
            if (unit != null) SetFactionColorInEditor(unit.gameObject, unit.isEnemy);
        }

        // Refresh Editor
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Debug.Log("<color=lime><b>[RTS Setup] HOÀN THÀNH SETUP THÀNH CÔNG!</b></color> Hãy nhấn nút 'Play' trong Editor và kiểm tra game ngay!");
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
            
            // Lấy tên vật liệu để kiểm tra minimap icon
            string matName = r.sharedMaterial != null ? r.sharedMaterial.name : "";
            bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") || 
                                 matName.Contains("MinimapIcon") || matName.Contains("Icon");
            
            if (isMinimapIcon)
            {
                r.material.color = isEnemy ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 0f, 1f);
            }
            else
            {
                r.material.color = factionColor;
            }
            EditorUtility.SetDirty(r);
        }
        EditorUtility.SetDirty(go);
    }
}
#endif
