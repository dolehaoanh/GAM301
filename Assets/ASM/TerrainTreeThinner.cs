using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TerrainTreeThinner : MonoBehaviour
{
    [Header("Tree Thinning Settings")]
    [Range(0f, 1f)]
    [Tooltip("Tỷ lệ cây muốn giữ lại. Ví dụ: 0.25 = Giữ lại 25% số cây, xóa bỏ 75% cây để bớt lag.")]
    public float keepRatio = 0.25f;

    [Tooltip("Số Seed ngẫu nhiên để đảm bảo việc lọc cây luôn đồng nhất")]
    public int randomSeed = 42;

    [ContextMenu("XÓA BỚT CÂY (Giữ Nguyên Địa Hình)")]
    public void ThinTrees()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("[Tree Thinner] Không tìm thấy Terrain component trên GameObject này!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            Debug.LogError("[Tree Thinner] TerrainData của địa hình này bị rỗng!");
            return;
        }

        TreeInstance[] currentTrees = terrainData.treeInstances;
        if (currentTrees == null || currentTrees.Length == 0)
        {
            Debug.LogWarning("[Tree Thinner] Địa hình này hiện không có cây nào để lọc!");
            return;
        }

        // Tạo backup trong Undo để bạn có thể nhấn Ctrl+Z quay lại nếu muốn!
        #if UNITY_EDITOR
        UnityEditor.Undo.RegisterCompleteObjectUndo(terrainData, "Thin Terrain Trees");
        #endif

        List<TreeInstance> keptTrees = new List<TreeInstance>();
        Random.InitState(randomSeed);

        for (int i = 0; i < currentTrees.Length; i++)
        {
            if (Random.value < keepRatio)
            {
                keptTrees.Add(currentTrees[i]);
            }
        }

        // Áp dụng danh sách cây mới đã được lọc thưa đi
        terrainData.treeInstances = keptTrees.ToArray();
        terrain.Flush();

        Debug.Log($"<color=lime><b>[Tree Thinner] THÀNH CÔNG!</b></color> Đã lọc bớt cây từ {currentTrees.Length} xuống còn {keptTrees.Count} cây. Địa hình núi non và màu sơn cỏ đá của bạn được <b>GIỮ NGUYÊN 100%</b>!");
    }
}
