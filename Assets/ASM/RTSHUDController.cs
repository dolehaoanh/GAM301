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
    [Tooltip("Bảng Panel Chiến Thắng (Victory Panel)")]
    public GameObject victoryPanel;
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

    
    private Vector2 originalNamePos;
    private Vector2 originalNameSize;
    private Vector2 originalNameAnchorMin;
    private Vector2 originalNameAnchorMax;
    private Vector2 originalNamePivot;
    private float originalNameFontSize;

    private Vector2 originalHPPos;
    private Vector2 originalHPSize;
    private Vector2 originalHPAnchorMin;
    private Vector2 originalHPAnchorMax;
    private Vector2 originalHPPivot;
    private TMPro.TextAlignmentOptions originalNameAlignment;

    
    private RenderTexture farmerRT;
    private RenderTexture soldierRT;
    private Camera farmerCam;
    private Camera soldierCam;

    
    private Texture originalPortraitTexture;

    
    private UnityEngine.UI.Image singlePortraitBackground;
    private Sprite portraitFrameSprite;
    private Button trainFarmerButton;
    private UnityEngine.UI.Image trainFarmerCooldownOverlay;
    private TMPro.TextMeshProUGUI trainFarmerCooldownText;
    private TownCenter activeSelectedTownCenter;

    private Button trainSoldierButton;
    private UnityEngine.UI.Image trainSoldierCooldownOverlay;
    private TMPro.TextMeshProUGUI trainSoldierCooldownText;
    private Barracks activeSelectedBarracks;

    private Button buildBarracksButton;

    private void Start()
    {
        
        if (selectedUnitName != null)
        {
            var r = selectedUnitName.GetComponent<RectTransform>();
            originalNamePos = r.anchoredPosition;
            originalNameSize = r.sizeDelta;
            originalNameAnchorMin = r.anchorMin;
            originalNameAnchorMax = r.anchorMax;
            originalNamePivot = r.pivot;
            originalNameAlignment = selectedUnitName.alignment;
            originalNameFontSize = selectedUnitName.fontSize;
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

        
        CleanPortraitInstance(portraitFarmerInstance);
        CleanPortraitInstance(portraitSoldierInstance);

        
        SetupDynamicPortraits();

        
        if (customGameFont == null)
        {
            #if UNITY_EDITOR
            customGameFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/ASM/Cinzel-VariableFont_wght SDF.asset");
            #endif
        }

        
        if (selectedUnitPortrait != null && selectedUnitPortrait.texture == null)
        {
            var portCam = GameObject.Find("PortraitCamera")?.GetComponent<Camera>();
            if (portCam != null && portCam.targetTexture != null)
            {
                selectedUnitPortrait.texture = portCam.targetTexture;
            }
        }

        if (selectedUnitPortrait != null)
        {
            originalPortraitTexture = selectedUnitPortrait.texture;
        }

        
        CleanPortraitInstance(portraitFarmerInstance);
        CleanPortraitInstance(portraitSoldierInstance);

        
        InvokeRepeating(nameof(UpdateDynamicPopulation), 0f, 0.5f);

        
        PlayerResourceManager.OnResourcesChanged += UpdateHUDResources;

        
        if (selectedUnitPortrait != null)
        {
            GameObject bgGo = new GameObject("SinglePortraitBackground");
            bgGo.transform.SetParent(selectedUnitPortrait.transform.parent, false);
            bgGo.transform.SetSiblingIndex(selectedUnitPortrait.transform.GetSiblingIndex()); 

            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = selectedUnitPortrait.rectTransform.anchorMin;
            bgRect.anchorMax = selectedUnitPortrait.rectTransform.anchorMax;
            bgRect.pivot = selectedUnitPortrait.rectTransform.pivot;
            bgRect.anchoredPosition = selectedUnitPortrait.rectTransform.anchoredPosition;
            bgRect.sizeDelta = selectedUnitPortrait.rectTransform.sizeDelta;

            singlePortraitBackground = bgGo.AddComponent<UnityEngine.UI.Image>();
            singlePortraitBackground.color = new Color(0.85f, 0.7f, 0.3f, 0.8f); 
            singlePortraitBackground.gameObject.SetActive(false);
        }

        
        CreateTrainFarmerButton();
        CreateTrainSoldierButton();
        CreateBuildBarracksButton();

        
        var frameGo = selectionPanel.transform.Find("PortraitFrame");
        if (frameGo != null)
        {
            var frameImg = frameGo.GetComponent<UnityEngine.UI.Image>();
            if (frameImg != null)
            {
                portraitFrameSprite = frameImg.sprite;
            }
        }
        
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        HideSelectionPanel();
    }

    private void CleanPortraitInstance(GameObject go)
    {
        if (go == null) return;

        
        var anim = go.GetComponent<RTSUnitAnimation>();
        if (anim != null) Destroy(anim);

        
        var unit = go.GetComponent<RTSUnit>();
        if (unit != null) Destroy(unit);

        
        var agent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) Destroy(agent);

        
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private void RestoreOriginalNameLayout()
    {
        if (selectedUnitName != null)
        {
            var r = selectedUnitName.GetComponent<RectTransform>();
            r.anchoredPosition = originalNamePos;
            r.sizeDelta = originalNameSize;
            r.anchorMin = originalNameAnchorMin;
            r.anchorMax = originalNameAnchorMax;
            r.pivot = originalNamePivot;
            selectedUnitName.alignment = originalNameAlignment;
            selectedUnitName.fontSize = originalNameFontSize;
        }
    }

    private void RestoreOriginalHPLayout()
    {
        if (selectedUnitHPBar != null)
        {
            var r = selectedUnitHPBar.GetComponent<RectTransform>();
            r.anchoredPosition = originalHPPos;
            r.sizeDelta = originalHPSize;
            r.anchorMin = originalHPAnchorMin;
            r.anchorMax = originalHPAnchorMax;
            r.pivot = originalHPPivot;
        }
    }

    private void SetupDynamicPortraits()
    {
        if (portraitFarmerInstance == null || portraitSoldierInstance == null) return;

        
        portraitSoldierInstance.transform.localPosition = new Vector3(5f, 0f, 0f);

        
        portraitFarmerInstance.SetActive(true);
        portraitSoldierInstance.SetActive(true);

        
        var originalCamGo = GameObject.Find("PortraitCamera");
        if (originalCamGo == null) return;
        var originalCam = originalCamGo.GetComponent<Camera>();
        if (originalCam == null) return;

        
        farmerRT = new RenderTexture(256, 256, 16);
        farmerRT.name = "DynamicFarmerRT";
        soldierRT = new RenderTexture(256, 256, 16);
        soldierRT.name = "DynamicSoldierRT";

        
        originalCam.enabled = false;

        
        GameObject farmerCamGo = Instantiate(originalCamGo, originalCamGo.transform.parent);
        farmerCamGo.name = "FarmerPortraitCamera";
        farmerCam = farmerCamGo.GetComponent<Camera>();
        farmerCam.enabled = true;
        farmerCam.targetTexture = farmerRT;
        farmerCam.transform.localPosition = new Vector3(0f, 0.66f, 1.80f); 

        
        GameObject soldierCamGo = Instantiate(originalCamGo, originalCamGo.transform.parent);
        soldierCamGo.name = "SoldierPortraitCamera";
        soldierCam = soldierCamGo.GetComponent<Camera>();
        soldierCam.enabled = true;
        soldierCam.targetTexture = soldierRT;
        soldierCam.transform.localPosition = new Vector3(5f, 0.66f, 1.80f); 

        
        var originalLightGo = GameObject.Find("PortraitLight");
        if (originalLightGo != null)
        {
            GameObject soldierLightGo = Instantiate(originalLightGo, originalLightGo.transform.parent);
            soldierLightGo.name = "SoldierPortraitLight";
            soldierLightGo.transform.localPosition = new Vector3(5f, originalLightGo.transform.localPosition.y, originalLightGo.transform.localPosition.z);
        }

        
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

        int currentFood = PlayerResourceManager.Instance.GetCurrentFoodUsed();
        int maxFood = PlayerResourceManager.Instance.maxFood;

        UpdateResourcesDisplay(
            PlayerResourceManager.Instance.gold, 
            PlayerResourceManager.Instance.wood, 
            currentFood, 
            maxFood
        );
    }

    private void UpdateDynamicPopulation()
    {
        UpdateHUDResources();
    }

    
    public void UpdateResourcesDisplay(int gold, int wood, int currentFood, int maxFood)
    {
        if (goldText != null) goldText.text = $"GOLD: {gold}";
        if (woodText != null) woodText.text = $"WOOD: {wood}";
        if (populationText != null) populationText.text = $"FOOD: {currentFood}/{maxFood}";
    }

    
    
    

    private void CreateTrainFarmerButton()
    {
        if (commandPanel == null) return;

        
        GameObject btnGo = new GameObject("TrainFarmerButton");
        btnGo.transform.SetParent(commandPanel.transform, false);
        
        var rectTrans = btnGo.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(70f, 70f);
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchoredPosition = new Vector2(-75f, 35f); 

        
        btnGo.AddComponent<LayoutElement>();

        var img = btnGo.AddComponent<UnityEngine.UI.Image>();
        
        
        #if UNITY_EDITOR
        var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/ASM/ButtonGather.jpg");
        if (sprite != null) img.sprite = sprite;
        #endif
        
        Sprite gatherSprite = commandPanel.transform.Find("Button_Gather")?.GetComponent<UnityEngine.UI.Image>()?.sprite;
        if (gatherSprite != null) img.sprite = gatherSprite;
        img.color = Color.white;

        trainFarmerButton = btnGo.AddComponent<Button>();
        trainFarmerButton.onClick.AddListener(OnTrainFarmerClicked);

        
        GameObject overlayGo = new GameObject("CooldownOverlay");
        overlayGo.transform.SetParent(btnGo.transform, false);
        var overlayRect = overlayGo.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;

        trainFarmerCooldownOverlay = overlayGo.AddComponent<UnityEngine.UI.Image>();
        trainFarmerCooldownOverlay.sprite = img.sprite; 
        trainFarmerCooldownOverlay.color = new Color(0f, 0f, 0f, 0.72f); 
        trainFarmerCooldownOverlay.type = UnityEngine.UI.Image.Type.Filled;
        trainFarmerCooldownOverlay.fillMethod = UnityEngine.UI.Image.FillMethod.Radial360;
        trainFarmerCooldownOverlay.fillOrigin = (int)UnityEngine.UI.Image.Origin360.Top;
        trainFarmerCooldownOverlay.fillClockwise = false;
        trainFarmerCooldownOverlay.fillAmount = 0f;
        overlayGo.SetActive(false); 

        
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

        
        if (customGameFont != null) trainFarmerCooldownText.font = customGameFont;

        btnGo.SetActive(false); 
    }

    public Sprite buildBarracksIcon;

    private void CreateBuildBarracksButton()
    {
        if (commandPanel == null) return;

        GameObject btnGo = new GameObject("BuildBarracksButton");
        btnGo.transform.SetParent(commandPanel.transform, false);
        
        var rectTrans = btnGo.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(70f, 70f);
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchoredPosition = new Vector2(5f, 35f); 

        btnGo.AddComponent<LayoutElement>();

        var img = btnGo.AddComponent<UnityEngine.UI.Image>();
        
        if (buildBarracksIcon != null)
        {
            img.sprite = buildBarracksIcon;
        }
        else
        {
            #if UNITY_EDITOR
            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/ASM/ButtonWood.png");
            if (sprite != null) img.sprite = sprite;
            #endif
        }
        
        Sprite moveSprite = commandPanel.transform.Find("Button_Move")?.GetComponent<UnityEngine.UI.Image>()?.sprite;
        if (moveSprite != null && img.sprite == null) img.sprite = moveSprite;
        img.color = Color.white;

        buildBarracksButton = btnGo.AddComponent<Button>();
        buildBarracksButton.onClick.AddListener(OnBuildBarracksClicked);

        btnGo.SetActive(false);
    }

    private void OnBuildBarracksClicked()
    {
        if (BuildingPlacer.Instance != null && activeSelectedTownCenter != null)
        {
            BuildingPlacer.Instance.StartPlacement(BuildingPlacer.Instance.barracksPrefab, BuildingPlacer.Instance.barracksCost);
        }
    }

    private void OnTrainFarmerClicked()
    {
        if (activeSelectedTownCenter != null)
        {
            activeSelectedTownCenter.StartTraining();
        }
    }

    private void CreateTrainSoldierButton()
    {
        if (commandPanel == null) return;

        
        GameObject btnGo = new GameObject("TrainSoldierButton");
        btnGo.transform.SetParent(commandPanel.transform, false);
        
        var rectTrans = btnGo.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(70f, 70f);
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        rectTrans.pivot = new Vector2(0.5f, 0.5f);

        
        btnGo.AddComponent<LayoutElement>();

        var img = btnGo.AddComponent<UnityEngine.UI.Image>();
        
        
        #if UNITY_EDITOR
        var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/ASM/ButtonAttack.jpg");
        if (sprite != null) img.sprite = sprite;
        #endif
        
        Sprite attackSprite = commandPanel.transform.Find("Button_Attack")?.GetComponent<UnityEngine.UI.Image>()?.sprite;
        if (attackSprite != null) img.sprite = attackSprite;
        img.color = Color.white;

        trainSoldierButton = btnGo.AddComponent<Button>();
        trainSoldierButton.onClick.AddListener(OnTrainSoldierClicked);

        
        GameObject overlayGo = new GameObject("CooldownOverlay");
        overlayGo.transform.SetParent(btnGo.transform, false);
        var overlayRect = overlayGo.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;

        trainSoldierCooldownOverlay = overlayGo.AddComponent<UnityEngine.UI.Image>();
        trainSoldierCooldownOverlay.sprite = img.sprite; 
        trainSoldierCooldownOverlay.color = new Color(0f, 0f, 0f, 0.72f); 
        trainSoldierCooldownOverlay.type = UnityEngine.UI.Image.Type.Filled;
        trainSoldierCooldownOverlay.fillMethod = UnityEngine.UI.Image.FillMethod.Radial360;
        trainSoldierCooldownOverlay.fillOrigin = (int)UnityEngine.UI.Image.Origin360.Top;
        trainSoldierCooldownOverlay.fillClockwise = false;
        trainSoldierCooldownOverlay.fillAmount = 0f;
        overlayGo.SetActive(false); 

        
        GameObject txtGo = new GameObject("CooldownText");
        txtGo.transform.SetParent(btnGo.transform, false);
        var txtRect = txtGo.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;

        trainSoldierCooldownText = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
        trainSoldierCooldownText.alignment = TMPro.TextAlignmentOptions.Center;
        trainSoldierCooldownText.fontSize = 18f;
        trainSoldierCooldownText.color = Color.white;
        trainSoldierCooldownText.text = "";

        
        if (customGameFont != null) trainSoldierCooldownText.font = customGameFont;

        btnGo.SetActive(false); 
    }

    private void OnTrainSoldierClicked()
    {
        if (activeSelectedBarracks != null)
        {
            activeSelectedBarracks.StartTraining();
        }
    }

    private void Update()
    {
        
        if (activeSelectedTownCenter != null && trainFarmerButton != null && trainFarmerButton.gameObject.activeSelf)
        {
            if (activeSelectedTownCenter.isTraining)
            {
                trainFarmerButton.interactable = false; 
                
                
                float progress = activeSelectedTownCenter.trainingTimer / activeSelectedTownCenter.trainingDuration;
                if (trainFarmerCooldownOverlay != null)
                {
                    trainFarmerCooldownOverlay.gameObject.SetActive(true);
                    trainFarmerCooldownOverlay.fillAmount = progress;
                }
                if (trainFarmerCooldownText != null)
                {
                    trainFarmerCooldownText.text = $"{activeSelectedTownCenter.trainingTimer:F1}s";
                }
            }
            else
            {
                trainFarmerButton.interactable = true; 
                if (trainFarmerCooldownOverlay != null)
                {
                    trainFarmerCooldownOverlay.fillAmount = 0f;
                    trainFarmerCooldownOverlay.gameObject.SetActive(false);
                }
                if (trainFarmerCooldownText != null)
                {
                    trainFarmerCooldownText.text = "";
                }
            }
        }

        
        if (activeSelectedBarracks != null && trainSoldierButton != null && trainSoldierButton.gameObject.activeSelf)
        {
            if (activeSelectedBarracks.isTraining)
            {
                trainSoldierButton.interactable = false; 
                
                
                float progress = activeSelectedBarracks.trainingTimer / activeSelectedBarracks.trainingDuration;
                if (trainSoldierCooldownOverlay != null)
                {
                    trainSoldierCooldownOverlay.gameObject.SetActive(true);
                    trainSoldierCooldownOverlay.fillAmount = progress;
                }
                if (trainSoldierCooldownText != null)
                {
                    trainSoldierCooldownText.text = $"{activeSelectedBarracks.trainingTimer:F1}s";
                }
            }
            else
            {
                trainSoldierButton.interactable = true; 
                if (trainSoldierCooldownOverlay != null)
                {
                    trainSoldierCooldownOverlay.fillAmount = 0f;
                    trainSoldierCooldownOverlay.gameObject.SetActive(false);
                }
                if (trainSoldierCooldownText != null)
                {
                    trainSoldierCooldownText.text = "";
                }
            }
        }

        
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

        
        if (activeSelectedTownCenter != null)
        {
            if (selectedUnitHPBar != null)
            {
                selectedUnitHPBar.value = activeSelectedTownCenter.currentHP;
            }
            if (activeSelectedTownCenter.currentHP <= 0f)
            {
                HideSelectionPanel();
            }
        }
        else if (activeSelectedBarracks != null)
        {
            if (selectedUnitHPBar != null)
            {
                selectedUnitHPBar.value = activeSelectedBarracks.currentHP;
            }
            if (activeSelectedBarracks.currentHP <= 0f)
            {
                HideSelectionPanel();
            }
        }
    }

    
    public void ShowSelection(System.Collections.Generic.List<RTSUnit> selectedList, TownCenter selectedTC, Barracks selectedB = null)
    {
        
        if (selectionPanel != null && selectionPanel.transform.parent != null)
        {
            var bottomPanelImg = selectionPanel.transform.parent.GetComponent<UnityEngine.UI.Image>();
            if (bottomPanelImg != null) bottomPanelImg.enabled = true;
        }

        
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

        
        foreach (var go in activeGroupPortraits)
        {
            if (go != null) Destroy(go);
        }
        activeGroupPortraits.Clear();

        activeSelectedTownCenter = selectedTC;
        activeSelectedBarracks = selectedB;

        
        if (trainFarmerButton != null) trainFarmerButton.gameObject.SetActive(false);
        if (buildBarracksButton != null) buildBarracksButton.gameObject.SetActive(false);
        if (trainSoldierButton != null) trainSoldierButton.gameObject.SetActive(false);
        if (commandButtons != null)
        {
            foreach (var btn in commandButtons)
            {
                if (btn != null) btn.gameObject.SetActive(true);
            }
        }

        if (selectionPanel != null) selectionPanel.SetActive(true);

        
        if (selectedB != null)
        {
            
            if (commandButtons != null)
            {
                foreach (var btn in commandButtons)
                {
                    if (btn != null) btn.gameObject.SetActive(false);
                }
            }
            if (trainSoldierButton != null) trainSoldierButton.gameObject.SetActive(true);

            
            if (selectedUnitName != null)
            {
                RestoreOriginalNameLayout();
                selectedUnitName.text = "NHÀ LÍNH"; 
            }

            
            if (selectedUnitHPBar != null)
            {
                RestoreOriginalHPLayout();
                selectedUnitHPBar.gameObject.SetActive(true);
                selectedUnitHPBar.maxValue = selectedB.maxHP; 
                selectedUnitHPBar.value = selectedB.currentHP;

                
                selectedUnitHPBar.transform.SetAsLastSibling();
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
            }

            
            if (selectedUnitPortrait != null)
            {
                selectedUnitPortrait.gameObject.SetActive(true);
                var r = selectedUnitPortrait.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f); 
                r.sizeDelta = new Vector2(112f, 112f);

                
                Sprite attackSprite = commandPanel.transform.Find("Button_Attack")?.GetComponent<UnityEngine.UI.Image>()?.sprite;
                if (attackSprite != null)
                {
                    selectedUnitPortrait.texture = attackSprite.texture;
                }
                selectedUnitPortrait.color = Color.white;
            }

            
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

        
        if (selectedTC != null)
        {
            
            if (commandButtons != null)
            {
                foreach (var btn in commandButtons)
                {
                    if (btn != null) btn.gameObject.SetActive(false);
                }
            }
            if (trainFarmerButton != null) trainFarmerButton.gameObject.SetActive(true);
            if (buildBarracksButton != null) buildBarracksButton.gameObject.SetActive(true);

            
            if (selectedUnitName != null)
            {
                RestoreOriginalNameLayout();
                selectedUnitName.text = "NHÀ CHÍNH"; 
            }

            
            if (selectedUnitHPBar != null)
            {
                RestoreOriginalHPLayout();
                selectedUnitHPBar.gameObject.SetActive(true);
                selectedUnitHPBar.maxValue = selectedTC.maxHP; 
                selectedUnitHPBar.value = selectedTC.currentHP;

                
                selectedUnitHPBar.transform.SetAsLastSibling();
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
            }

            
            if (selectedUnitPortrait != null)
            {
                selectedUnitPortrait.gameObject.SetActive(true);
                var r = selectedUnitPortrait.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f); 
                r.sizeDelta = new Vector2(112f, 112f);

                #if UNITY_EDITOR
                var tcSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/ASM/ButtonGather.jpg");
                if (tcSprite != null) selectedUnitPortrait.texture = tcSprite.texture;
                #endif
                selectedUnitPortrait.color = Color.white;
            }

            
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

        
        if (selectedList == null || selectedList.Count == 0)
        {
            HideSelectionPanel();
            return;
        }

        
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
            
            RTSUnit leader = selectedList[0];

            
            if (selectedUnitName != null)
            {
                RestoreOriginalNameLayout();
                selectedUnitName.text = leader.unitName;
            }

            
            if (selectedUnitHPBar != null)
            {
                RestoreOriginalHPLayout();
                selectedUnitHPBar.gameObject.SetActive(true);

                
                selectedUnitHPBar.transform.SetAsLastSibling();
                var r = selectedUnitHPBar.GetComponent<RectTransform>();
                var pos = r.localPosition;
                pos.z = -10f;
                r.localPosition = pos;
            }

            
            if (selectedUnitPortrait != null)
            {
                selectedUnitPortrait.gameObject.SetActive(true);
                var r = selectedUnitPortrait.rectTransform;
                r.anchorMin = new Vector2(0f, 0.5f);
                r.anchorMax = new Vector2(0f, 0.5f);
                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = new Vector2(60f, -10f); 
                r.sizeDelta = new Vector2(112f, 112f);

                if (leader.portrait != null)
                {
                    selectedUnitPortrait.texture = leader.portrait.texture;
                    selectedUnitPortrait.color = Color.white;
                }
                else
                {
                    
                    RenderTexture targetRT = leader.unitType == RTSUnitType.Farmer ? farmerRT : soldierRT;
                    selectedUnitPortrait.texture = targetRT;
                    selectedUnitPortrait.color = targetRT != null ? Color.white : new Color(0.12f, 0.12f, 0.18f, 0.65f);
                }
            }

            
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
            
            
            if (selectedUnitPortrait != null) selectedUnitPortrait.gameObject.SetActive(false);
            if (singlePortraitBackground != null) singlePortraitBackground.gameObject.SetActive(false);
            
            var frame = selectionPanel.transform.Find("PortraitFrame");
            if (frame != null) frame.gameObject.SetActive(false);

            
            if (selectedUnitName != null)
            {
                RestoreOriginalNameLayout();
                selectedUnitName.text = $"ARMY ({selectedList.Count} UNITS)";
            }

            
            
            if (selectedUnitHPBar != null)
            {
                selectedUnitHPBar.gameObject.SetActive(false);
            }

            
            int groupCount = typeGroups.Count;
            
            
            float groupPortraitSpacingUpgrade = 140f; 
            float startX = -((groupCount - 1) * groupPortraitSpacingUpgrade) / 2f;
            int index = 0;

            foreach (var kvp in typeGroups)
            {
                RTSUnitType type = kvp.Key;
                System.Collections.Generic.List<RTSUnit> list = kvp.Value;
                RTSUnit firstOfGroup = list[0];

                
                GameObject cardGo = new GameObject($"GroupCard_{type}");
                cardGo.transform.SetParent(selectionPanel.transform, false);
                activeGroupPortraits.Add(cardGo);

                var rectTrans = cardGo.AddComponent<RectTransform>();
                rectTrans.sizeDelta = new Vector2(112f, 112f);
                
                rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
                rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
                rectTrans.pivot = new Vector2(0.5f, 0.5f);
                rectTrans.anchoredPosition = new Vector2(startX + index * groupPortraitSpacingUpgrade, -10f); 

                
                GameObject bgGo = new GameObject("Background");
                bgGo.transform.SetParent(cardGo.transform, false);
                var bgRect = bgGo.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.sizeDelta = Vector2.zero; 

                var bgImg = bgGo.AddComponent<UnityEngine.UI.Image>();
                bgImg.color = new Color(0.85f, 0.7f, 0.3f, 0.8f); 

                
                GameObject imgGo = new GameObject("PortraitImage");
                imgGo.transform.SetParent(cardGo.transform, false);
                var imgRect = imgGo.AddComponent<RectTransform>();
                imgRect.anchorMin = Vector2.zero;
                imgRect.anchorMax = Vector2.one;
                imgRect.sizeDelta = new Vector2(-6f, -6f); 

                var rawImg = imgGo.AddComponent<UnityEngine.UI.RawImage>();
                RenderTexture targetRT = type == RTSUnitType.Farmer ? farmerRT : soldierRT;
                rawImg.texture = targetRT;
                rawImg.color = targetRT != null ? Color.white : new Color(0.12f, 0.12f, 0.18f, 0.65f);

                
                GameObject borderGo = new GameObject("Border");
                borderGo.transform.SetParent(cardGo.transform, false);
                var borderRect = borderGo.AddComponent<RectTransform>();
                borderRect.anchorMin = Vector2.zero;
                borderRect.anchorMax = Vector2.one;
                borderRect.sizeDelta = new Vector2(8f, 8f); 

                var borderImg = borderGo.AddComponent<UnityEngine.UI.Image>();
                borderImg.color = Color.white; 
                if (portraitFrameSprite != null)
                {
                    borderImg.sprite = portraitFrameSprite;
                }
                else
                {
                    borderImg.color = new Color(0.85f, 0.7f, 0.3f, 0.8f);
                }

                
                
                GameObject countGo = new GameObject("CounterText");
                countGo.transform.SetParent(cardGo.transform, false);
                var countRect = countGo.AddComponent<RectTransform>();
                countRect.sizeDelta = new Vector2(80f, 30f);
                countRect.anchorMin = new Vector2(0.5f, 0f);
                countRect.anchorMax = new Vector2(0.5f, 0f);
                countRect.pivot = new Vector2(0.5f, 0f);
                countRect.anchoredPosition = new Vector2(0f, -36f);

                var countText = countGo.AddComponent<TMPro.TextMeshProUGUI>();
                countText.text = $"{list.Count}"; 
                countText.alignment = TMPro.TextAlignmentOptions.Center;
                countText.fontSize = 16f;
                countText.color = Color.white; 
                
                
                if (customGameFont != null)
                {
                    countText.font = customGameFont;
                }
                else if (selectedUnitName != null)
                {
                    countText.font = selectedUnitName.font;
                }

                // Add UnitNameText above portrait frame for each group card
                GameObject nameGo = new GameObject("UnitNameText");
                nameGo.transform.SetParent(borderGo.transform, false);

                var nameRect = nameGo.AddComponent<RectTransform>();
                if (selectedUnitName != null)
                {
                    var origRect = selectedUnitName.GetComponent<RectTransform>();
                    nameRect.anchorMin = origRect.anchorMin;
                    nameRect.anchorMax = origRect.anchorMax;
                    nameRect.pivot = origRect.pivot;
                    nameRect.anchoredPosition = origRect.anchoredPosition;
                    nameRect.sizeDelta = origRect.sizeDelta;
                    nameRect.localScale = origRect.localScale;
                }

                var groupNameText = nameGo.AddComponent<TMPro.TextMeshProUGUI>();
                groupNameText.text = firstOfGroup.unitName.ToUpper();
                if (selectedUnitName != null)
                {
                    groupNameText.font = selectedUnitName.font;
                    groupNameText.fontSize = selectedUnitName.fontSize;
                    groupNameText.color = selectedUnitName.color;
                    groupNameText.fontStyle = selectedUnitName.fontStyle;
                    groupNameText.alignment = selectedUnitName.alignment;
                    groupNameText.fontSharedMaterial = selectedUnitName.fontSharedMaterial;

                    // Copy Outline/Shadow components if they exist
                    var origOutline = selectedUnitName.GetComponent<UnityEngine.UI.Outline>();
                    if (origOutline != null)
                    {
                        var newOutline = nameGo.AddComponent<UnityEngine.UI.Outline>();
                        newOutline.effectColor = origOutline.effectColor;
                        newOutline.effectDistance = origOutline.effectDistance;
                        newOutline.useGraphicAlpha = origOutline.useGraphicAlpha;
                    }
                    var origShadow = selectedUnitName.GetComponent<UnityEngine.UI.Shadow>();
                    if (origShadow != null && origOutline == null)
                    {
                        var newShadow = nameGo.AddComponent<UnityEngine.UI.Shadow>();
                        newShadow.effectColor = origShadow.effectColor;
                        newShadow.effectDistance = origShadow.effectDistance;
                        newShadow.useGraphicAlpha = origShadow.useGraphicAlpha;
                    }
                }

                index++;
            }

            
            if (selectedUnitHPBar != null)
            {
                selectedUnitHPBar.gameObject.SetActive(false);
            }
        }
        UpdateGatherButtonInteractable(selectedList);
    }

    
    public void HideSelectionPanel()
    {
        
        if (selectionPanel != null && selectionPanel.transform.parent != null)
        {
            var bottomPanelImg = selectionPanel.transform.parent.GetComponent<UnityEngine.UI.Image>();
            if (bottomPanelImg != null) bottomPanelImg.enabled = false;
        }

        
        foreach (var go in activeGroupPortraits)
        {
            if (go != null) Destroy(go);
        }
        activeGroupPortraits.Clear();

        activeSelectedTownCenter = null;
        activeSelectedBarracks = null;

        if (selectionPanel != null) selectionPanel.SetActive(false);
        if (singlePortraitBackground != null) singlePortraitBackground.gameObject.SetActive(false);
        if (trainFarmerButton != null) trainFarmerButton.gameObject.SetActive(false);
        if (buildBarracksButton != null) buildBarracksButton.gameObject.SetActive(false);
        if (trainSoldierButton != null) trainSoldierButton.gameObject.SetActive(false);

        if (commandButtons != null)
        {
            foreach (var btn in commandButtons)
            {
                if (btn != null) btn.gameObject.SetActive(true);
            }
        }
        UpdateGatherButtonInteractable(null);
    }

    private void UpdateGatherButtonInteractable(System.Collections.Generic.List<RTSUnit> selectedList)
    {
        if (commandPanel == null) return;
        var gatherBtn = commandPanel.transform.Find("Button_Gather")?.GetComponent<UnityEngine.UI.Button>();
        if (gatherBtn != null)
        {
            if (selectedList == null || selectedList.Count == 0)
            {
                gatherBtn.interactable = true;
                var img = gatherBtn.GetComponent<UnityEngine.UI.Image>();
                if (img != null) img.color = Color.white;
                return;
            }

            bool hasNonFarmer = false;
            bool hasFarmer = false;
            foreach (var unit in selectedList)
            {
                if (unit != null)
                {
                    if (unit.unitType == RTSUnitType.Farmer) hasFarmer = true;
                    else hasNonFarmer = true;
                }
            }

            bool isInteractable = hasFarmer && !hasNonFarmer;
            gatherBtn.interactable = isInteractable;
            
            var imgComponent = gatherBtn.GetComponent<UnityEngine.UI.Image>();
            if (imgComponent != null)
            {
                imgComponent.color = isInteractable ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            // Find and hook up button listeners dynamically
            Button[] buttons = victoryPanel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.gameObject.name == "PlayAgainButton")
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        Time.timeScale = 1f;
                        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                    });
                }
                else if (btn.gameObject.name == "MainMenuButton")
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        Time.timeScale = 1f;
                        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                    });
                }
            }

            CanvasGroup panelCg = victoryPanel.GetComponent<CanvasGroup>();
            if (panelCg != null)
            {
                panelCg.alpha = 0f;
                StartCoroutine(FadeInCanvasGroup(panelCg, 1.2f));
            }
            StartCoroutine(PauseAfterDelay(1.5f));
            return;
        }

        
        if (GameObject.Find("VictoryPanel") != null) return;

        
        Transform canvasTransform = transform;

        GameObject dynVictoryPanel = new GameObject("VictoryPanel");
        dynVictoryPanel.transform.SetParent(canvasTransform, false);

        
        RectTransform panelRect = dynVictoryPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        
        Image bgImage = dynVictoryPanel.AddComponent<Image>();
        bgImage.color = new Color(0.04f, 0.04f, 0.06f, 0.88f); 

        
        CanvasGroup cg = dynVictoryPanel.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        
        
        StartCoroutine(FadeInCanvasGroup(cg, 1.2f));

        
        GameObject container = new GameObject("ContentContainer");
        container.transform.SetParent(dynVictoryPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(500f, 350f);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);

        
        Image containerBg = container.AddComponent<Image>();
        containerBg.color = new Color(0.12f, 0.12f, 0.16f, 0.95f);
        
        
        Outline outline = container.AddComponent<Outline>();
        outline.effectColor = new Color(0.85f, 0.65f, 0.15f, 0.8f); 
        outline.effectDistance = new Vector2(3f, 3f);

        
        GameObject titleGo = new GameObject("VictoryTitle");
        titleGo.transform.SetParent(container.transform, false);
        RectTransform titleRect = titleGo.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);
        titleRect.sizeDelta = new Vector2(450f, 60f);

        TextMeshProUGUI titleText = titleGo.AddComponent<TextMeshProUGUI>();
        titleText.text = "VICTORY";
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontSize = 48f;
        titleText.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        titleText.color = new Color(1f, 0.84f, 0f); 
        if (customGameFont != null) titleText.font = customGameFont;

        
        StartCoroutine(PulseTitleText(titleRect));

        
        GameObject subtitleGo = new GameObject("VictorySubtitle");
        subtitleGo.transform.SetParent(container.transform, false);
        RectTransform subtitleRect = subtitleGo.AddComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleRect.pivot = new Vector2(0.5f, 0.5f);
        subtitleRect.anchoredPosition = new Vector2(0f, 10f);
        subtitleRect.sizeDelta = new Vector2(400f, 60f);

        TextMeshProUGUI subtitleText = subtitleGo.AddComponent<TextMeshProUGUI>();
        subtitleText.text = "Thank you for playing!";
        subtitleText.alignment = TextAlignmentOptions.Center;
        subtitleText.fontSize = 18f;
        subtitleText.color = new Color(0.9f, 0.9f, 0.95f, 0.9f); 
        if (customGameFont != null) subtitleText.font = customGameFont;

        
        GameObject btnContainer = new GameObject("Buttons");
        btnContainer.transform.SetParent(container.transform, false);
        RectTransform btnContainerRect = btnContainer.AddComponent<RectTransform>();
        btnContainerRect.anchorMin = new Vector2(0.5f, 0f);
        btnContainerRect.anchorMax = new Vector2(0.5f, 0f);
        btnContainerRect.pivot = new Vector2(0.5f, 0f);
        btnContainerRect.anchoredPosition = new Vector2(0f, 40f);
        btnContainerRect.sizeDelta = new Vector2(400f, 50f);

        
        GameObject playAgainGo = new GameObject("PlayAgainButton");
        playAgainGo.transform.SetParent(btnContainer.transform, false);
        RectTransform playAgainRect = playAgainGo.AddComponent<RectTransform>();
        playAgainRect.sizeDelta = new Vector2(160f, 45f);
        playAgainRect.anchorMin = new Vector2(0.25f, 0.5f);
        playAgainRect.anchorMax = new Vector2(0.25f, 0.5f);
        playAgainRect.pivot = new Vector2(0.5f, 0.5f);

        Image playAgainImg = playAgainGo.AddComponent<Image>();
        playAgainImg.color = new Color(0.15f, 0.45f, 0.2f, 1f); 
        Button playAgainBtn = playAgainGo.AddComponent<Button>();
        playAgainBtn.onClick.AddListener(() => {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        });

        GameObject playAgainTxtGo = new GameObject("Text");
        playAgainTxtGo.transform.SetParent(playAgainGo.transform, false);
        RectTransform playAgainTxtRect = playAgainTxtGo.AddComponent<RectTransform>();
        playAgainTxtRect.anchorMin = Vector2.zero;
        playAgainTxtRect.anchorMax = Vector2.one;
        playAgainTxtRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI playAgainText = playAgainTxtGo.AddComponent<TextMeshProUGUI>();
        playAgainText.text = "PLAY AGAIN";
        playAgainText.alignment = TextAlignmentOptions.Center;
        playAgainText.fontSize = 16f;
        playAgainText.fontStyle = FontStyles.Bold;
        playAgainText.color = Color.white;
        if (customGameFont != null) playAgainText.font = customGameFont;

        
        GameObject mainMenuGo = new GameObject("MainMenuButton");
        mainMenuGo.transform.SetParent(btnContainer.transform, false);
        RectTransform mainMenuRect = mainMenuGo.AddComponent<RectTransform>();
        mainMenuRect.sizeDelta = new Vector2(160f, 45f);
        mainMenuRect.anchorMin = new Vector2(0.75f, 0.5f);
        mainMenuRect.anchorMax = new Vector2(0.75f, 0.5f);
        mainMenuRect.pivot = new Vector2(0.5f, 0.5f);

        Image mainMenuImg = mainMenuGo.AddComponent<Image>();
        mainMenuImg.color = new Color(0.25f, 0.25f, 0.3f, 1f); 
        Button mainMenuBtn = mainMenuGo.AddComponent<Button>();
        mainMenuBtn.onClick.AddListener(() => {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });

        GameObject mainMenuTxtGo = new GameObject("Text");
        mainMenuTxtGo.transform.SetParent(mainMenuGo.transform, false);
        RectTransform mainMenuTxtRect = mainMenuTxtGo.AddComponent<RectTransform>();
        mainMenuTxtRect.anchorMin = Vector2.zero;
        mainMenuTxtRect.anchorMax = Vector2.one;
        mainMenuTxtRect.sizeDelta = Vector2.zero;
        TextMeshProUGUI mainMenuText = mainMenuTxtGo.AddComponent<TextMeshProUGUI>();
        mainMenuText.text = "MAIN MENU";
        mainMenuText.alignment = TextAlignmentOptions.Center;
        mainMenuText.fontSize = 16f;
        mainMenuText.fontStyle = FontStyles.Bold;
        mainMenuText.color = Color.white;
        if (customGameFont != null) mainMenuText.font = customGameFont;

        
        StartCoroutine(PauseAfterDelay(1.5f));
    }

    private System.Collections.IEnumerator FadeInCanvasGroup(CanvasGroup cg, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    private System.Collections.IEnumerator PulseTitleText(RectTransform rect)
    {
        Vector3 origScale = Vector3.one;
        while (true)
        {
            float wave = Mathf.PingPong(Time.unscaledTime * 2f, 0.1f);
            rect.localScale = origScale + new Vector3(wave, wave, wave);
            yield return null;
        }
    }

    private System.Collections.IEnumerator PauseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;
    }
}
