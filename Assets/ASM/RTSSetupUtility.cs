#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RTSSetupUtility
{
    [MenuItem("RTS Game/Setup Buildings, Enemies & Trees")]
    public static void SetupScene()
    {
        // 1. Tìm TownCenter phe ta trong Scene
        TownCenter playerTC = GameObject.FindAnyObjectByType<TownCenter>();
        if (playerTC == null)
        {
            Debug.LogError("[RTS Setup] Không tìm thấy TownCenter (phe ta) trong Scene!");
            return;
        }

        // Tạo Undo Group để Ctrl+Z được
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("RTS Setup GameObjects");
        int undoGroup = Undo.GetCurrentGroup();

        // 2. Tạo Nhà Lính (Barracks) phe ta bằng cách nhân bản TownCenter
        GameObject barracksGo = GameObject.Find("Barracks_Player");
        if (barracksGo == null)
        {
            barracksGo = Object.Instantiate(playerTC.gameObject, playerTC.transform.position + new Vector3(15f, 0f, 0f), Quaternion.identity);
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
            if (treePrefab != null)
            {
                treeGo = PrefabUtility.InstantiatePrefab(treePrefab) as GameObject;
                treeGo.transform.position = spawnPos;
                treeGo.transform.rotation = Quaternion.identity;
            }
            else
            {
                // Fallback: Tạo hình Cylinder nếu không có prefab
                treeGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                treeGo.transform.position = spawnPos + Vector3.up * 1.5f;
                treeGo.transform.localScale = new Vector3(0.5f, 1.5f, 0.5f);
                var renderer = treeGo.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.15f, 0.5f, 0.1f);
                }
            }

            treeGo.name = treeName;
            
            // Phóng to 1.8 lần để tạo nét độc đáo, nổi bật hẳn so với cây rừng thông thường!
            treeGo.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

            // Thêm CapsuleCollider nếu chưa có
            CapsuleCollider col = treeGo.GetComponent<CapsuleCollider>();
            if (col == null)
            {
                col = treeGo.AddComponent<CapsuleCollider>();
                col.center = new Vector3(0f, 1f, 0f);
                col.radius = 0.5f;
                col.height = 3.0f;
            }

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
        Debug.Log("[RTS Setup] Đã tạo thành công 5 cây gỗ Đặc Biệt (Special Harvestable Trees) 1.8x!");

        // 4. Tạo Căn Cứ Địch (Enemy Base) ở phía xa đối diện (X = 160f, Z = 160f)
        Vector3 enemyBasePos = new Vector3(160f, 0f, 160f);
        if (Terrain.activeTerrain != null)
        {
            enemyBasePos.y = Terrain.activeTerrain.SampleHeight(enemyBasePos) + Terrain.activeTerrain.transform.position.y;
        }

        GameObject enemyTCGo = GameObject.Find("Enemy_TownCenter");
        if (enemyTCGo == null)
        {
            enemyTCGo = Object.Instantiate(playerTC.gameObject, enemyBasePos, Quaternion.identity);
            enemyTCGo.name = "Enemy_TownCenter";
            
            TownCenter enemyTC = enemyTCGo.GetComponent<TownCenter>();
            if (enemyTC != null)
            {
                enemyTC.isEnemy = true;
            }

            // Tạo Nhà Lính địch cạnh Nhà Chính địch
            GameObject enemyBarracksGo = Object.Instantiate(barracksGo, enemyBasePos + new Vector3(15f, 0f, 0f), Quaternion.identity);
            enemyBarracksGo.name = "Enemy_Barracks";
            
            Barracks enemyB = enemyBarracksGo.GetComponent<Barracks>();
            if (enemyB != null)
            {
                enemyB.isEnemy = true;
            }

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

            Undo.RegisterCreatedObjectUndo(enemyTCGo, "Create Enemy Camp");
            Debug.Log("[RTS Setup] Đã tạo thành công Căn Cứ Địch (Enemy Base) gồm Nhà Chính, Nhà Lính và quân địch!");
        }

        // Refresh Editor
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("<color=lime><b>[RTS Setup] HOÀN THÀNH SETUP THÀNH CÔNG!</b></color> Hãy nhấn nút 'Play' trong Editor và kiểm tra game ngay!");
    }
}
#endif
