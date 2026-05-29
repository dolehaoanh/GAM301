using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RTSHUDController : MonoBehaviour
{
    [Header("Resources Text References")]
    [Tooltip("Text hiển thị lượng Vàng")]
    public TextMeshProUGUI goldText;
    [Tooltip("Text hiển thị lượng Gỗ")]
    public TextMeshProUGUI woodText;
    [Tooltip("Text hiển thị lượng Dân số")]
    public TextMeshProUGUI populationText;

    [Header("Selected Unit Panel References")]
    [Tooltip("Bảng Panel thông tin lính được chọn (Bottom-Center)")]
    public GameObject selectionPanel;
    [Tooltip("Ảnh chân dung của lính (Dùng RawImage để hỗ trợ Render Texture 3D)")]
    public UnityEngine.UI.RawImage selectedUnitPortrait;
    [Tooltip("Tên của lính")]
    public TextMeshProUGUI selectedUnitName;
    [Tooltip("Thanh máu của lính")]
    public Slider selectedUnitHPBar;

    [Header("Command Grid Panel References")]
    [Tooltip("Bảng Panel chứa các ô lệnh hành động (Bottom-Right)")]
    public GameObject commandPanel;
    [Tooltip("Danh sách các nút bấm lệnh (Move, Stop, Attack...)")]
    public Button[] commandButtons;

    [Header("3D Portrait Settings")]
    [Tooltip("Instance của Nông Dân tĩnh trong phòng Chân dung 3D")]
    public GameObject portraitFarmerInstance;
    [Tooltip("Instance của Binh Sĩ tĩnh trong phòng Chân dung 3D")]
    public GameObject portraitSoldierInstance;

    [Header("Dynamic Group Selection UI Settings")]
    [Tooltip("Khoảng cách giữa các khung chân dung lính nhóm")]
    public float groupPortraitSpacing = 110f;
    [Tooltip("Font game để gán cho các con số đếm nhóm")]
    public TMPro.TMP_FontAsset customGameFont;

    private System.Collections.Generic.List<GameObject> activeGroupPortraits = new System.Collections.Generic.List<GameObject>();

    // Bộ đệm lưu trữ vị trí/kích thước gốc của Name và HP Bar để khôi phục linh hoạt
    private Vector2 originalNamePos;
    private Vector2 originalNameSize;
    private Vector2 originalNameAnchorMin;
    private Vector2 originalNameAnchorMax;
    private Vector2 originalNamePivot;

    private Vector2 originalHPPos;
    private Vector2 originalHPSize;
    private Vector2 originalHPAnchorMin;
    private Vector2 originalHPAnchorMax;
    private Vector2 originalHPPivot;
    private TMPro.TextAlignmentOptions originalNameAlignment;

    // Dual 3D rendering cho chân dung lính nhóm động
    private RenderTexture farmerRT;
    private RenderTexture soldierRT;
    private Camera farmerCam;
    private Camera soldierCam;

    // Lưu trữ Render Texture 3D mặc định được gán từ Inspector
    private Texture originalPortraitTexture;

    private void Start()
    {
        // 1. Sao lưu vị trí/kích thước gốc của NameText và HPSlider để phục vụ chuyển đổi UI
        if (selectedUnitName != null)
        {
            var r = selectedUnitName.GetComponent<RectTransform>();
            originalNamePos = r.anchoredPosition;
            originalNameSize = r.sizeDelta;
            originalNameAnchorMin = r.anchorMin;
            originalNameAnchorMax = r.anchorMax;
            originalNamePivot = r.pivot;
            originalNameAlignment = selectedUnitName.alignment;
        }

        if (selectedUnitHPBar != null)
        {
            var r = selectedUnitHPBar.GetComponent<RectTransform>();
            originalHPPos = r.anchoredPosition;
            originalHPSize = r.sizeDelta;
            originalHPAnchorMin = r.anchorMin;
            originalHPAnchorMax = r.anchorMax;
            originalHPPivot = r.pivot;
        }

        // 2. Dọn dẹp các component không cần thiết ở phòng chân dung 3D để tránh lỗi NavMesh dưới lòng đất Y = -500
        CleanPortraitInstance(portraitFarmerInstance);
        CleanPortraitInstance(portraitSoldierInstance);

        // 3. Khởi tạo hệ thống camera chân dung động song song (Dual 3D Camera Setup)
        SetupDynamicPortraits();

        // 4. Tự động nạp Font game từ thư mục để hiển thị số lượng nhóm đồng bộ
        if (customGameFont == null)
        {
            #if UNITY_EDITOR
            customGameFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/ASM/Cinzel-VariableFont_wght SDF.asset");
            #endif
        }

        // Tự động khôi phục và gán Render Texture từ Portrait Camera nếu bị trống trong Inspector
        if (selectedUnitPortrait != null && selectedUnitPortrait.texture == null)
        {
            var portCam = GameObject.Find("PortraitCamera")?.GetComponent<Camera>();
            if (portCam != null && portCam.targetTexture != null)
            {
                selectedUnitPortrait.texture = portCam.targetTexture;
                Debug.Log($"[RTS Portrait] Tự động khôi phục và gán Render Texture từ camera: {portCam.name}!");
            }
        }

        if (selectedUnitPortrait != null)
        {
            originalPortraitTexture = selectedUnitPortrait.texture;
        }

        // Dọn dẹp các component không cần thiết ở phòng chân dung 3D để tránh lỗi NavMesh dưới lòng đất Y = -500
        CleanPortraitInstance(portraitFarmerInstance);
        CleanPortraitInstance(portraitSoldierInstance);

        // Tự động cập nhật dân số động và tài nguyên lặp đi lặp lại mỗi 0.5 giây (Tối ưu hóa hiệu năng, tránh giật lag)
        InvokeRepeating(nameof(UpdateDynamicPopulation), 0f, 0.5f);

        // Đăng ký lắng nghe sự kiện thay đổi tài nguyên
        PlayerResourceManager.OnResourcesChanged += UpdateHUDResources;
        
        HideSelectionPanel();
    }

    private void CleanPortraitInstance(GameObject go)
    {
        if (go == null) return;

        // Xóa RTSUnitAnimation trước vì nó phụ thuộc vào NavMeshAgent [RequireComponent]
        var anim = go.GetComponent<RTSUnitAnimation>();
        if (anim != null) Destroy(anim);

        // Xóa RTSUnit để tránh nhiễu logic
        var unit = go.GetComponent<RTSUnit>();
        if (unit != null) Destroy(unit);

        // Giờ mới xóa được NavMeshAgent vì không còn component phụ thuộc nào nữa
        var agent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) Destroy(agent);

        // Đóng băng Rigidbody nếu có
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private void SetupDynamicPortraits()
    {
        if (portraitFarmerInstance == null || portraitSoldierInstance == null) return;

        // Định vị Binh Sĩ đứng lệch sang bên phải 5 mét để không bị trùng lặp với Nông Dân
        portraitSoldierInstance.transform.localPosition = new Vector3(5f, 0f, 0f);

        // Giữ cả 2 đơn vị luôn ở trạng thái Active để duy trì vòng lặp hoạt ảnh Idle
        portraitFarmerInstance.SetActive(true);
        portraitSoldierInstance.SetActive(true);

        // Tìm camera chân dung gốc trong Scene
        var originalCamGo = GameObject.Find("PortraitCamera");
        if (originalCamGo == null) return;
        var originalCam = originalCamGo.GetComponent<Camera>();
        if (originalCam == null) return;

        // Tạo 2 Render Texture động cho Dân và Lính
        farmerRT = new RenderTexture(256, 256, 16);
        farmerRT.name = "DynamicFarmerRT";
        soldierRT = new RenderTexture(256, 256, 16);
        soldierRT.name = "DynamicSoldierRT";

        // Tắt camera gốc vì chúng ta sẽ nhân bản nó thành 2 camera độc lập
        originalCam.enabled = false;

        // Tạo Camera cho Nông dân (Farmer Camera)
        GameObject farmerCamGo = Instantiate(originalCamGo, originalCamGo.transform.parent);
        farmerCamGo.name = "FarmerPortraitCamera";
        farmerCam = farmerCamGo.GetComponent<Camera>();
        farmerCam.enabled = true;
        farmerCam.targetTexture = farmerRT;
        farmerCam.transform.localPosition = new Vector3(0f, 0.66f, 1.80f); // Nhìn vào Farmer ở (0,0,0)

        // Tạo Camera cho Binh sĩ (Soldier Camera)
        GameObject soldierCamGo = Instantiate(originalCamGo, originalCamGo.transform.parent);
        soldierCamGo.name = "SoldierPortraitCamera";
        soldierCam = soldierCamGo.GetComponent<Camera>();
        soldierCam.enabled = true;
        soldierCam.targetTexture = soldierRT;
        soldierCam.transform.localPosition = new Vector3(5f, 0.66f, 1.80f); // Nhìn vào Soldier ở (5,0,0)

        // Nhân bản thêm nguồn sáng để đảm bảo Lính cũng được chiếu sáng đầy đủ
        var originalLightGo = GameObject.Find("PortraitLight");
        if (originalLightGo != null)
        {
            GameObject soldierLightGo = Instantiate(originalLightGo, originalLightGo.transform.parent);
            soldierLightGo.name = "SoldierPortraitLight";
            soldierLightGo.transform.localPosition = new Vector3(5f, originalLightGo.transform.localPosition.y, originalLightGo.transform.localPosition.z);
        }

        // Tự động gán Render Texture mặc định của Farmer lên selectedUnitPortrait khi bắt đầu
        if (selectedUnitPortrait != null)
        {
            selectedUnitPortrait.texture = farmerRT;
            originalPortraitTexture = farmerRT;
        }
    }

    private void OnDestroy()
    {
        PlayerResourceManager.OnResourcesChanged -= UpdateHUDResources;
    }

    private void UpdateHUDResources()
    {
        if (PlayerResourceManager.Instance == null) return;

        // Đếm tổng số quân lính đang tồn tại thực tế trên bản đồ
        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude);
        int currentPop = 0;
        
        foreach (RTSUnit unit in allUnits)
        {
            if (unit != null && unit.transform.position.y > -100f)
            {
                currentPop++;
            }
        }
        
        int maxPop = PlayerResourceManager.Instance.maxPopulation;

        UpdateResourcesDisplay(
            PlayerResourceManager.Instance.gold, 
            PlayerResourceManager.Instance.wood, 
            currentPop, 
            maxPop
        );
    }

    private void UpdateDynamicPopulation()
    {
        UpdateHUDResources();
    }

    // Hàm cập nhật lượng tài nguyên hiển thị lên thanh TopBar
    public void UpdateResourcesDisplay(int gold, int wood, int currentPop, int maxPop)
    {
        if (goldText != null) goldText.text = $"GOLD: {gold}";
        if (woodText != null) woodText.text = $"WOOD: {wood}";
        if (populationText != null) populationText.text = $"POPULATION: {currentPop}/{maxPop}";
    }

    // Hàm hiển thị thông tin chi tiết khi chọn một hoặc nhiều quân lính
    public void ShowUnitSelection(System.Collections.Generic.List<RTSUnit> selectedList)
    {
        // 1. Dọn dẹp toàn bộ chân dung nhóm cũ trước khi xử lý
        foreach (var go in activeGroupPortraits)
        {
            if (go != null) Destroy(go);
        }
        activeGroupPortraits.Clear();

        if (selectedList == null || selectedList.Count == 0)
        {
            HideSelectionPanel();
            return;
        }

        if (selectionPanel != null) selectionPanel.SetActive(true);

        // Gom nhóm lính theo loại quân để đếm số lượng
        var typeGroups = new System.Collections.Generic.Dictionary<RTSUnitType, System.Collections.Generic.List<RTSUnit>>();
        foreach (var unit in selectedList)
        {
            if (unit == null) continue;
            if (!typeGroups.ContainsKey(unit.unitType))
            {
                typeGroups[unit.unitType] = new System.Collections.Generic.List<RTSUnit>();
            }
            typeGroups[unit.unitType].Add(unit);
        }

        // Quyết định chế độ hiển thị:
        // - Nếu chỉ chọn duy nhất 1 quân: Hiện chế độ 3D Portrait cận cảnh chi tiết (Classic Single Leader)
        // - Nếu chọn nhiều quân (nhiều hơn 1 đơn vị): Hiện chế độ thẻ chân dung nhóm song song (Multi-Group Portraits)
        bool isMultiSelection = selectedList.Count > 1;

        if (!isMultiSelection)
        {
            // --- CHẾ ĐỘ 1: SINGLE LEADER 3D PORTRAIT ---
            RTSUnit leader = selectedList[0];

            // Khôi phục lại vị trí và kích thước gốc của NameText và HPSlider trong Inspector
            if (selectedUnitName != null)
            {
                var r = selectedUnitName.GetComponent<RectTransform>();
                r.anchorMin = originalNameAnchorMin;
                r.anchorMax = originalNameAnchorMax;
                r.pivot = originalNamePivot;
                r.anchoredPosition = originalNamePos;
                r.sizeDelta = originalNameSize;
                selectedUnitName.alignment = originalNameAlignment;
            }

            if (selectedUnitHPBar != null)
            {
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                r.anchorMin = originalHPAnchorMin;
                r.anchorMax = originalHPAnchorMax;
                r.pivot = originalHPPivot;
                r.anchoredPosition = originalHPPos;
                r.sizeDelta = originalHPSize;
                selectedUnitHPBar.gameObject.SetActive(true);
            }

            // Bật lại các khung mặc định
            if (selectedUnitPortrait != null)
            {
                selectedUnitPortrait.gameObject.SetActive(true);
                if (leader.portrait != null)
                {
                    selectedUnitPortrait.texture = leader.portrait.texture;
                    selectedUnitPortrait.color = Color.white;
                }
                else
                {
                    // Lấy camera tương ứng của Farmer hoặc Soldier
                    RenderTexture targetRT = leader.unitType == RTSUnitType.Farmer ? farmerRT : soldierRT;
                    selectedUnitPortrait.texture = targetRT;
                    selectedUnitPortrait.color = targetRT != null ? Color.white : new Color(0.12f, 0.12f, 0.18f, 0.65f);
                }
            }

            var frame = selectionPanel.transform.Find("PortraitFrame");
            if (frame != null) frame.gameObject.SetActive(true);

            if (selectedUnitName != null) selectedUnitName.text = leader.unitName;

            if (selectedUnitHPBar != null)
            {
                selectedUnitHPBar.maxValue = leader.maxHP;
                selectedUnitHPBar.value = leader.currentHP;
            }
        }
        else
        {
            // --- CHẾ ĐỘ 2: MULTI-GROUP PORTRAITS (Đồng thời hiển thị chân dung 3D động của các nhóm lính) ---
            // Tắt các khung mặc định đơn lẻ đi
            if (selectedUnitPortrait != null) selectedUnitPortrait.gameObject.SetActive(false);
            
            var frame = selectionPanel.transform.Find("PortraitFrame");
            if (frame != null) frame.gameObject.SetActive(false);

            // Căn chỉnh NameText lên chính giữa phía trên (Top-Center)
            if (selectedUnitName != null)
            {
                var r = selectedUnitName.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0.5f, 1f);
                r.anchorMax = new Vector2(0.5f, 1f);
                r.pivot = new Vector2(0.5f, 1f); // Set to (0.5, 1) for perfect top-center alignment
                r.anchoredPosition = new Vector2(0f, -10f); // Exactly centered horizontally, 10px from the top
                r.sizeDelta = new Vector2(350f, 30f);
                selectedUnitName.alignment = TMPro.TextAlignmentOptions.Center; // Center text inside TMP

                // Yêu cầu: Hiển thị "ARMY (XX UNITS)" trong đó XX là tổng số quân lính đang chọn
                selectedUnitName.text = $"ARMY ({selectedList.Count} UNITS)";
            }

            // Căn chỉnh HP Slider xuống chính giữa phía dưới (Bottom-Center) để không bị đè và giữ nguyên tỉ lệ tròn gốc
            if (selectedUnitHPBar != null)
            {
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0.5f, 0f);
                r.anchorMax = new Vector2(0.5f, 0f);
                r.pivot = originalHPPivot; // Giữ nguyên pivot gốc để tránh biến dạng và lệch tâm UI
                r.anchoredPosition = new Vector2(0f, 18f); // Cách mép dưới 18px thoáng mát
                r.sizeDelta = new Vector2(250f, originalHPSize.y); // Giữ nguyên chiều cao gốc tránh méo bo tròn slider, kéo dài 250px cực sang trọng
                selectedUnitHPBar.gameObject.SetActive(true);
            }

            // Tính toán khoảng cách để xếp thẻ song song ở giữa SelectionPanel
            int groupCount = typeGroups.Count;
            float startX = -((groupCount - 1) * groupPortraitSpacing) / 2f;
            int index = 0;

            foreach (var kvp in typeGroups)
            {
                RTSUnitType type = kvp.Key;
                System.Collections.Generic.List<RTSUnit> list = kvp.Value;
                RTSUnit firstOfGroup = list[0];

                // 1. Tạo thẻ nhóm (Group Card)
                GameObject cardGo = new GameObject($"GroupCard_{type}");
                cardGo.transform.SetParent(selectionPanel.transform, false);
                activeGroupPortraits.Add(cardGo);

                var rectTrans = cardGo.AddComponent<RectTransform>();
                rectTrans.sizeDelta = new Vector2(75f, 75f);
                // Căn chỉnh nằm giữa và dịch chuyển sang hai bên
                rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
                rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
                rectTrans.pivot = new Vector2(0.5f, 0.5f);
                rectTrans.anchoredPosition = new Vector2(startX + index * groupPortraitSpacing, -8f); // Dịch nhẹ xuống dưới tạo khoảng cách thoáng mát với text name

                // 2. Tạo hình ảnh chân dung đại diện cho nhóm (ALWAYS use the live-animating 3D head portraits)
                GameObject imgGo = new GameObject("PortraitImage");
                imgGo.transform.SetParent(cardGo.transform, false);
                var imgRect = imgGo.AddComponent<RectTransform>();
                imgRect.anchorMin = Vector2.zero;
                imgRect.anchorMax = Vector2.one;
                imgRect.sizeDelta = Vector2.zero; // Stretch-Stretch full card

                var rawImg = imgGo.AddComponent<UnityEngine.UI.RawImage>();
                
                // ĐỈNH CAO: Tự động gán camera 3D Render Texture động cho mỏm mặt Dân/Lính thở đều!
                RenderTexture targetRT = type == RTSUnitType.Farmer ? farmerRT : soldierRT;
                rawImg.texture = targetRT;
                rawImg.color = targetRT != null ? Color.white : new Color(0.12f, 0.12f, 0.18f, 0.65f);

                // 3. Tạo Khung Viền Cho Thẻ Nhóm
                GameObject borderGo = new GameObject("Border");
                borderGo.transform.SetParent(cardGo.transform, false);
                var borderRect = borderGo.AddComponent<RectTransform>();
                borderRect.anchorMin = Vector2.zero;
                borderRect.anchorMax = Vector2.one;
                borderRect.sizeDelta = new Vector2(4f, 4f); // Viền dày rộng hơn 2px mỗi mép

                var borderImg = borderGo.AddComponent<UnityEngine.UI.Image>();
                borderImg.color = new Color(0.85f, 0.7f, 0.3f, 0.8f); // Viền vàng kim quý tộc
                borderImg.sprite = null;
                borderGo.transform.SetAsFirstSibling(); // Đặt đằng sau ảnh đại diện

                // 4. Tạo văn bản đếm số lượng nổi lên phía trên thẻ (Unit Counter Text above)
                GameObject countGo = new GameObject("CounterText");
                countGo.transform.SetParent(cardGo.transform, false);
                var countRect = countGo.AddComponent<RectTransform>();
                countRect.sizeDelta = new Vector2(80f, 30f);
                countRect.anchorMin = new Vector2(0.5f, 1f); // Căn ở giữa cạnh trên của card
                countRect.anchorMax = new Vector2(0.5f, 1f);
                countRect.pivot = new Vector2(0.5f, 0f); // Xoay quanh đáy chữ
                countRect.anchoredPosition = new Vector2(0f, 6f); // 6px cách đỉnh card

                var countText = countGo.AddComponent<TMPro.TextMeshProUGUI>();
                countText.text = $"{list.Count}"; // Ví dụ: 8 hoặc 4
                countText.alignment = TMPro.TextAlignmentOptions.Center;
                countText.fontSize = 22f;
                countText.color = new Color(1f, 0.85f, 0f); // Màu vàng gold
                
                // Gán font chữ, ưu tiên font game tùy chỉnh, fallback về font của unit name để tránh lỗi render TMP mesh
                if (customGameFont != null)
                {
                    countText.font = customGameFont;
                }
                else if (selectedUnitName != null)
                {
                    countText.font = selectedUnitName.font;
                }

                index++;
            }

            // Thanh HP Slider của cả nhóm
            if (selectedUnitHPBar != null)
            {
                float currentTotalHP = 0f;
                float maxTotalHP = 0f;
                foreach (var unit in selectedList)
                {
                    if (unit != null)
                    {
                        currentTotalHP += unit.currentHP;
                        maxTotalHP += unit.maxHP;
                    }
                }
                selectedUnitHPBar.maxValue = maxTotalHP;
                selectedUnitHPBar.value = currentTotalHP;
            }
        }
    }

    // Hàm tự động ẩn bảng thông tin đi khi người chơi nhấp ra ngoài đất trống (bỏ chọn)
    public void HideSelectionPanel()
    {
        // Dọn dẹp toàn bộ chân dung nhóm cũ khi ẩn bảng
        foreach (var go in activeGroupPortraits)
        {
            if (go != null) Destroy(go);
        }
        activeGroupPortraits.Clear();

        if (selectionPanel != null) selectionPanel.SetActive(false);
        
        // Trả cả 2 camera động về trạng thái tắt để tiết kiệm hiệu năng (chỉ bật khi có hiển thị)
        // Lưu ý: do camera ngầm được clone và gán target texture, chúng ta không cần tắt vật lý
    }
}
