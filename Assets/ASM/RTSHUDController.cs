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

    // Lưu trữ Render Texture 3D mặc định được gán từ Inspector
    private Texture originalPortraitTexture;

    private void Start()
    {
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

    private void OnDestroy()
    {
        PlayerResourceManager.OnResourcesChanged -= UpdateHUDResources;
    }

    private void UpdateHUDResources()
    {
        if (PlayerResourceManager.Instance == null) return;

        // Đếm tổng số quân lính đang tồn tại thực tế trên bản đồ
        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsSortMode.None);
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
        if (selectedList == null || selectedList.Count == 0)
        {
            HideSelectionPanel();
            return;
        }

        if (selectionPanel != null) selectionPanel.SetActive(true);

        // Leader (Thủ lĩnh) là con quân đầu tiên được chọn
        RTSUnit leader = selectedList[0];

        if (selectedUnitPortrait != null)
        {
            if (leader.portrait != null)
            {
                selectedUnitPortrait.texture = leader.portrait.texture;
                selectedUnitPortrait.color = Color.white; // Màu gốc đầy đủ
            }
            else
            {
                // Khôi phục lại Render Texture 3D ban đầu nếu không có Sprite 2D
                selectedUnitPortrait.texture = originalPortraitTexture;
                if (originalPortraitTexture != null)
                {
                    selectedUnitPortrait.color = Color.white; // Hiện rõ nét 3D Portrait
                }
                else
                {
                    selectedUnitPortrait.texture = null;
                    // Nếu hoàn toàn không có gì, hiện khung kính mờ tối sang trọng
                    selectedUnitPortrait.color = new Color(0.12f, 0.12f, 0.18f, 0.65f);
                }
            }
        }

        // Tự động bật khuôn mặt 3D của loại quân được chọn (Thủ lĩnh) trong Portrait Room
        if (portraitFarmerInstance != null) portraitFarmerInstance.SetActive(leader.unitType == RTSUnitType.Farmer);
        if (portraitSoldierInstance != null) portraitSoldierInstance.SetActive(leader.unitType == RTSUnitType.Soldier);

        // Cập nhật tên và quân số đạo quân (Leader Portrait + Đếm số lượng)
        if (selectedUnitName != null)
        {
            if (selectedList.Count == 1)
            {
                selectedUnitName.text = leader.unitName;
            }
            else
            {
                // Đếm số lượng từng loại lớp quân
                int soldierCount = 0;
                int farmerCount = 0;
                foreach (var unit in selectedList)
                {
                    if (unit != null)
                    {
                        if (unit.unitType == RTSUnitType.Soldier) soldierCount++;
                        else farmerCount++;
                    }
                }
                // Định dạng hiển thị chuyên nghiệp: CHIẾN BINH (8 Soldiers | 4 Farmers)
                selectedUnitName.text = $"{leader.unitName} ({soldierCount} Soldiers | {farmerCount} Farmers)";
            }
        }
        
        // Cập nhật thanh HP Slider (Nếu chọn nhiều quân, hiện tổng HP tích lũy của cả đạo quân)
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

    // Hàm tự động ẩn bảng thông tin đi khi người chơi nhấp ra ngoài đất trống (bỏ chọn)
    public void HideSelectionPanel()
    {
        if (selectionPanel != null) selectionPanel.SetActive(false);
        
        // Tắt cả hai khuôn mặt chân dung 3D đi khi không chọn ai
        if (portraitFarmerInstance != null) portraitFarmerInstance.SetActive(false);
        if (portraitSoldierInstance != null) portraitSoldierInstance.SetActive(false);
    }
}
