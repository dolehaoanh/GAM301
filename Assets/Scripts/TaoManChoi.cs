using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Fusion;

public class TaoManChoi : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Tao Man Choi")]
    public static void TaoManChoiMenu()
    {
        TaoToanBoManChoi();
    }
#endif

    public static void TaoToanBoManChoi()
    {
#if UNITY_EDITOR
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null) urpShader = Shader.Find("URP/Lit");
        if (urpShader == null) urpShader = Shader.Find("Standard");

        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        System.Func<string, Color, float, Material> layHoacTaoMaterial = (tenMat, mau, doBong) =>
        {
            string duongDan = $"Assets/Materials/{tenMat}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(duongDan);
            if (mat == null)
            {
                mat = new Material(urpShader);
                AssetDatabase.CreateAsset(mat, duongDan);
            }
            mat.shader = urpShader;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", mau);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", mau);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", doBong);
            EditorUtility.SetDirty(mat);
            return mat;
        };

        Material matNenTim = layHoacTaoMaterial("Mat_NenTim", new Color(0.46f, 0.36f, 0.77f), 0.2f);
        Material matXanhLa = layHoacTaoMaterial("Mat_XanhLa", new Color(0.42f, 0.78f, 0.24f), 0.5f);
        Material matTrang = layHoacTaoMaterial("Mat_Trang", new Color(0.94f, 0.94f, 0.97f), 0.3f);
        Material matTimNhat = layHoacTaoMaterial("Mat_TimNhat", new Color(0.81f, 0.76f, 0.94f), 0.3f);
        Material matTuongDen = layHoacTaoMaterial("Mat_TuongDen", new Color(0.08f, 0.08f, 0.1f), 0.6f);
        Material matDo = layHoacTaoMaterial("Mat_Do", new Color(0.9f, 0.1f, 0.15f), 0.8f);
        Material matBiXanh = layHoacTaoMaterial("Mat_BiXanh", new Color(0.02f, 0.25f, 0.9f), 0.9f);
        Material matThanhDen = layHoacTaoMaterial("Mat_ThanhDen", new Color(0.05f, 0.05f, 0.07f), 0.5f);

        AssetDatabase.SaveAssets();

        string duongDanPrefab = "Assets/Prefabs/NguoiChoiPrefab.prefab";
        GameObject tempPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tempPlayer.name = "NguoiChoiPrefab";
        tempPlayer.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        tempPlayer.GetComponent<Renderer>().sharedMaterial = matDo;
        Rigidbody rb = tempPlayer.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        tempPlayer.AddComponent<NetworkObject>();
        tempPlayer.AddComponent<NetworkTransform>();
        tempPlayer.AddComponent<NguoiChoiMang>();

        GameObject prefabPlayer = PrefabUtility.SaveAsPrefabAsset(tempPlayer, duongDanPrefab);
        DestroyImmediate(tempPlayer);

        GameObject cameraObj = GameObject.FindWithTag("MainCamera");
        if (cameraObj == null)
        {
            cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        Camera cam = cameraObj.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.46f, 0.36f, 0.77f);
        cameraObj.transform.position = new Vector3(0f, 15.5f, -0.5f);
        cameraObj.transform.rotation = Quaternion.Euler(85f, 0f, 0f);

        GameObject lightObj = GameObject.Find("Directional Light");
        if (lightObj == null)
        {
            lightObj = new GameObject("Directional Light");
            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Directional;
        }
        Light directionalLight = lightObj.GetComponent<Light>();
        directionalLight.intensity = 1.3f;
        directionalLight.color = Color.white;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        GameObject root = GameObject.Find("ManChoiRoot");
        if (root != null) DestroyImmediate(root);
        root = new GameObject("ManChoiRoot");

        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "NenTim";
        background.transform.SetParent(root.transform);
        background.transform.position = new Vector3(0f, -0.2f, 0f);
        background.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        background.transform.localScale = new Vector3(40f, 30f, 1f);
        background.GetComponent<Renderer>().sharedMaterial = matNenTim;
        DestroyImmediate(background.GetComponent<Collider>());

        GameObject floorColliderObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorColliderObj.name = "MatSanPhungChi";
        floorColliderObj.transform.SetParent(root.transform);
        floorColliderObj.transform.position = new Vector3(0f, -0.1f, 0f);
        floorColliderObj.transform.localScale = new Vector3(20f, 0.2f, 10f);
        floorColliderObj.GetComponent<Renderer>().enabled = false;

        GameObject vungTrai = GameObject.CreatePrimitive(PrimitiveType.Cube);
        vungTrai.name = "VungAnToanTrai";
        vungTrai.transform.SetParent(root.transform);
        vungTrai.transform.position = new Vector3(-7.5f, 0f, 0.5f);
        vungTrai.transform.localScale = new Vector3(3f, 0.2f, 6f);
        vungTrai.GetComponent<Renderer>().sharedMaterial = matXanhLa;
        vungTrai.tag = "VungSafeTrai";
        vungTrai.GetComponent<BoxCollider>().isTrigger = true;

        GameObject vungPhai = GameObject.CreatePrimitive(PrimitiveType.Cube);
        vungPhai.name = "VungAnToanPhai";
        vungPhai.transform.SetParent(root.transform);
        vungPhai.transform.position = new Vector3(7.5f, 0f, -0.5f);
        vungPhai.transform.localScale = new Vector3(3f, 0.2f, 6f);
        vungPhai.GetComponent<Renderer>().sharedMaterial = matXanhLa;
        vungPhai.tag = "VungSafePhai";
        vungPhai.GetComponent<BoxCollider>().isTrigger = true;

        GameObject gridRoot = new GameObject("BanCaro");
        gridRoot.transform.SetParent(root.transform);

        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 5; row++)
            {
                float x = -3.5f + col;
                float z = -2.0f + row;
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"O_{col}_{row}";
                tile.transform.SetParent(gridRoot.transform);
                tile.transform.position = new Vector3(x, 0f, z);
                tile.transform.localScale = new Vector3(1f, 0.2f, 1f);
                bool laTrang = (col + row) % 2 == 0;
                tile.GetComponent<Renderer>().sharedMaterial = laTrang ? matTrang : matTimNhat;
                DestroyImmediate(tile.GetComponent<Collider>());
            }
        }

        GameObject oNoiTrai1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        oNoiTrai1.name = "ONoiTrai1";
        oNoiTrai1.transform.SetParent(gridRoot.transform);
        oNoiTrai1.transform.position = new Vector3(-4.5f, 0f, -2.0f);
        oNoiTrai1.transform.localScale = new Vector3(1f, 0.2f, 1f);
        oNoiTrai1.GetComponent<Renderer>().sharedMaterial = matTrang;
        DestroyImmediate(oNoiTrai1.GetComponent<Collider>());

        GameObject oNoiTrai2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        oNoiTrai2.name = "ONoiTrai2";
        oNoiTrai2.transform.SetParent(gridRoot.transform);
        oNoiTrai2.transform.position = new Vector3(-5.5f, 0f, -2.0f);
        oNoiTrai2.transform.localScale = new Vector3(1f, 0.2f, 1f);
        oNoiTrai2.GetComponent<Renderer>().sharedMaterial = matTimNhat;
        DestroyImmediate(oNoiTrai2.GetComponent<Collider>());

        GameObject oNoiPhai1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        oNoiPhai1.name = "ONoiPhai1";
        oNoiPhai1.transform.SetParent(gridRoot.transform);
        oNoiPhai1.transform.position = new Vector3(4.5f, 0f, 2.0f);
        oNoiPhai1.transform.localScale = new Vector3(1f, 0.2f, 1f);
        oNoiPhai1.GetComponent<Renderer>().sharedMaterial = matTimNhat;
        DestroyImmediate(oNoiPhai1.GetComponent<Collider>());

        GameObject oNoiPhai2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        oNoiPhai2.name = "ONoiPhai2";
        oNoiPhai2.transform.SetParent(gridRoot.transform);
        oNoiPhai2.transform.position = new Vector3(5.5f, 0f, 2.0f);
        oNoiPhai2.transform.localScale = new Vector3(1f, 0.2f, 1f);
        oNoiPhai2.GetComponent<Renderer>().sharedMaterial = matTrang;
        DestroyImmediate(oNoiPhai2.GetComponent<Collider>());

        GameObject tuongRoot = new GameObject("CacBucTuong");
        tuongRoot.transform.SetParent(root.transform);

        System.Action<Vector3, Vector3> taoBucTuong = (pos, scale) =>
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Tuong";
            wall.transform.SetParent(tuongRoot.transform);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().sharedMaterial = matTuongDen;
        };

        float height = 0.5f;
        float yPos = 0.25f;

        taoBucTuong(new Vector3(-9.1f, yPos, 0.5f), new Vector3(0.2f, height, 6.2f));
        taoBucTuong(new Vector3(-7.5f, yPos, 3.6f), new Vector3(3.4f, height, 0.2f));
        taoBucTuong(new Vector3(-5.9f, yPos, 1.1f), new Vector3(0.2f, height, 5.0f));
        taoBucTuong(new Vector3(-4.9f, yPos, -1.4f), new Vector3(2.2f, height, 0.2f));
        taoBucTuong(new Vector3(-3.9f, yPos, 0.6f), new Vector3(0.2f, height, 4.0f));
        taoBucTuong(new Vector3(2.6f, yPos, 2.6f), new Vector3(13.2f, height, 0.2f));
        taoBucTuong(new Vector3(-2.6f, yPos, -2.6f), new Vector3(13.2f, height, 0.2f));
        taoBucTuong(new Vector3(3.9f, yPos, -0.6f), new Vector3(0.2f, height, 4.0f));
        taoBucTuong(new Vector3(4.9f, yPos, 1.4f), new Vector3(2.2f, height, 0.2f));
        taoBucTuong(new Vector3(5.9f, yPos, -1.1f), new Vector3(0.2f, height, 5.0f));
        taoBucTuong(new Vector3(7.5f, yPos, -3.6f), new Vector3(3.4f, height, 0.2f));
        taoBucTuong(new Vector3(9.1f, yPos, -0.5f), new Vector3(0.2f, height, 6.2f));

        GameObject biRoot = new GameObject("ChuongNgaiVatBiXanh");
        biRoot.transform.SetParent(root.transform);

        float[] zPositions = new float[] { -2.0f, -1.0f, 0.0f, 1.0f, 2.0f };
        for (int i = 0; i < zPositions.Length; i++)
        {
            GameObject bi = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bi.name = $"BiXanh_{i}";
            bi.transform.SetParent(biRoot.transform);
            bi.transform.position = new Vector3(0f, 0.45f, zPositions[i]);
            bi.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            bi.GetComponent<Renderer>().sharedMaterial = matBiXanh;
            bi.tag = "BiXanh";
            bi.GetComponent<SphereCollider>().isTrigger = true;
            Rigidbody rbi = bi.AddComponent<Rigidbody>();
            rbi.isKinematic = true;
            rbi.useGravity = false;
            BiXanh bx = bi.AddComponent<BiXanh>();
            bx.vanToc = 5.5f;
            bx.gioihanTrai = -3.4f;
            bx.gioihanPhai = 3.4f;
            bx.diSangPhai = (i % 2 == 0);
        }

        GameObject quanLyObj = GameObject.Find("QuanLyTroChoi");
        if (quanLyObj == null) quanLyObj = new GameObject("QuanLyTroChoi");
        if (quanLyObj.GetComponent<NetworkObject>() == null) quanLyObj.AddComponent<NetworkObject>();
        QuanLyTroChoi quanLy = quanLyObj.GetComponent<QuanLyTroChoi>();
        if (quanLy == null) quanLy = quanLyObj.AddComponent<QuanLyTroChoi>();

        GameObject ketNoiObj = GameObject.Find("QuanLyKetNoiFusion");
        if (ketNoiObj == null) ketNoiObj = new GameObject("QuanLyKetNoiFusion");
        QuanLyKetNoiFusion ketNoi = ketNoiObj.GetComponent<QuanLyKetNoiFusion>();
        if (ketNoi == null) ketNoi = ketNoiObj.AddComponent<QuanLyKetNoiFusion>();
        if (prefabPlayer != null)
        {
            NetworkObject noPlayer = prefabPlayer.GetComponent<NetworkObject>();
            ketNoi.prefabNguoiChoi = noPlayer;
        }

        TaoGiaoDien(quanLy, matThanhDen);
#endif
    }

    private static void TaoGiaoDien(QuanLyTroChoi quanLy, Material matThanhDen)
    {
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null) DestroyImmediate(canvasObj);
        canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null)
        {
            eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        GameObject thanhTren = new GameObject("ThanhTren");
        thanhTren.transform.SetParent(canvasObj.transform, false);
        RectTransform rtTren = thanhTren.AddComponent<RectTransform>();
        rtTren.anchorMin = new Vector2(0, 1);
        rtTren.anchorMax = new Vector2(1, 1);
        rtTren.pivot = new Vector2(0.5f, 1);
        rtTren.sizeDelta = new Vector2(0, 70);
        rtTren.anchoredPosition = Vector2.zero;
        Image imgTren = thanhTren.AddComponent<Image>();
        imgTren.color = new Color(0.05f, 0.05f, 0.07f, 1f);

        GameObject textLevelObj = new GameObject("TextLevel");
        textLevelObj.transform.SetParent(thanhTren.transform, false);
        RectTransform rtLevel = textLevelObj.AddComponent<RectTransform>();
        rtLevel.anchorMin = new Vector2(0, 0.5f);
        rtLevel.anchorMax = new Vector2(0, 0.5f);
        rtLevel.pivot = new Vector2(0, 0.5f);
        rtLevel.anchoredPosition = new Vector2(30, 0);
        rtLevel.sizeDelta = new Vector2(400, 60);
        TMP_Text txtLevel = textLevelObj.AddComponent<TextMeshProUGUI>();
        txtLevel.text = "LEVEL: 1";
        txtLevel.fontSize = 38;
        txtLevel.fontStyle = FontStyles.Bold;
        txtLevel.color = Color.white;
        txtLevel.alignment = TextAlignmentOptions.Left;

        GameObject textSoLanObj = new GameObject("TextSoLan");
        textSoLanObj.transform.SetParent(thanhTren.transform, false);
        RectTransform rtSoLan = textSoLanObj.AddComponent<RectTransform>();
        rtSoLan.anchorMin = new Vector2(1, 0.5f);
        rtSoLan.anchorMax = new Vector2(1, 0.5f);
        rtSoLan.pivot = new Vector2(1, 0.5f);
        rtSoLan.anchoredPosition = new Vector2(-30, 0);
        rtSoLan.sizeDelta = new Vector2(400, 60);
        TMP_Text txtSoLan = textSoLanObj.AddComponent<TextMeshProUGUI>();
        txtSoLan.text = "SỐ LẦN: 0";
        txtSoLan.fontSize = 38;
        txtSoLan.fontStyle = FontStyles.Bold;
        txtSoLan.color = Color.white;
        txtSoLan.alignment = TextAlignmentOptions.Right;
        quanLy.textSoLan = txtSoLan;

        GameObject thanhDuoi = new GameObject("ThanhDuoi");
        thanhDuoi.transform.SetParent(canvasObj.transform, false);
        RectTransform rtDuoi = thanhDuoi.AddComponent<RectTransform>();
        rtDuoi.anchorMin = new Vector2(0, 0);
        rtDuoi.anchorMax = new Vector2(1, 0);
        rtDuoi.pivot = new Vector2(0.5f, 0);
        rtDuoi.sizeDelta = new Vector2(0, 70);
        rtDuoi.anchoredPosition = Vector2.zero;
        Image imgDuoi = thanhDuoi.AddComponent<Image>();
        imgDuoi.color = new Color(0.05f, 0.05f, 0.07f, 1f);

        GameObject btnMenuObj = new GameObject("BtnMenu");
        btnMenuObj.transform.SetParent(thanhDuoi.transform, false);
        RectTransform rtBtnMenu = btnMenuObj.AddComponent<RectTransform>();
        rtBtnMenu.anchorMin = new Vector2(0, 0.5f);
        rtBtnMenu.anchorMax = new Vector2(0, 0.5f);
        rtBtnMenu.pivot = new Vector2(0, 0.5f);
        rtBtnMenu.anchoredPosition = new Vector2(30, 0);
        rtBtnMenu.sizeDelta = new Vector2(300, 50);
        Button btnMenu = btnMenuObj.AddComponent<Button>();
        TMP_Text txtMenu = btnMenuObj.AddComponent<TextMeshProUGUI>();
        txtMenu.text = "DỪNG/MENU";
        txtMenu.fontSize = 32;
        txtMenu.fontStyle = FontStyles.Bold;
        txtMenu.color = Color.white;
        txtMenu.alignment = TextAlignmentOptions.Left;

        GameObject btnNhacObj = new GameObject("BtnNhac");
        btnNhacObj.transform.SetParent(thanhDuoi.transform, false);
        RectTransform rtBtnNhac = btnNhacObj.AddComponent<RectTransform>();
        rtBtnNhac.anchorMin = new Vector2(1, 0.5f);
        rtBtnNhac.anchorMax = new Vector2(1, 0.5f);
        rtBtnNhac.pivot = new Vector2(1, 0.5f);
        rtBtnNhac.anchoredPosition = new Vector2(-30, 0);
        rtBtnNhac.sizeDelta = new Vector2(300, 50);
        Button btnNhac = btnNhacObj.AddComponent<Button>();
        TMP_Text txtNhac = btnNhacObj.AddComponent<TextMeshProUGUI>();
        txtNhac.text = "BẬT NHẠC";
        txtNhac.fontSize = 32;
        txtNhac.fontStyle = FontStyles.Bold;
        txtNhac.color = Color.white;
        txtNhac.alignment = TextAlignmentOptions.Right;

        GameObject panelWinObj = new GameObject("PanelWin");
        panelWinObj.transform.SetParent(canvasObj.transform, false);
        RectTransform rtWin = panelWinObj.AddComponent<RectTransform>();
        rtWin.anchorMin = new Vector2(0.5f, 0.5f);
        rtWin.anchorMax = new Vector2(0.5f, 0.5f);
        rtWin.pivot = new Vector2(0.5f, 0.5f);
        rtWin.sizeDelta = new Vector2(600, 350);
        Image imgWin = panelWinObj.AddComponent<Image>();
        imgWin.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        GameObject textThangObj = new GameObject("TextThang");
        textThangObj.transform.SetParent(panelWinObj.transform, false);
        RectTransform rtTextThang = textThangObj.AddComponent<RectTransform>();
        rtTextThang.anchorMin = new Vector2(0.5f, 0.7f);
        rtTextThang.anchorMax = new Vector2(0.5f, 0.7f);
        rtTextThang.pivot = new Vector2(0.5f, 0.5f);
        rtTextThang.sizeDelta = new Vector2(500, 80);
        TMP_Text txtThang = textThangObj.AddComponent<TextMeshProUGUI>();
        txtThang.text = "BẠN ĐÃ THẮNG!";
        txtThang.fontSize = 48;
        txtThang.fontStyle = FontStyles.Bold;
        txtThang.color = new Color(0.2f, 0.9f, 0.3f);
        txtThang.alignment = TextAlignmentOptions.Center;

        GameObject btnChoiLaiObj = new GameObject("BtnChoiLai");
        btnChoiLaiObj.transform.SetParent(panelWinObj.transform, false);
        RectTransform rtBtnChoiLai = btnChoiLaiObj.AddComponent<RectTransform>();
        rtBtnChoiLai.anchorMin = new Vector2(0.5f, 0.3f);
        rtBtnChoiLai.anchorMax = new Vector2(0.5f, 0.3f);
        rtBtnChoiLai.pivot = new Vector2(0.5f, 0.5f);
        rtBtnChoiLai.sizeDelta = new Vector2(250, 60);
        Image imgBtnChoiLai = btnChoiLaiObj.AddComponent<Image>();
        imgBtnChoiLai.color = new Color(0.2f, 0.6f, 0.9f);
        Button btnChoiLai = btnChoiLaiObj.AddComponent<Button>();

        GameObject textChoiLaiObj = new GameObject("TextChoiLai");
        textChoiLaiObj.transform.SetParent(btnChoiLaiObj.transform, false);
        RectTransform rtTextChoiLai = textChoiLaiObj.AddComponent<RectTransform>();
        rtTextChoiLai.anchorMin = Vector2.zero;
        rtTextChoiLai.anchorMax = Vector2.one;
        rtTextChoiLai.sizeDelta = Vector2.zero;
        TMP_Text txtChoiLai = textChoiLaiObj.AddComponent<TextMeshProUGUI>();
        txtChoiLai.text = "CHƠI LẠI";
        txtChoiLai.fontSize = 32;
        txtChoiLai.fontStyle = FontStyles.Bold;
        txtChoiLai.color = Color.white;
        txtChoiLai.alignment = TextAlignmentOptions.Center;

        quanLy.panelWin = panelWinObj;
        btnChoiLai.onClick.AddListener(quanLy.ChoiLai);
        btnMenu.onClick.AddListener(quanLy.TamDungHoacMenu);
        btnNhac.onClick.AddListener(quanLy.BatTatNhac);

        panelWinObj.SetActive(false);
    }
}
