using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    public Terrain terrain;

    [Header("Base Dimensions")]
    public int width = 256;
    public int height = 256;
    public int depth = 20; 

    [Header("Noise Settings (FBM)")]
    public float scale = 20f; 
    [Range(1, 8)]
    public int octaves = 3; 
    [Range(0, 1)]
    public float persistence = 0.25f; 
    public float lacunarity = 2f; 
    
    [Tooltip("THAY ĐỔI SỐ NÀY để tạo bản đồ hoàn toàn mới!")]
    public Vector2 offset = Vector2.zero;

    [Header("RTS Multi-Basin Settings")]
    [Tooltip("Số lượng khu vực lòng chảo phẳng để xây dựng căn cứ")]
    [Range(2, 6)]
    public int basinsCount = 3;
    [Tooltip("Bán kính của từng lòng chảo phẳng")]
    public float basinRadius = 35f;
    [Tooltip("Độ dốc của vách núi bao quanh rìa bản đồ")]
    public float mountainSteepness = 3f;

    [Header("Auto Texturing (Splatmapping)")]
    public bool autoTexture = true;

    [Header("Forest/Tree Settings")]
    public bool spawnTrees = true;
    public float treeNoiseScale = 15f; 
    [Range(0, 1)]
    public float treeDensityCutoff = 0.4f; 
    public int maxTrees = 2000;

    
    private List<Vector2> activeBasinCenters = new List<Vector2>();

    private void Start()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }
    }

    [ContextMenu("Generate")]
    public void GenerateTerrain()
    {
        if (terrain == null)
        {
            Debug.LogError("Vui lòng gán Terrain component!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        
        Random.InitState((int)(offset.x * 1000f + offset.y)); 
        GenerateBasinCenters();

        
        float[,] heights = CalculateHeights();
        terrainData.SetHeights(0, 0, heights);

        
        if (autoTexture)
        {
            ApplyProceduralTextures(terrainData, heights);
        }

        
        if (spawnTrees)
        {
            GenerateForest(terrainData, heights);
        }
        else
        {
            terrainData.treeInstances = new TreeInstance[0];
        }
        
        terrain.Flush();
    }

    private void GenerateBasinCenters()
    {
        activeBasinCenters.Clear();
        
        float minPos = width * 0.25f;
        float maxPos = width * 0.75f;

        for (int i = 0; i < basinsCount; i++)
        {
            float rx = Random.Range(minPos, maxPos);
            float ry = Random.Range(minPos, maxPos);
            activeBasinCenters.Add(new Vector2(rx, ry));
        }
    }

    private float[,] CalculateHeights()
    {
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float baseNoise = GetNoiseHeight(x, y);

                
                float currentHeight = baseNoise * 0.25f;

                
                float nx = 2f * x / width - 1f;
                float ny = 2f * y / height - 1f;
                float distanceToCenter = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));

                float borderMask = Mathf.Clamp01((distanceToCenter - 0.75f) / 0.25f);
                borderMask = Mathf.Pow(borderMask, mountainSteepness);
                
                
                currentHeight = Mathf.Lerp(currentHeight, baseNoise * 0.6f + 0.12f, borderMask);

                
                
                if (borderMask < 0.1f) 
                {
                    float minBasinEffect = 1f; 
                    
                    foreach (Vector2 center in activeBasinCenters)
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), center);
                        if (dist < basinRadius)
                        {
                            float factor = dist / basinRadius;
                            
                            float smoothFactor = (Mathf.Sin(factor * Mathf.PI - Mathf.PI / 2f) + 1f) * 0.5f;
                            
                            if (smoothFactor < minBasinEffect)
                            {
                                minBasinEffect = smoothFactor;
                            }
                        }
                    }
                    
                    
                    
                    currentHeight = Mathf.Lerp(baseNoise * 0.02f, currentHeight, minBasinEffect);
                }

                heights[x, y] = currentHeight;
            }
        }

        return heights;
    }

    private float GetNoiseHeight(int x, int y)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float xCoord = (float)x / width * scale * frequency + offset.x;
            float yCoord = (float)y / height * scale * frequency + offset.y;

            float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
            
            total += noiseValue * amplitude;
            maxValue += amplitude;
            
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }

    private void ApplyProceduralTextures(TerrainData terrainData, float[,] heights)
    {
        int numLayers = terrainData.terrainLayers.Length;
        if (numLayers < 3)
        {
            Debug.LogWarning("Tự động sơn màu yêu cầu Terrain phải có ít nhất 3 layers.");
            return;
        }

        int mapWidth = terrainData.alphamapWidth;
        int mapHeight = terrainData.alphamapHeight;
        float[,,] splatmapData = new float[mapWidth, mapHeight, numLayers];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int hx = Mathf.FloorToInt((float)x / mapWidth * width);
                int hy = Mathf.FloorToInt((float)y / mapHeight * height);
                float h = heights[hx, hy];

                float[] weights = new float[numLayers];

                if (h < 0.05f) 
                {
                    weights[0] = 1f; 
                }
                else if (h < 0.12f) 
                {
                    float blend = (h - 0.05f) / (0.12f - 0.05f);
                    weights[0] = 1f - blend;
                    weights[1] = blend; 
                }
                else if (h < 0.22f) 
                {
                    float blend = (h - 0.12f) / (0.22f - 0.12f);
                    weights[1] = 1f - blend;
                    weights[2] = blend; 
                }
                else 
                {
                    if (numLayers >= 4)
                    {
                        float blend = Mathf.Clamp01((h - 0.22f) / (0.35f - 0.22f));
                        weights[2] = 1f - blend;
                        weights[3] = blend; 
                    }
                    else
                    {
                        weights[2] = 1f;
                    }
                }

                float z = 0f;
                for (int i = 0; i < numLayers; i++) z += weights[i];
                for (int i = 0; i < numLayers; i++) splatmapData[x, y, i] = weights[i] / z;
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private void GenerateForest(TerrainData terrainData, float[,] heights)
    {
        if (terrainData.treePrototypes.Length == 0)
        {
            Debug.LogWarning("Chưa cấu hình Tree Prototype!");
            return;
        }

        List<TreeInstance> treeList = new List<TreeInstance>();

        for (int i = 0; i < maxTrees; i++)
        {
            float xNorm = Random.value;
            float zNorm = Random.value;

            int xIdx = Mathf.Clamp((int)(xNorm * width), 0, width - 1);
            int yIdx = Mathf.Clamp((int)(zNorm * height), 0, height - 1);

            float currentHeight = heights[xIdx, yIdx];

            
            
            if (currentHeight > 0.06f && currentHeight < 0.18f)
            {
                float forestNoise = Mathf.PerlinNoise(xNorm * treeNoiseScale, zNorm * treeNoiseScale);

                if (forestNoise > treeDensityCutoff)
                {
                    TreeInstance tree = new TreeInstance();
                    tree.position = new Vector3(xNorm, currentHeight, zNorm);
                    tree.prototypeIndex = Random.Range(0, terrainData.treePrototypes.Length);
                    
                    tree.widthScale = Random.Range(1.5f, 2.5f);
                    tree.heightScale = Random.Range(1.5f, 2.5f);
                    tree.rotation = Random.Range(0f, Mathf.PI * 2f);
                    tree.color = Color.white;
                    tree.lightmapColor = Color.white;
                    
                    treeList.Add(tree);
                }
            }
        }

        terrainData.SetTreeInstances(treeList.ToArray(), true);
        Debug.Log($"[Terrain Generator] Đã sinh thành công {treeList.Count} cây thông lên các sườn đồi!");
    }
}