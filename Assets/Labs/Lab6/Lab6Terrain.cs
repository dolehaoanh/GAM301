using UnityEngine;

public class Lab6Terrain : MonoBehaviour
{
    [Header("Cấu hình Bài 1 (Dùng Add Perlin Noise)")]
    public float scaleBai1 = 20f;
    public float strengthBai1 = 0.1f;

    [Header("Cấu hình Bài 2 (Linear Gradient + Perlin)")]
    public float gradientStrength = 1.0f;
    public float scaleBai2 = 20f;
    public float strengthBai2 = 0.15f;

    // Nút kích hoạt Bài 1 trên Inspector
    [ContextMenu("Bài 1: Add Perlin Noise")]
    public void CallAddPerlinNoise()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            AddPerlinNoise(terrain, scaleBai1, strengthBai1);
        }
    }

    // Nút kích hoạt Bài 2 trên Inspector
    [ContextMenu("Bài 2: Set Linear Gradient with Perlin")]
    public void CallSetLinearGradientWithPerlin()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            SetLinearGradientWithPerlin(terrain, gradientStrength, scaleBai2, strengthBai2);
        }
    }

    // --- BÀI TẬP 1: THÊM PERLIN NOISE ---
    public void AddPerlinNoise(Terrain terrain, float scale = 20f, float strength = 0.1f)
    {
        int res = terrain.terrainData.heightmapResolution;
        float[,] heights = terrain.terrainData.GetHeights(0, 0, res, res);

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float xCoord = (float)x / res * scale;
                float yCoord = (float)y / res * scale;
                // Thêm noise, đảm bảo kết quả nằm trong khoảng [0,1]
                heights[x, y] = Mathf.Clamp01(heights[x, y] + Mathf.PerlinNoise(xCoord, yCoord) * strength);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    // --- BÀI TẬP 2: THIẾT LẬP ĐỘ DỐC VÀ PERLIN NOISE ---
    public void SetLinearGradientWithPerlin(Terrain terrain, float gradientStrength = 1.0f, float noiseScale = 20f, float noiseStrength = 0.15f)
    {
        int res = terrain.terrainData.heightmapResolution;
        float[,] heights = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                // Linear Gradient theo trục X
                float gradient = (float)x / (res - 1) * gradientStrength;
                // Thêm Perlin Noise cho tự nhiên
                float xCoord = (float)x / res * noiseScale;
                float yCoord = (float)y / res * noiseScale;
                float noise = Mathf.PerlinNoise(xCoord, yCoord) * noiseStrength;
                // Tổng hợp lại
                heights[x, y] = Mathf.Clamp01(gradient + noise);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
}