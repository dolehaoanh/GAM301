#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RTSSetupUtility
{
    // [MenuItem("RTS Game/Setup Buildings, Enemies & Trees")]
    public static void SetupScene()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Assignment")
        {
            return;
        }

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
        if (PrefabUtility.IsPartOfPrefabAsset(go)) return;

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
            if (r == null || r is LineRenderer || r.sharedMaterial == null) continue;
            
            // Lấy tên vật liệu để kiểm tra minimap icon
            string matName = r.sharedMaterial.name;
            bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") || 
                                 matName.Contains("MinimapIcon") || matName.Contains("Icon");
            
            Color targetColor = isMinimapIcon ? (isEnemy ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 0f, 1f)) : factionColor;

            // Instantiate material in editor to avoid modifying shared assets and avoid prefab errors
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

    [MenuItem("RTS Game/Chuyển đổi cây địa hình thành cây thu hoạch được")]
    public static void ChuyenDoiCayDiaHinh()
    {
        Terrain banDo = Terrain.activeTerrain;
        if (banDo == null)
        {
            Debug.LogError("[Chuyển đổi Cây] Không tìm thấy Địa hình (Terrain) nào đang hoạt động trong Scene!");
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

        // Tạo GameObject cha để gom nhóm cây cho gọn Hierarchy
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

            // Tính toán vị trí thế giới của cây
            Vector3 viTriCucBo = Vector3.Scale(cayHienTai.position, kichThuocDiaHinh);
            Vector3 viTriTheGioi = viTriDiaHinh + viTriCucBo;

            if (cayHienTai.prototypeIndex >= danhSachMauCay.Length) continue;
            GameObject mauCayGoc = danhSachMauCay[cayHienTai.prototypeIndex].prefab;
            if (mauCayGoc == null) continue;

            // Nhân bản Prefab trong Scene và gán nhóm cha
            GameObject cayTuongTac = (GameObject)PrefabUtility.InstantiatePrefab(mauCayGoc);
            cayTuongTac.transform.SetParent(nhomCha.transform);
            cayTuongTac.transform.position = viTriTheGioi;

            // Lấy kích thước gốc của Prefab (ví dụ: 100, 100, 100) để nhân tỷ lệ chính xác
            Vector3 scaleGoc = mauCayGoc.transform.localScale;
            // Hệ số phóng to thêm theo ý muốn của người dùng (ví dụ: 1.5f để cây to và dễ click hơn)
            float heSoPhongToThem = 1.5f;

            cayTuongTac.transform.localScale = new Vector3(
                cayHienTai.widthScale * scaleGoc.x * heSoPhongToThem,
                cayHienTai.heightScale * scaleGoc.y * heSoPhongToThem,
                cayHienTai.widthScale * scaleGoc.z * heSoPhongToThem
            );
            cayTuongTac.transform.rotation = Quaternion.AngleAxis(cayHienTai.rotation * Mathf.Rad2Deg, Vector3.up);
            cayTuongTac.name = $"{mauCayGoc.name}_ThuHoachDuoc_{i}";

            // Đảm bảo cây có Collider để Click chọn và va chạm với kích thước thế giới chính xác
            Collider boVaCham = cayTuongTac.GetComponent<Collider>();
            if (boVaCham == null)
            {
                CapsuleCollider boVaChamHinhTru = cayTuongTac.AddComponent<CapsuleCollider>();
                // Chia tỷ lệ cục bộ cho kích thước gốc của Prefab để giữ kích thước thế giới chuẩn (Radius ~0.5m-1.5m, Height ~3m-9m)
                boVaChamHinhTru.center = new Vector3(0f, 1.0f / scaleGoc.y, 0f);
                boVaChamHinhTru.radius = 0.5f / scaleGoc.x;
                boVaChamHinhTru.height = 3.0f / scaleGoc.y;
            }

            // Đảm bảo cây có script ResourceNode để khai thác
            ResourceNode nguonTaiNguyen = cayTuongTac.GetComponent<ResourceNode>();
            if (nguonTaiNguyen == null)
            {
                nguonTaiNguyen = cayTuongTac.AddComponent<ResourceNode>();
            }
            nguonTaiNguyen.resourceType = RTSResourceType.Wood;
            nguonTaiNguyen.remainingResources = 500; // 500 Gỗ mỗi cây
            nguonTaiNguyen.harvestRange = 2.5f;

            // Cho phép Ctrl+Z để hoàn tác việc sinh GameObject
            Undo.RegisterCreatedObjectUndo(cayTuongTac, "Tao Cay Thu Hoach");
            soLuongThanhCong++;
        }

        // Xóa sạch các cây tĩnh trên Terrain để tránh bị trùng lặp cây
        Undo.RegisterCompleteObjectUndo(duLieuBanDo, "Xoa Cay Tren Terrain");
        duLieuBanDo.treeInstances = new TreeInstance[0];
        banDo.Flush();

        Debug.Log($"<color=lime><b>[Chuyển đổi Cây] THÀNH CÔNG!</b></color> Đã chuyển đổi {soLuongThanhCong} cây tĩnh trên địa hình thành các vật thể GameObjects (nằm trong thư mục Nhom_Cay_Thu_Hoach)!");
    }

    [MenuItem("RTS Game/Phóng to tất cả cây trên Terrain")]
    public static void PhongToCayTerrain()
    {
        Terrain banDo = Terrain.activeTerrain;
        if (banDo == null)
        {
            Debug.LogError("[Phóng to Cây] Không tìm thấy Địa hình (Terrain) nào đang hoạt động trong Scene!");
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

        // Hệ số phóng to (phóng to gấp 2 lần)
        float heSoNhan = 2.0f; 

        for (int i = 0; i < danhSachCay.Length; i++)
        {
            danhSachCay[i].widthScale *= heSoNhan;
            danhSachCay[i].heightScale *= heSoNhan;
        }

        duLieuBanDo.treeInstances = danhSachCay;
        banDo.Flush();

        Debug.Log($"<color=lime><b>[Phóng to Cây] THÀNH CÔNG!</b></color> Đã phóng to {danhSachCay.Length} cây trên địa hình lên gấp {heSoNhan} lần!");
    }
}
#endif
