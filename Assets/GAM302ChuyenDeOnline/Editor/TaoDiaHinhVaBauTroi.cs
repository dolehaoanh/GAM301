using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.Rendering;

public class TaoDiaHinhVaBauTroi
{
    [MenuItem("Tools/Tao Dia Hinh Va Bau Troi")]
    public static void ThucHienTao()
    {
        Texture2D textureNen = TaoTextureCoDat();
        Material matNen = TaoMaterialNen(textureNen);
        Material matBauTroi = TaoMaterialBauTroi();
        ThietLapBauTroi(matBauTroi);

        Mesh meshDiaHinh = TaoMeshDiaHinh();
        CapNhatObjectPlane(meshDiaHinh, matNen);

        Material matThanCay = TaoMaterialThanCay();
        Material matLaCay = TaoMaterialLaCay();
        Mesh meshThanCay = TaoMeshThanCay();
        Mesh meshLaCay = TaoMeshLaCay();

        TaoRungCayRanhGioi(matThanCay, matLaCay, meshThanCay, meshLaCay);

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static Texture2D TaoTextureCoDat()
    {
        int kichThuoc = 512;
        Texture2D texture = new Texture2D(kichThuoc, kichThuoc, TextureFormat.RGBA32, true);
        Color mauCoBat = new Color(0.20f, 0.48f, 0.15f);
        Color mauCoSang = new Color(0.35f, 0.60f, 0.22f);
        Color mauDatBat = new Color(0.45f, 0.32f, 0.20f);
        Color mauDatToi = new Color(0.30f, 0.20f, 0.12f);

        for (int y = 0; y < kichThuoc; y++)
        {
            for (int x = 0; x < kichThuoc; x++)
            {
                float u = (float)x / kichThuoc;
                float v = (float)y / kichThuoc;
                float noiseNen = Mathf.PerlinNoise(u * 8f, v * 8f);
                float noiseChiTiet = Mathf.PerlinNoise(u * 25f, v * 25f);
                float noiseVao = Mathf.PerlinNoise(u * 50f, v * 50f);

                Color mauCo = Color.Lerp(mauCoBat, mauCoSang, noiseChiTiet);
                Color mauDat = Color.Lerp(mauDatBat, mauDatToi, noiseChiTiet);
                float heSoTron = Mathf.SmoothStep(0.25f, 0.75f, noiseNen + noiseChiTiet * 0.2f);

                Color mauCuoi = Color.Lerp(mauDat, mauCo, heSoTron) * (0.85f + noiseVao * 0.3f);
                mauCuoi.a = 1.0f;
                texture.SetPixel(x, y, mauCuoi);
            }
        }
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string duongDan = "Assets/GAM302ChuyenDeOnline/NenCoDat.png";
        File.WriteAllBytes(duongDan, bytes);
        AssetDatabase.ImportAsset(duongDan);

        TextureImporter importer = AssetImporter.GetAtPath(duongDan) as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(duongDan);
    }

    public static Material TaoMaterialNen(Texture2D texture)
    {
        string duongDanMat = "Assets/GAM302ChuyenDeOnline/Mat_NenCoDat.mat";
        Shader shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(duongDanMat);
        if (material == null)
        {
            material = new Material(shaderLit);
            AssetDatabase.CreateAsset(material, duongDanMat);
        }
        else
        {
            material.shader = shaderLit;
        }

        if (material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", texture);
            material.SetTextureScale("_BaseMap", new Vector2(40f, 40f));
        }
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", texture);
            material.SetTextureScale("_MainTex", new Vector2(40f, 40f));
        }
        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.1f);
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    public static Material TaoMaterialBauTroi()
    {
        string duongDanMat = "Assets/GAM302ChuyenDeOnline/Mat_BauTroi.mat";
        Shader shaderSky = Shader.Find("Skybox/Procedural");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(duongDanMat);
        if (material == null)
        {
            material = new Material(shaderSky);
            AssetDatabase.CreateAsset(material, duongDanMat);
        }
        else
        {
            material.shader = shaderSky;
        }

        material.SetFloat("_SunSize", 0.04f);
        material.SetFloat("_SunSizeConvergence", 10f);
        material.SetFloat("_AtmosphereThickness", 1.0f);
        material.SetColor("_SkyTint", new Color(0.42f, 0.65f, 0.95f, 1f));
        material.SetColor("_GroundColor", new Color(0.35f, 0.32f, 0.28f, 1f));
        material.SetFloat("_Exposure", 1.1f);

        EditorUtility.SetDirty(material);
        return material;
    }

    public static void ThietLapBauTroi(Material matBauTroi)
    {
        RenderSettings.skybox = matBauTroi;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        DynamicGI.UpdateEnvironment();
    }

    public static float TinhDoCaoDiaHinh(float x, float z)
    {
        float khoangCachTam = Mathf.Sqrt(x * x + z * z);
        float nLow = Mathf.PerlinNoise((x + 500f) * 0.02f, (z + 500f) * 0.02f) * 4.0f;
        float nMid = Mathf.PerlinNoise((x + 500f) * 0.07f, (z + 500f) * 0.07f) * 1.5f;
        float nHigh = Mathf.PerlinNoise((x + 500f) * 0.15f, (z + 500f) * 0.15f) * 0.5f;
        float yTrungTam = nLow + nMid + nHigh;

        float banKinhTrong = 65f;
        float banKinhNgoai = 95f;
        float heSoRanhGioi = Mathf.Clamp01((khoangCachTam - banKinhTrong) / (banKinhNgoai - banKinhTrong));
        float heSoRanhGioiSmooth = heSoRanhGioi * heSoRanhGioi * (3f - 2f * heSoRanhGioi);

        float nNui = Mathf.PerlinNoise((x + 200f) * 0.035f, (z + 200f) * 0.035f) * 35.0f + Mathf.PerlinNoise((x + 100f) * 0.09f, (z + 100f) * 0.09f) * 8.0f;
        return Mathf.Lerp(yTrungTam, 10.0f + nNui, heSoRanhGioiSmooth);
    }

    public static Mesh TaoMeshDiaHinh()
    {
        int phanDoan = 120;
        float kichThuoc = 200f;
        int soDiemAnh = phanDoan + 1;

        Vector3[] danhSachDiem = new Vector3[soDiemAnh * soDiemAnh];
        Vector2[] danhSachUV = new Vector2[soDiemAnh * soDiemAnh];
        int[] danhSachTamGiac = new int[phanDoan * phanDoan * 6];

        float nuaKichThuoc = kichThuoc / 2f;
        float buocNhat = kichThuoc / phanDoan;

        for (int j = 0; j < soDiemAnh; j++)
        {
            for (int i = 0; i < soDiemAnh; i++)
            {
                int chiSo = j * soDiemAnh + i;
                float x = -nuaKichThuoc + i * buocNhat;
                float z = -nuaKichThuoc + j * buocNhat;
                float y = TinhDoCaoDiaHinh(x, z);

                danhSachDiem[chiSo] = new Vector3(x, y, z);
                danhSachUV[chiSo] = new Vector2((float)i / phanDoan, (float)j / phanDoan);
            }
        }

        int chiSoTamGiac = 0;
        for (int j = 0; j < phanDoan; j++)
        {
            for (int i = 0; i < phanDoan; i++)
            {
                int diemTraiDuoi = j * soDiemAnh + i;
                int diemPhaiDuoi = diemTraiDuoi + 1;
                int diemTraiTren = (j + 1) * soDiemAnh + i;
                int diemPhaiTren = diemTraiTren + 1;

                danhSachTamGiac[chiSoTamGiac++] = diemTraiDuoi;
                danhSachTamGiac[chiSoTamGiac++] = diemTraiTren;
                danhSachTamGiac[chiSoTamGiac++] = diemPhaiDuoi;

                danhSachTamGiac[chiSoTamGiac++] = diemPhaiDuoi;
                danhSachTamGiac[chiSoTamGiac++] = diemTraiTren;
                danhSachTamGiac[chiSoTamGiac++] = diemPhaiTren;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "DiaHinhMesh";
        mesh.vertices = danhSachDiem;
        mesh.uv = danhSachUV;
        mesh.triangles = danhSachTamGiac;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        string duongDanMesh = "Assets/GAM302ChuyenDeOnline/DiaHinhMesh.asset";
        Mesh meshTonTai = AssetDatabase.LoadAssetAtPath<Mesh>(duongDanMesh);
        if (meshTonTai != null)
        {
            EditorUtility.CopySerialized(mesh, meshTonTai);
            return meshTonTai;
        }
        else
        {
            AssetDatabase.CreateAsset(mesh, duongDanMesh);
            return mesh;
        }
    }

    public static void CapNhatObjectPlane(Mesh meshDiaHinh, Material matNen)
    {
        GameObject planeObj = GameObject.Find("Plane");
        if (planeObj == null)
        {
            planeObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeObj.name = "Plane";
        }

        planeObj.transform.position = Vector3.zero;
        planeObj.transform.rotation = Quaternion.identity;
        planeObj.transform.localScale = Vector3.one;

        MeshFilter filter = planeObj.GetComponent<MeshFilter>();
        if (filter == null) filter = planeObj.AddComponent<MeshFilter>();
        filter.sharedMesh = meshDiaHinh;

        MeshRenderer renderer = planeObj.GetComponent<MeshRenderer>();
        if (renderer == null) renderer = planeObj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = matNen;

        MeshCollider collider = planeObj.GetComponent<MeshCollider>();
        if (collider == null) collider = planeObj.AddComponent<MeshCollider>();
        collider.sharedMesh = meshDiaHinh;
    }

    public static Material TaoMaterialThanCay()
    {
        string duongDan = "Assets/GAM302ChuyenDeOnline/Mat_ThanCay.mat";
        Shader shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(duongDan);
        if (material == null)
        {
            material = new Material(shaderLit);
            AssetDatabase.CreateAsset(material, duongDan);
        }
        else
        {
            material.shader = shaderLit;
        }

        Color mauThan = new Color(0.35f, 0.22f, 0.12f, 1f);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", mauThan);
        if (material.HasProperty("_Color")) material.SetColor("_Color", mauThan);

        EditorUtility.SetDirty(material);
        return material;
    }

    public static Material TaoMaterialLaCay()
    {
        string duongDan = "Assets/GAM302ChuyenDeOnline/Mat_LaCay.mat";
        Shader shaderLit = Shader.Find("Universal Render Pipeline/Lit");
        if (shaderLit == null) shaderLit = Shader.Find("Standard");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(duongDan);
        if (material == null)
        {
            material = new Material(shaderLit);
            AssetDatabase.CreateAsset(material, duongDan);
        }
        else
        {
            material.shader = shaderLit;
        }

        Color mauLa = new Color(0.12f, 0.42f, 0.16f, 1f);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", mauLa);
        if (material.HasProperty("_Color")) material.SetColor("_Color", mauLa);

        EditorUtility.SetDirty(material);
        return material;
    }

    public static Mesh TaoMeshThanCay()
    {
        string duongDan = "Assets/GAM302ChuyenDeOnline/MeshThanCay.asset";
        Mesh meshTonTai = AssetDatabase.LoadAssetAtPath<Mesh>(duongDan);
        if (meshTonTai != null) return meshTonTai;

        GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Mesh meshGoc = tempCylinder.GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = meshGoc.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x *= 0.5f;
            vertices[i].z *= 0.5f;
            vertices[i].y += 1.0f;
            vertices[i].y *= 1.5f;
        }

        Mesh meshMoi = new Mesh();
        meshMoi.name = "MeshThanCay";
        meshMoi.vertices = vertices;
        meshMoi.triangles = meshGoc.triangles;
        meshMoi.uv = meshGoc.uv;
        meshMoi.RecalculateNormals();
        meshMoi.RecalculateBounds();

        Object.DestroyImmediate(tempCylinder);
        AssetDatabase.CreateAsset(meshMoi, duongDan);
        return meshMoi;
    }

    public static Mesh TaoMeshLaCay()
    {
        string duongDan = "Assets/GAM302ChuyenDeOnline/MeshLaCay.asset";
        Mesh meshTonTai = AssetDatabase.LoadAssetAtPath<Mesh>(duongDan);
        if (meshTonTai != null) return meshTonTai;

        int phanDoan = 8;
        int soDiem = phanDoan + 2;
        Vector3[] vertices = new Vector3[soDiem];
        vertices[0] = new Vector3(0, 4.5f, 0);
        vertices[soDiem - 1] = new Vector3(0, 1.5f, 0);

        float banKinh = 2.0f;
        for (int i = 0; i < phanDoan; i++)
        {
            float goc = i * Mathf.PI * 2f / phanDoan;
            vertices[i + 1] = new Vector3(Mathf.Cos(goc) * banKinh, 2.0f, Mathf.Sin(goc) * banKinh);
        }

        int[] triangles = new int[phanDoan * 6];
        int idx = 0;
        for (int i = 0; i < phanDoan; i++)
        {
            int nxt = (i + 1) % phanDoan;
            triangles[idx++] = 0;
            triangles[idx++] = i + 1;
            triangles[idx++] = nxt + 1;

            triangles[idx++] = nxt + 1;
            triangles[idx++] = i + 1;
            triangles[idx++] = soDiem - 1;
        }

        Mesh meshMoi = new Mesh();
        meshMoi.name = "MeshLaCay";
        meshMoi.vertices = vertices;
        meshMoi.triangles = triangles;
        meshMoi.RecalculateNormals();
        meshMoi.RecalculateBounds();

        AssetDatabase.CreateAsset(meshMoi, duongDan);
        return meshMoi;
    }

    public static void TaoRungCayRanhGioi(Material matThan, Material matLa, Mesh meshThan, Mesh meshLa)
    {
        GameObject rungCayObj = GameObject.Find("RungCayRanhGioi");
        if (rungCayObj != null)
        {
            Object.DestroyImmediate(rungCayObj);
        }

        rungCayObj = new GameObject("RungCayRanhGioi");
        int soLuongCay = 120;
        Random.InitState(12345);

        for (int i = 0; i < soLuongCay; i++)
        {
            float goc = (float)i / soLuongCay * 360f * Mathf.Deg2Rad + Random.Range(-0.1f, 0.1f);
            float banKinh = Random.Range(68f, 88f);
            float posX = Mathf.Cos(goc) * banKinh;
            float posZ = Mathf.Sin(goc) * banKinh;
            float posY = TinhDoCaoDiaHinh(posX, posZ);

            GameObject cay = new GameObject("Cay_" + i);
            cay.transform.parent = rungCayObj.transform;
            cay.transform.position = new Vector3(posX, posY, posZ);
            float tyLe = Random.Range(0.85f, 1.4f);
            cay.transform.localScale = new Vector3(tyLe, tyLe, tyLe);
            cay.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject than = new GameObject("ThanCay");
            than.transform.parent = cay.transform;
            than.transform.localPosition = Vector3.zero;
            than.transform.localRotation = Quaternion.identity;
            MeshFilter filterThan = than.AddComponent<MeshFilter>();
            filterThan.sharedMesh = meshThan;
            MeshRenderer rendererThan = than.AddComponent<MeshRenderer>();
            rendererThan.sharedMaterial = matThan;

            GameObject la = new GameObject("LaCay");
            la.transform.parent = cay.transform;
            la.transform.localPosition = Vector3.zero;
            la.transform.localRotation = Quaternion.identity;
            MeshFilter filterLa = la.AddComponent<MeshFilter>();
            filterLa.sharedMesh = meshLa;
            MeshRenderer rendererLa = la.AddComponent<MeshRenderer>();
            rendererLa.sharedMaterial = matLa;
        }
    }
}
