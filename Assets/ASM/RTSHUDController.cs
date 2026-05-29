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

    // Các thành phần UI và tham chiếu bổ sung cho huấn luyện quân
    private UnityEngine.UI.Image singlePortraitBackground;
    private Sprite portraitFrameSprite;
    private Button trainFarmerButton;
    private UnityEngine.UI.Image trainFarmerCooldownOverlay;
    private TMPro.TextMeshProUGUI trainFarmerCooldownText;
    private TownCenter activeSelectedTownCenter;

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

        // Tạo nền vàng kim quý tộc cho chân dung đơn lẻ (Single Portrait Background)
        if (selectedUnitPortrait != null)
        {
            GameObject bgGo = new GameObject("SinglePortraitBackground");
            bgGo.transform.SetParent(selectedUnitPortrait.transform.parent, false);
            bgGo.transform.SetSiblingIndex(selectedUnitPortrait.transform.GetSiblingIndex()); // Đặt trực tiếp phía sau RawImage chân dung

            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = selectedUnitPortrait.rectTransform.anchorMin;
            bgRect.anchorMax = selectedUnitPortrait.rectTransform.anchorMax;
            bgRect.pivot = selectedUnitPortrait.rectTransform.pivot;
            bgRect.anchoredPosition = selectedUnitPortrait.rectTransform.anchoredPosition;
            bgRect.sizeDelta = selectedUnitPortrait.rectTransform.sizeDelta;

            singlePortraitBackground = bgGo.AddComponent<UnityEngine.UI.Image>();
            singlePortraitBackground.color = new Color(0.85f, 0.7f, 0.3f, 0.8f); // Màu vàng kim
            singlePortraitBackground.gameObject.SetActive(false);
        }

        // Khởi tạo nút mua Nông dân của Nhà Chính
        CreateTrainFarmerButton();

        // Lưu trữ sprite khung viền vàng để nhân bản cho thẻ nhóm
        var frameGo = selectionPanel.transform.Find("PortraitFrame");
        if (frameGo != null)
        {
            var frameImg = frameGo.GetComponent<UnityEngine.UI.Image>();
            if (frameImg != null)
            {
                portraitFrameSprite = frameImg.sprite;
            }
        }
        
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

    // ==================================================
    // 🌾 TOWN CENTER UNIT TRAINING QUEUE & HUD SYSTEM 🌾
    // ==================================================

    private void CreateTrainFarmerButton()
    {
        if (commandPanel == null) return;

        // Tạo Nút huấn luyện Nông Dân bên trong Action Command Panel (Bottom-Right)
        GameObject btnGo = new GameObject("TrainFarmerButton");
        btnGo.transform.SetParent(commandPanel.transform, false);
        
        var rectTrans = btnGo.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(70f, 70f);
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchoredPosition = new Vector2(-75f, 35f); // Đặt ở vị trí ô lệnh đầu tiên

        var img = btnGo.AddComponent<UnityEngine.UI.Image>();
        
        // Tải ảnh Gather đại diện cho Nông Dân làm Portrait tĩnh cho nút bấm
        #if UNITY_EDITOR
        var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/ASM/ButtonGather.jpg");
        if (sprite != null) img.sprite = sprite;
        #endif
        img.color = Color.white;

        trainFarmerButton = btnGo.AddComponent<Button>();
        trainFarmerButton.onClick.AddListener(OnTrainFarmerClicked);

        // Tạo hình ảnh phủ làm hiệu ứng Cooldown đè lên nút
        GameObject overlayGo = new GameObject("CooldownOverlay");
        overlayGo.transform.SetParent(btnGo.transform, false);
        var overlayRect = overlayGo.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;

        trainFarmerCooldownOverlay = overlayGo.AddComponent<UnityEngine.UI.Image>();
        trainFarmerCooldownOverlay.color = new Color(0f, 0f, 0f, 0.72f); // Màu tối mờ 72%
        trainFarmerCooldownOverlay.type = UnityEngine.UI.Image.Type.Filled;
        trainFarmerCooldownOverlay.fillMethod = UnityEngine.UI.Image.FillMethod.Radial360;
        trainFarmerCooldownOverlay.fillOrigin = (int)UnityEngine.UI.Image.Origin360.Top;
        trainFarmerCooldownOverlay.fillClockwise = false;
        trainFarmerCooldownOverlay.fillAmount = 0f;

        // Tạo text đếm ngược giây hiển thị ở giữa nút huấn luyện
        GameObject txtGo = new GameObject("CooldownText");
        txtGo.transform.SetParent(btnGo.transform, false);
        var txtRect = txtGo.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;

        trainFarmerCooldownText = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
        trainFarmerCooldownText.alignment = TMPro.TextAlignmentOptions.Center;
        trainFarmerCooldownText.fontSize = 18f;
        trainFarmerCooldownText.color = Color.white;
        trainFarmerCooldownText.text = "";

        // Gán font chữ medieval nếu có
        if (customGameFont != null) trainFarmerCooldownText.font = customGameFont;

        btnGo.SetActive(false); // Ẩn mặc định khi chưa chọn nhà chính
    }

    private void OnTrainFarmerClicked()
    {
        if (activeSelectedTownCenter != null)
        {
            activeSelectedTownCenter.StartTraining();
        }
    }

    private void Update()
    {
        // Đồng bộ đếm ngược huấn luyện Nông Dân thời gian thực của Nhà Chính lên HUD
        if (activeSelectedTownCenter != null && trainFarmerButton != null && trainFarmerButton.gameObject.activeSelf)
        {
            if (activeSelectedTownCenter.isTraining)
            {
                trainFarmerButton.interactable = false; // Đang train thì khóa nút
                
                // fillAmount giảm dần theo tiến trình huấn luyện
                float progress = activeSelectedTownCenter.trainingTimer / activeSelectedTownCenter.trainingDuration;
                if (trainFarmerCooldownOverlay != null)
                {
                    trainFarmerCooldownOverlay.fillAmount = progress;
                }
                if (trainFarmerCooldownText != null)
                {
                    trainFarmerCooldownText.text = $"{activeSelectedTownCenter.trainingTimer:F1}s";
                }
            }
            else
            {
                trainFarmerButton.interactable = true; // Sẵn sàng để train tiếp
                if (trainFarmerCooldownOverlay != null)
                {
                    trainFarmerCooldownOverlay.fillAmount = 0f;
                }
                if (trainFarmerCooldownText != null)
                {
                    trainFarmerCooldownText.text = "";
                }
            }
        }

        // Đảm bảo HP Bar luôn nằm trên cùng (không bị card đè mất) khi ở chế độ nhiều quân
        if (selectionPanel != null && selectionPanel.activeSelf && selectedUnitHPBar != null && selectedUnitHPBar.gameObject.activeSelf)
        {
            selectedUnitHPBar.transform.SetAsLastSibling();
            var r = selectedUnitHPBar.GetComponent<RectTransform>();
            var pos = r.localPosition;
            if (pos.z != -10f)
            {
                pos.z = -10f;
                r.localPosition = pos;
            }
        }
    }

    // Hàm hiển thị thông tin chi tiết khi chọn quân lính hoặc Nhà Chính (Town Center)
    public void ShowSelection(System.Collections.Generic.List<RTSUnit> selectedList, TownCenter selectedTC)
    {
        // Fail-safe để nạp lại portraitFrameSprite nếu nó bị null
        if (portraitFrameSprite == null && selectionPanel != null)
        {
            var frameGo = selectionPanel.transform.Find("PortraitFrame");
            if (frameGo != null)
            {
                var frameImg = frameGo.GetComponent<UnityEngine.UI.Image>();
                if (frameImg != null)
                {
                    portraitFrameSprite = frameImg.sprite;
                }
            }
        }

        // 1. Dọn dẹp toàn bộ chân dung nhóm cũ trước khi xử lý
        foreach (var go in activeGroupPortraits)
        {
            if (go != null) Destroy(go);
        }
        activeGroupPortraits.Clear();

        activeSelectedTownCenter = selectedTC;

        // Reset các nút lệnh trong command panel
        if (trainFarmerButton != null) trainFarmerButton.gameObject.SetActive(false);
        if (commandButtons != null)
        {
            foreach (var btn in commandButtons)
            {
                if (btn != null) btn.gameObject.SetActive(true);
            }
        }

        if (selectionPanel != null) selectionPanel.SetActive(true);

        // --- TRƯỜNG HỢP A: CHỌN NHÀ CHÍNH (TOWN CENTER) ---
        if (selectedTC != null)
        {
            // Ẩn tất cả các nút lệnh của quân lính, chỉ hiện nút Train Farmer
            if (commandButtons != null)
            {
                foreach (var btn in commandButtons)
                {
                    if (btn != null) btn.gameObject.SetActive(false);
                }
            }
            if (trainFarmerButton != null) trainFarmerButton.gameObject.SetActive(true);

            // Căn giữa NameText đối xứng đẹp mắt bên trên chân dung ở tọa độ (X=60f, Y=-12f)
            if (selectedUnitName != null)
            {
                var r = selectedUnitName.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 1f);
                r.anchorMax = new Vector2(0f, 1f);
                r.pivot = new Vector2(0.5f, 1f); // Set to (0.5, 1) for perfect top-center alignment above portrait
                r.anchoredPosition = new Vector2(60f, -12f); // Căn giữa cách đỉnh 12px
                r.sizeDelta = new Vector2(120f, 22f); 
                selectedUnitName.alignment = TMPro.TextAlignmentOptions.Center;
                selectedUnitName.fontSize = 22f; // Đồng bộ font size 22f
                selectedUnitName.text = "NHÀ CHÍNH"; 
            }

            // Đặt HP Bar Nhà Chính nằm dưới chân dung đối xứng hoàn hảo (X=60, Y=12)
            if (selectedUnitHPBar != null)
            {
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 0f);
                r.anchorMax = new Vector2(0f, 0f);
                r.pivot = originalHPPivot;
                r.anchoredPosition = new Vector2(60f, 12f); // HP Bar đối xứng ở X=60, cách đáy 12px
                r.sizeDelta = new Vector2(112f, originalHPSize.y); // Rộng 112px
                selectedUnitHPBar.gameObject.SetActive(true);
                selectedUnitHPBar.maxValue = 1000f; // Máu Nhà Chính
                selectedUnitHPBar.value = 1000f;

                // Đảm bảo HP Bar nằm trên cùng và local Z = -10f
                selectedUnitHPBar.transform.SetAsLastSibling();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
            }

            // Gán ảnh chân dung tĩnh cho Nhà Chính và căn chỉnh size 112x112 dịch xuống Y=-10
            if (selectedUnitPortrait != null)
            {
                selectedUnitPortrait.gameObject.SetActive(true);
                var r = selectedUnitPortrait.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f); // X=60, Y=-10 (dịch xuống 10px để tránh đè chữ)
                r.sizeDelta = new Vector2(112f, 112f);

                #if UNITY_EDITOR
                var tcSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/ASM/ButtonGather.jpg");
                if (tcSprite != null) selectedUnitPortrait.texture = tcSprite.texture;
                #endif
                selectedUnitPortrait.color = Color.white;
            }

            // Bật nền vàng đối xứng X=60, Y=-10
            if (singlePortraitBackground != null)
            {
                singlePortraitBackground.gameObject.SetActive(true);
                var r = singlePortraitBackground.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f);
                r.sizeDelta = new Vector2(112f, 112f);
            }

            // Khung viền đối xứng X=60, Y=-10
            var frame = selectionPanel.transform.Find("PortraitFrame");
            if (frame != null)
            {
                frame.gameObject.SetActive(true);
                var r = frame.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f);
                r.sizeDelta = new Vector2(120f, 120f);
            }

            return;
        }

        // --- TRƯỜNG HỢP B: CHỌN ĐẠO QUÂN LÍNH ---
        if (selectedList == null || selectedList.Count == 0)
        {
            HideSelectionPanel();
            return;
        }

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

        bool isMultiSelection = selectedList.Count > 1;

        if (!isMultiSelection)
        {
            // --- CHẾ ĐỘ 1: SINGLE LEADER 3D PORTRAIT ---
            RTSUnit leader = selectedList[0];

            // UPGRADE: Căn giữa NameText đối xứng đẹp mắt bên trên chân dung ở tọa độ (X=60f, Y=-12f) loại bỏ chồng lấn!
            if (selectedUnitName != null)
            {
                var r = selectedUnitName.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 1f);
                r.anchorMax = new Vector2(0f, 1f);
                r.pivot = new Vector2(0.5f, 1f); // Set to (0.5, 1) for perfect top-center alignment above portrait
                r.anchoredPosition = new Vector2(60f, -12f); // Căn giữa cách đỉnh 12px
                r.sizeDelta = new Vector2(120f, 22f); 
                selectedUnitName.alignment = TMPro.TextAlignmentOptions.Center;
                selectedUnitName.fontSize = 22f; // Đồng bộ font size 22f
                selectedUnitName.text = leader.unitName;
            }

            // UPGRADE: Đặt HP Bar đơn lẻ nằm dưới chân dung đối xứng hoàn hảo (X=60, Y=12)
            if (selectedUnitHPBar != null)
            {
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 0f);
                r.anchorMax = new Vector2(0f, 0f);
                r.pivot = originalHPPivot;
                r.anchoredPosition = new Vector2(60f, 12f); // HP Bar đối xứng ở X=60, cách đáy 12px
                r.sizeDelta = new Vector2(112f, originalHPSize.y); // Rộng 112px
                selectedUnitHPBar.gameObject.SetActive(true);

                // Đảm bảo HP Bar nằm trên cùng và local Z = -10f
                selectedUnitHPBar.transform.SetAsLastSibling();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
            }

            // UPGRADE: Căn chỉnh RawImage chân dung size 112x112 dịch xuống Y=-10
            if (selectedUnitPortrait != null)
            {
                selectedUnitPortrait.gameObject.SetActive(true);
                var r = selectedUnitPortrait.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f); // X=60, Y=-10 (dịch xuống 10px)
                r.sizeDelta = new Vector2(112f, 112f);

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

            // ĐỈNH CAO: Bật nền vàng quý tộc cho chân dung đơn lẻ đối xứng X=60, Y=-10
            if (singlePortraitBackground != null)
            {
                singlePortraitBackground.gameObject.SetActive(true);
                var r = singlePortraitBackground.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f);
                r.sizeDelta = new Vector2(112f, 112f);
            }

            // Khung viền đối xứng X=60, Y=-10
            var frame = selectionPanel.transform.Find("PortraitFrame");
            if (frame != null)
            {
                frame.gameObject.SetActive(true);
                var r = frame.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f);
                r.sizeDelta = new Vector2(120f, 120f);
            }

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
            if (singlePortraitBackground != null) singlePortraitBackground.gameObject.SetActive(false);
            
            var frame = selectionPanel.transform.Find("PortraitFrame");
            if (frame != null) frame.gameObject.SetActive(false);

            // Căn chỉnh NameText lên chính giữa phía trên (Top-Center)
            if (selectedUnitName != null)
            {
                var r = selectedUnitName.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0.5f, 1f);
                r.anchorMax = new Vector2(0.5f, 1f);
                r.pivot = new Vector2(0.5f, 1f); // Set to (0.5, 1) for perfect top-center alignment
                r.anchoredPosition = new Vector2(0f, -12f); // Đưa lên cao cách đỉnh 12px cực thoáng đãng và đồng bộ!
                r.sizeDelta = new Vector2(350f, 22f); // Thu hẹp chiều cao thành 22px để khớp với font đếm vàng kim
                selectedUnitName.alignment = TMPro.TextAlignmentOptions.Center; // Căn giữa chữ
                selectedUnitName.fontSize = 22f; // Font size bằng đúng size số đếm 22f tránh đè lệch

                // Yêu cầu: Hiển thị "ARMY (XX UNITS)" trong đó XX là tổng số quân lính đang chọn
                selectedUnitName.text = $"ARMY ({selectedList.Count} UNITS)";
            }

            // Căn chỉnh HP Slider xuống chính giữa phía dưới (Bottom-Center) để không bị đè và giữ nguyên tỉ lệ tròn gốc
            // Đặt Y = 12f để HP bar cao 20px nằm vừa khít trong khoảng trống bên dưới các card Y=-10!
            if (selectedUnitHPBar != null)
            {
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0.5f, 0f);
                r.anchorMax = new Vector2(0.5f, 0f);
                r.pivot = originalHPPivot; // Giữ nguyên pivot gốc để tránh biến dạng và lệch tâm UI
                r.anchoredPosition = new Vector2(0f, 12f); // Căn chính giữa đáy Y = 12px ( HP bar cao 20px sẽ nằm từ Y=2 đến Y=22)
                r.sizeDelta = new Vector2(250f, originalHPSize.y); // Giữ nguyên chiều cao gốc tránh méo bo tròn slider, kéo dài 250px cực sang trọng
                selectedUnitHPBar.gameObject.SetActive(true);

                // Đảm bảo HP Bar nằm trên cùng và local Z = -10f
                selectedUnitHPBar.transform.SetAsLastSibling();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
            }

            // Tính toán khoảng cách để xếp thẻ song song ở giữa SelectionPanel
            int groupCount = typeGroups.Count;
            
            // UPGRADE: Thẻ nhóm to bằng portrait đơn (112 x 112), nên dùng groupPortraitSpacing rộng hơn (140f)
            float groupPortraitSpacingUpgrade = 140f; 
            float startX = -((groupCount - 1) * groupPortraitSpacingUpgrade) / 2f;
            int index = 0;

            foreach (var kvp in typeGroups)
            {
                RTSUnitType type = kvp.Key;
                System.Collections.Generic.List<RTSUnit> list = kvp.Value;
                RTSUnit firstOfGroup = list[0];

                // 1. Tạo thẻ nhóm (Group Card) - UPGRADE size to 112 x 112, dịch xuống Y=-10 để đồng bộ 100% với single selection!
                GameObject cardGo = new GameObject($"GroupCard_{type}");
                cardGo.transform.SetParent(selectionPanel.transform, false);
                activeGroupPortraits.Add(cardGo);

                var rectTrans = cardGo.AddComponent<RectTransform>();
                rectTrans.sizeDelta = new Vector2(112f, 112f);
                // Căn chỉnh nằm giữa và dịch chuyển sang hai bên
                rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
                rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
                rectTrans.pivot = new Vector2(0.5f, 0.5f);
                rectTrans.anchoredPosition = new Vector2(startX + index * groupPortraitSpacingUpgrade, -10f); // Xếp ở trung tâm Y = -10f (nằm từ Y=14px đến Y=126px)

                // 2. Tạo hình ảnh nền vàng kim (Gold Background) nằm dưới chân dung
                GameObject bgGo = new GameObject("Background");
                bgGo.transform.SetParent(cardGo.transform, false);
                var bgRect = bgGo.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.sizeDelta = Vector2.zero; // Stretch-Stretch

                var bgImg = bgGo.AddComponent<UnityEngine.UI.Image>();
                bgImg.color = new Color(0.85f, 0.7f, 0.3f, 0.8f); // Màu vàng kim quý tộc

                // 3. Tạo hình ảnh chân dung đại diện cho nhóm (3D head portraits, thụt nhẹ vào 3px để lộ viền)
                GameObject imgGo = new GameObject("PortraitImage");
                imgGo.transform.SetParent(cardGo.transform, false);
                var imgRect = imgGo.AddComponent<RectTransform>();
                imgRect.anchorMin = Vector2.zero;
                imgRect.anchorMax = Vector2.one;
                imgRect.sizeDelta = new Vector2(-6f, -6f); // Co vào 3px mỗi mép để nằm khít trong khung viền

                var rawImg = imgGo.AddComponent<UnityEngine.UI.RawImage>();
                RenderTexture targetRT = type == RTSUnitType.Farmer ? farmerRT : soldierRT;
                rawImg.texture = targetRT;
                rawImg.color = targetRT != null ? Color.white : new Color(0.12f, 0.12f, 0.18f, 0.65f);

                // 4. Tạo Khung Viền PNG Quý Tộc Cho Thẻ Nhóm (ĐỈNH CAO: Đè LÊN chân dung, đồng bộ 100% với single selection!)
                GameObject borderGo = new GameObject("Border");
                borderGo.transform.SetParent(cardGo.transform, false);
                var borderRect = borderGo.AddComponent<RectTransform>();
                borderRect.anchorMin = Vector2.zero;
                borderRect.anchorMax = Vector2.one;
                borderRect.sizeDelta = new Vector2(8f, 8f); // Kích thước khung to hơn 4px mỗi mép để ôm trọn card

                var borderImg = borderGo.AddComponent<UnityEngine.UI.Image>();
                borderImg.color = Color.white; // Màu trắng để sprite hiển thị nguyên gốc
                if (portraitFrameSprite != null)
                {
                    borderImg.sprite = portraitFrameSprite;
                }
                else
                {
                    borderImg.color = new Color(0.85f, 0.7f, 0.3f, 0.8f);
                }

                // 5. Tạo văn bản đếm số lượng nổi lên phía trên thẻ (Unit Counter Text)
                // UPGRADE: Đặt bên trong card ở góc trên (Y=-4f, pivot=1) để hoàn toàn không chạm vào ARMY text!
                GameObject countGo = new GameObject("CounterText");
                countGo.transform.SetParent(cardGo.transform, false);
                var countRect = countGo.AddComponent<RectTransform>();
                countRect.sizeDelta = new Vector2(80f, 30f);
                countRect.anchorMin = new Vector2(0.5f, 1f); // Căn ở giữa cạnh trên của card (trong card)
                countRect.anchorMax = new Vector2(0.5f, 1f);
                countRect.pivot = new Vector2(0.5f, 1f); // Xoay quanh đỉnh chữ
                countRect.anchoredPosition = new Vector2(0f, -4f); // 4px dưới cạnh trên card (nằm hoàn toàn trong card!)

                var countText = countGo.AddComponent<TMPro.TextMeshProUGUI>();
                countText.text = $"{list.Count}"; // Ví dụ: 8 hoặc 4
                countText.alignment = TMPro.TextAlignmentOptions.Center;
                countText.fontSize = 22f;
                countText.color = new Color(1f, 0.85f, 0f); // Màu vàng gold
                
                // Gán font chữ
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
                
                // CỰC KỲ QUAN TRỌNG: Đẩy thanh HP lên render ở lớp TRÊN CÙNG của panel để không bị các thẻ lính đè mất!
                selectedUnitHPBar.transform.SetAsLastSibling();
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
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

        activeSelectedTownCenter = null;

        if (selectionPanel != null) selectionPanel.SetActive(false);
        if (singlePortraitBackground != null) singlePortraitBackground.gameObject.SetActive(false);
        if (trainFarmerButton != null) trainFarmerButton.gameObject.SetActive(false);

        if (commandButtons != null)
        {
            foreach (var btn in commandButtons)
            {
                if (btn != null) btn.gameObject.SetActive(true);
            }
        }
    }
}
