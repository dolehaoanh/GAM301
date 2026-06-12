#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RTSSetupUtility
{
    
    public static void SetupScene()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Assignment")
        {
            return;
        }

        
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

        
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("RTS Setup GameObjects");
        int undoGroup = Undo.GetCurrentGroup();

        
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

        
        GameObject barracksGo = GameObject.Find("Barracks_Player");
        if (barracksGo == null)
        {
            barracksGo = Object.Instantiate(playerTC.gameObject, playerTC.transform.position + new Vector3(15f, 0f, 0f), playerTC.transform.rotation);
            barracksGo.name = "Barracks_Player";
            
            
            var oldTC = barracksGo.GetComponent<TownCenter>();
            if (oldTC != null) Object.DestroyImmediate(oldTC);

            Undo.RegisterCreatedObjectUndo(barracksGo, "Create Player Barracks");
        }
        else
        {
            
            barracksGo.transform.rotation = playerTC.transform.rotation;
        }

        
        Barracks barracksComp = barracksGo.GetComponent<Barracks>();
        if (barracksComp == null)
        {
            barracksComp = barracksGo.AddComponent<Barracks>();
        }
        barracksComp.isEnemy = false;
        barracksComp.soldierCost = 80;
        barracksComp.trainingDuration = 6f;
        
        
        if (barracksComp.soldierPrefab == null)
        {
            GameObject sPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Soldier.prefab");
            if (sPrefab != null) barracksComp.soldierPrefab = sPrefab;
        }

        
        if (barracksComp.spawnPoint == null)
        {
            var oldSpawnPoint = barracksGo.transform.Find("SpawnPoint");
            if (oldSpawnPoint != null) barracksComp.spawnPoint = oldSpawnPoint;
        }

        
        if (playerTC.farmerPrefab == null)
        {
            GameObject fPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ASM/Farmer.prefab");
            if (fPrefab != null)
            {
                playerTC.farmerPrefab = fPrefab;
                EditorUtility.SetDirty(playerTC);
            }
        }

        
        string[] userTrees = new string[] { "TreeMine", "TreeMine (1)", "TreeMine (2)" };
        foreach (string treeName in userTrees)
        {
            GameObject treeGo = GameObject.Find(treeName);
            if (treeGo != null)
            {
                
                Collider col = treeGo.GetComponent<Collider>();
                if (col == null)
                {
                    CapsuleCollider capCol = treeGo.AddComponent<CapsuleCollider>();
                    capCol.center = new Vector3(0f, 1f, 0f);
                    capCol.radius = 0.5f;
                    capCol.height = 3.0f;
                }

                
                ResourceNode node = treeGo.GetComponent<ResourceNode>();
                if (node == null)
                {
                    node = treeGo.AddComponent<ResourceNode>();
                    Undo.RegisterCreatedObjectUndo(node, $"Configure {treeName} ResourceNode");
                }
                node.resourceType = RTSResourceType.Wood;
                node.remainingResources = 500; 
                node.harvestRange = 2.5f;
                
                EditorUtility.SetDirty(node);
            }
        }

        
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
            
            
            
        }

        TownCenter enemyTC = enemyTCGo.GetComponent<TownCenter>();
        if (enemyTC == null)
        {
            enemyTC = enemyTCGo.AddComponent<TownCenter>();
        }
        enemyTC.isEnemy = true;

        
        GameObject enemyBarracksGo = GameObject.Find("Enemy_Barracks");
        if (enemyBarracksGo == null)
        {
            enemyBarracksGo = Object.Instantiate(barracksGo, enemyBasePos + new Vector3(15f, 0f, 0f), playerTC.transform.rotation);
            enemyBarracksGo.name = "Enemy_Barracks";
            Undo.RegisterCreatedObjectUndo(enemyBarracksGo, "Create Enemy Barracks");
        }
        else
        {
            
            
            
        }

        Barracks enemyB = enemyBarracksGo.GetComponent<Barracks>();
        if (enemyB == null)
        {
            enemyB = enemyBarracksGo.AddComponent<Barracks>();
        }
        enemyB.isEnemy = true;

        

        
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

        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        
    }

    private static void SetFactionColorInEditor(GameObject go, bool isEnemy)
    {
        if (go == null) return;
        if (PrefabUtility.IsPartOfPrefabAsset(go)) return;

        var renderers = go.GetComponentsInChildren<Renderer>();
        Color factionColor;
        if (isEnemy)
        {
            factionColor = new Color(1.0f, 0.6f, 0.6f, 1f); 
        }
        else
        {
            RTSUnit unit = go.GetComponent<RTSUnit>();
            if (unit != null && unit.unitType == RTSUnitType.Farmer)
            {
                factionColor = new Color(0.6f, 0.9f, 0.7f, 1f); 
            }
            else
            {
                factionColor = new Color(0.55f, 0.75f, 1.0f, 1f); 
            }
        }

        foreach (var r in renderers)
        {
            if (r == null || r is LineRenderer || r.sharedMaterial == null) continue;
            
            
            string matName = r.sharedMaterial.name;
            bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") || 
                                 matName.Contains("MinimapIcon") || matName.Contains("Icon");
            
            Color targetColor = isMinimapIcon ? (isEnemy ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 0f, 1f)) : factionColor;

            
            if (!r.sharedMaterial.name.Contains("(Instance)"))
            {
                Material instantiatedMat = new Material(r.sharedMaterial);
                instantiatedMat.name = r.sharedMaterial.name + " (Instance)";
                r.sharedMaterial = instantiatedMat;
            }

            r.sharedMaterial.color = targetColor;
            EditorUtility.SetDirty(r);
        }
        EditorUtility.SetDirty(go);
    }

    [MenuItem("Công cụ thêm/Chuyển đổi cây địa hình thành cây thu hoạch được")]
    public static void ChuyenDoiCayDiaHinh()
    {
        Terrain banDo = null;
        GameObject mapGo = GameObject.Find("Map");
        if (mapGo != null)
        {
            banDo = mapGo.GetComponent<Terrain>();
        }

        if (banDo == null)
        {
            banDo = Terrain.activeTerrain;
        }

        if (banDo == null)
        {
            Debug.LogError("[Chuyển đổi Cây] Không tìm thấy Địa hình (Terrain) nào tên là 'Map' hoặc đang hoạt động trong Scene!");
            return;
        }

        TerrainData duLieuBanDo = banDo.terrainData;
        if (duLieuBanDo == null)
        {
            Debug.LogError("[Chuyển đổi Cây] Không tìm thấy dữ liệu địa hình (TerrainData)!");
            return;
        }

        TreeInstance[] danhSachCay = duLieuBanDo.treeInstances;
        TreePrototype[] danhSachMauCay = duLieuBanDo.treePrototypes;

        Vector3 viTriDiaHinh = banDo.transform.position;
        Vector3 kichThuocDiaHinh = duLieuBanDo.size;

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Chuyen Doi Cay Dia Hinh");

        
        GameObject nhomCha = GameObject.Find("Nhom_Cay_Thu_Hoach");
        if (nhomCha == null)
        {
            nhomCha = new GameObject("Nhom_Cay_Thu_Hoach");
            Undo.RegisterCreatedObjectUndo(nhomCha, "Tao Nhom Cay Cha");
        }

        int soLuongThanhCong = 0;

        for (int i = 0; i < danhSachCay.Length; i++)
        {
            TreeInstance cayHienTai = danhSachCay[i];

            
            Vector3 viTriCucBo = Vector3.Scale(cayHienTai.position, kichThuocDiaHinh);
            Vector3 viTriTheGioi = viTriDiaHinh + viTriCucBo;

            if (cayHienTai.prototypeIndex >= danhSachMauCay.Length) continue;
            GameObject mauCayGoc = danhSachMauCay[cayHienTai.prototypeIndex].prefab;
            if (mauCayGoc == null) continue;

            
            GameObject cayTuongTac = (GameObject)PrefabUtility.InstantiatePrefab(mauCayGoc);
            cayTuongTac.transform.SetParent(nhomCha.transform);
            cayTuongTac.transform.position = viTriTheGioi;

            
            Vector3 scaleGoc = mauCayGoc.transform.localScale;

            cayTuongTac.transform.localScale = new Vector3(
                cayHienTai.widthScale * scaleGoc.x,
                cayHienTai.heightScale * scaleGoc.y,
                cayHienTai.widthScale * scaleGoc.z
            );
            cayTuongTac.transform.rotation = Quaternion.AngleAxis(cayHienTai.rotation * Mathf.Rad2Deg, Vector3.up);
            cayTuongTac.name = $"{mauCayGoc.name}_ThuHoachDuoc_{i}";

            
            ResourceNode nguonTaiNguyen = cayTuongTac.GetComponent<ResourceNode>();
            if (nguonTaiNguyen == null)
            {
                nguonTaiNguyen = cayTuongTac.AddComponent<ResourceNode>();
            }
            nguonTaiNguyen.resourceType = RTSResourceType.Wood;
            nguonTaiNguyen.remainingResources = 500; 
            nguonTaiNguyen.harvestRange = 2.5f;

            
            Undo.RegisterCreatedObjectUndo(cayTuongTac, "Tao Cay Thu Hoach");
            soLuongThanhCong++;
        }

        
        Undo.RegisterCompleteObjectUndo(duLieuBanDo, "Xoa Cay Tren Terrain");
        duLieuBanDo.treeInstances = new TreeInstance[0];
        banDo.Flush();
    }

    [MenuItem("Công cụ thêm/Phóng to tất cả cây trên Terrain")]
    public static void PhongToCayTerrain()
    {
        Terrain banDo = null;
        GameObject mapGo = GameObject.Find("Map");
        if (mapGo != null)
        {
            banDo = mapGo.GetComponent<Terrain>();
        }

        if (banDo == null)
        {
            banDo = Terrain.activeTerrain;
        }

        if (banDo == null)
        {
            Debug.LogError("[Phóng to Cây] Không tìm thấy Địa hình (Terrain) nào tên là 'Map' hoặc đang hoạt động trong Scene!");
            return;
        }

        TerrainData duLieuBanDo = banDo.terrainData;
        if (duLieuBanDo == null) return;

        TreeInstance[] danhSachCay = duLieuBanDo.treeInstances;
        if (danhSachCay == null || danhSachCay.Length == 0)
        {
            Debug.LogWarning("[Phóng to Cây] Không có cây nào trên địa hình để phóng to!");
            return;
        }

        Undo.RegisterCompleteObjectUndo(duLieuBanDo, "Phong To Cay Terrain");

        
        float heSoNhan = 2.0f; 

        for (int i = 0; i < danhSachCay.Length; i++)
        {
            danhSachCay[i].widthScale *= heSoNhan;
            danhSachCay[i].heightScale *= heSoNhan;
        }

        duLieuBanDo.treeInstances = danhSachCay;
        banDo.Flush();
    }
}
#endif
