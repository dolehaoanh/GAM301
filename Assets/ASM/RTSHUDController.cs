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
        if (selectedUnitPortrait != null)
        {
            originalPortraitTexture = selectedUnitPortrait.texture;
        }

        // Tự động cập nhật dân số động và tài nguyên lặp đi lặp lại mỗi 0.5 giây (Tối ưu hóa hiệu năng, tránh giật lag)
        InvokeRepeating(nameof(UpdateDynamicPopulation), 0f, 0.5f);

        // Đăng ký lắng nghe sự kiện thay đổi tài nguyên
        PlayerResourceManager.OnResourcesChanged += UpdateHUDResources;
        
        HideSelectionPanel();
    }

    private void OnDestroy()
    {
        PlayerResourceManager.OnResourcesChanged -= UpdateHUDResources;
    }

    private void UpdateHUDResources()
    {
        if (PlayerResourceManager.Instance == null) return;

        // Đếm tổng số quân lính đang tồn tại thực tế trên bản đồ
        RTSUnit[] allUnits = FindObjectsOfType<RTSUnit>();
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

    // Hàm hiển thị thông tin chi tiết khi quét trúng một quân lính bất kỳ
    public void ShowUnitSelection(Sprite portrait, string unitName, float currentHP, float maxHP, RTSUnitType unitType)
    {
        if (selectionPanel != null) selectionPanel.SetActive(true);
        
        if (selectedUnitPortrait != null)
        {
            if (portrait != null)
            {
                selectedUnitPortrait.texture = portrait.texture;
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

        // Tự động bật khuôn mặt 3D của loại quân được chọn trong Portrait Room
        if (portraitFarmerInstance != null) portraitFarmerInstance.SetActive(unitType == RTSUnitType.Farmer);
        if (portraitSoldierInstance != null) portraitSoldierInstance.SetActive(unitType == RTSUnitType.Soldier);

        if (selectedUnitName != null) selectedUnitName.text = unitName;
        
        if (selectedUnitHPBar != null)
        {
            selectedUnitHPBar.maxValue = maxHP;
            selectedUnitHPBar.value = currentHP;
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
