using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    [Tooltip("Kéo đối tượng Terrain vào")]
    public Terrain terrain;

    [Header("Base Dimensions")]
    [Tooltip("Chiều rộng bản đồ (Trục X)")]
    public int width = 512;
    [Tooltip("Chiều dài bản đồ (Trục Z)")]
    public int height = 512;
    [Tooltip("Chiều cao tối đa của đồi núi (Trục Y)")]
    public int depth = 50; 

    [Header("Noise Settings (FBM)")]
    [Tooltip("Độ thu phóng của nhiễu. Giá trị lớn = phẳng hơn, Giá trị nhỏ = nhấp nhô nhiều hơn")]
    public float scale = 20f; 
    
    [Tooltip("Số lượng lớp nhiễu xếp chồng lên nhau. Càng nhiều lớp thì địa hình càng chi tiết")]
    [Range(1, 8)]
    public int octaves = 4; 
    
    [Tooltip("Độ giảm biên độ (độ cao) của từng lớp phía sau. Thường từ 0 đến 1")]
    [Range(0, 1)]
    public float persistence = 0.5f; 
    
    [Tooltip("Độ tăng tần số (mức độ nhấp nhô) của từng lớp phía sau")]
    public float lacunarity = 2f; 

    [Tooltip("Dùng để tạo địa hình ngẫu nhiên khác nhau mỗi lần nhấn nút")]
    public Vector2 offset = Vector2.zero;

    [Header("Forest/Tree Settings")]
    [Tooltip("Bật/Tắt tính năng tự động trồng cây")]
    public bool spawnTrees = true;
    
    [Tooltip("Mức độ phân bố cây. Giá trị lớn tạo thành cụm rừng dày đặc")]
    public float treeNoiseScale = 15f; 
    
    [Tooltip("Ngưỡng mật độ cây. Chỉ những khu vực có giá trị nhiễu vượt ngưỡng này mới mọc cây")]
    [Range(0, 1)]
    public float treeDensityCutoff = 0.6f; 
    
    [Tooltip("Số lượng cây tối đa muốn thử nghiệm")]
    public int maxTrees = 1500;

    private void Start()
    {
        // Tự động tìm Terrain gắn cùng GameObject nếu chưa kéo thả thủ công
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }
        
        GenerateTerrain();
    }

    // ContextMenu để có thể click chuột phải vào Component trên Inspector và nhấn "Generate Terrain for ASM" để chạy ngay lập tức mà không cần bấm Play game!
    [ContextMenu("Generate Terrain for ASM")]
    public void GenerateTerrain()
    {
        if (terrain == null)
        {
            Debug.LogError("Vui lòng gán Terrain component trước khi chạy!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        
        // Thiết lập kích thước vật lý và độ phân giải bản đồ chiều cao cho Terrain
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        // Tính toán ma trận chiều cao 2D dựa trên thuật toán Perlin Noise đa tầng
        float[,] heights = CalculateHeights();
        
        // Áp dụng mảng chiều cao vào Terrain
        terrainData.SetHeights(0, 0, heights);

        // Nếu bật tự động tạo rừng
        if (spawnTrees)
        {
            GenerateForest(terrainData, heights);
        }
    }

    // Hàm duyệt qua từng pixel trên bản đồ để tính độ cao tương ứng
    private float[,] CalculateHeights()
    {
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = GetNoiseHeight(x, y);
            }
        }

        return heights;
    }

    // Hàm trung tâm: Áp dụng thuật toán Fractal Brownian Motion (FBM)
    private float GetNoiseHeight(int x, int y)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f; // Tổng biên độ tối đa để chuẩn hóa dữ liệu về khoảng [0, 1]

        for (int i = 0; i < octaves; i++)
        {
            // Tính toán tọa độ x, y dựa trên tần số và offset
            float xCoord = (float)x / width * scale * frequency + offset.x;
            float yCoord = (float)y / height * scale * frequency + offset.y;

            // Hàm Mathf.PerlinNoise của Unity luôn trả về giá trị từ 0.0 đến 1.0
            float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
            
            total += noiseValue * amplitude;

            maxValue += amplitude;
            
            // Cập nhật biên độ và tần số cho tầng kế tiếp (Octave sau sẽ nhỏ hơn nhưng nhanh hơn)
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue; // Trả về giá trị chuẩn hóa [0, 1]
    }

    // Hàm tự động trồng cây dựa trên độ cao và phân vùng mọc rừng
    private void GenerateForest(TerrainData terrainData, float[,] heights)
    {
        // Reset toàn bộ cây hiện có trên Terrain
        terrainData.treeInstances = new TreeInstance[0];

        // Rừng yêu cầu phải có Tree Prototypes (Các mẫu cây) đã khai báo sẵn trong Terrain Inspector
        if (terrainData.treePrototypes.Length == 0)
        {
            Debug.LogWarning("Chưa có cây nào được cấu hình trong mục Tree Prototypes của Terrain! Hãy thiết lập cây trên Editor trước.");
            return;
        }

        List<TreeInstance> treeList = new List<TreeInstance>();

        for (int i = 0; i < maxTrees; i++)
        {
            // Chọn ngẫu nhiên tọa độ chuẩn hóa từ 0.0 đến 1.0 trên bản đồ
            float xNorm = Random.value;
            float zNorm = Random.value;

            // Ánh xạ ngược lại chỉ số của ma trận chiều cao để kiểm tra độ cao tại điểm đó
            int xIdx = Mathf.Clamp((int)(xNorm * width), 0, width - 1);
            int yIdx = Mathf.Clamp((int)(zNorm * height), 0, height - 1);

            float currentHeight = heights[xIdx, yIdx];

            // 🌳 LOGIC PHÂN BỔ ĐỒNG BẰNG VÀ NÚI:
            // Chỉ trồng cây ở vùng thung lũng/đồng bằng và sườn đồi thấp (Tránh đỉnh núi cao dốc đứng và thung lũng sâu)
            if (currentHeight > 0.08f && currentHeight < 0.5f)
            {
                // Sử dụng thêm 1 tầng Perlin Noise phụ để nhóm cây lại thành từng cụm rừng tự nhiên
                // Tránh tình trạng cây mọc phân tán đơn lẻ đều nhau trông rất nhân tạo
                float forestNoise = Mathf.PerlinNoise(xNorm * treeNoiseScale, zNorm * treeNoiseScale);

                if (forestNoise > treeDensityCutoff)
                {
                    TreeInstance tree = new TreeInstance();
                    
                    // Trong Terrain, tọa độ TreeInstance là chuẩn hóa từ 0 đến 1
                    tree.position = new Vector3(xNorm, currentHeight, zNorm);
                    
                    // Chọn ngẫu nhiên một mẫu cây trong danh sách mẫu đã add vào Terrain
                    tree.prototypeIndex = Random.Range(0, terrainData.treePrototypes.Length);
                    
                    // Tạo kích thước cây hơi lệch nhau một chút để sinh động
                    tree.widthScale = Random.Range(0.8f, 1.3f);
                    tree.heightScale = Random.Range(0.8f, 1.3f);
                    
                    tree.color = Color.white;
                    tree.lightmapColor = Color.white;
                    
                    treeList.Add(tree);
                }
            }
        }

        // Đổ danh sách cây đã sinh vào dữ liệu của Terrain
        terrainData.treeInstances = treeList.ToArray();
        terrain.Flush(); // <-- Đã sửa thành Flush() đúng chuẩn của Unity    
        }
}