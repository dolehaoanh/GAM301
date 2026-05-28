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
    [Tooltip("Ảnh chân dung của lính")]
    public Image selectedUnitPortrait;
    [Tooltip("Tên của lính")]
    public TextMeshProUGUI selectedUnitName;
    [Tooltip("Thanh máu của lính")]
    public Slider selectedUnitHPBar;

    [Header("Command Grid Panel References")]
    [Tooltip("Bảng Panel chứa các ô lệnh hành động (Bottom-Right)")]
    public GameObject commandPanel;
    [Tooltip("Danh sách các nút bấm lệnh (Move, Stop, Attack...)")]
    public Button[] commandButtons;

    private void Start()
    {
        // Khởi tạo các giá trị hiển thị giả lập ban đầu (chưa cần liên kết logic chiến đấu thực tế)
        UpdateResourcesDisplay(500, 300, 8, 20);
        HideSelectionPanel();
    }

    // Hàm cập nhật lượng tài nguyên hiển thị lên thanh TopBar
    public void UpdateResourcesDisplay(int gold, int wood, int currentPop, int maxPop)
    {
        if (goldText != null) goldText.text = $"VÀNG: {gold}";
        if (woodText != null) woodText.text = $"GỖ: {wood}";
        if (populationText != null) populationText.text = $"DÂN SỐ: {currentPop}/{maxPop}";
    }

    // Hàm hiển thị thông tin chi tiết khi quét trúng một quân lính bất kỳ
    public void ShowUnitSelection(Sprite portrait, string unitName, float currentHP, float maxHP)
    {
        if (selectionPanel != null) selectionPanel.SetActive(true);
        
        if (selectedUnitPortrait != null)
        {
            if (portrait != null)
            {
                selectedUnitPortrait.sprite = portrait;
                selectedUnitPortrait.color = Color.white; // Màu gốc đầy đủ
            }
            else
            {
                selectedUnitPortrait.sprite = null;
                // Nếu chưa có ảnh chân dung, hiển thị một khung màu xám tối bo góc sang trọng (phong cách Glassmorphism)
                selectedUnitPortrait.color = new Color(0.12f, 0.12f, 0.18f, 0.65f);
            }
        }

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
    }
}
