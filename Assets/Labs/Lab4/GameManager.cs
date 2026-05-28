using UnityEngine;
using UnityEngine.UI; // Required for Slider!

public class GameManager : MonoBehaviour
{
    // Instance Singleton để bất kỳ mã nguồn nào cũng có thể truy cập GameManager dễ dàng
    public static GameManager Instance;

    [Header("Chỉ số Người chơi")]
    public int playerHP = 6;

    [Header("Tham chiếu Giao diện (UI)")]
    public Slider hpSlider;          // Kéo Slider HP của bạn vào đây
    public GameObject gameOverUI;    // Kéo Text Game Over của bạn vào đây

    void Awake()
    {
        // Thiết lập mẫu thiết kế Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Đảm bảo thời gian trong game chạy bình thường khi bắt đầu trò chơi
        Time.timeScale = 1f;
    }

    void Start()
    {
        // Khởi tạo thanh máu (Health Bar)
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = 6;
            hpSlider.value = playerHP;

            // Tự động căn chỉnh RectTransform của phần Fill Area và Fill để lấp đầy 100% khi không có Handle
            if (hpSlider.fillRect != null)
            {
                RectTransform fillArea = hpSlider.fillRect.parent as RectTransform;
                if (fillArea != null)
                {
                    fillArea.offsetMin = new Vector2(0f, fillArea.offsetMin.y);
                    fillArea.offsetMax = new Vector2(0f, fillArea.offsetMax.y);
                }
                hpSlider.fillRect.offsetMin = new Vector2(0f, hpSlider.fillRect.offsetMin.y);
                hpSlider.fillRect.offsetMax = new Vector2(0f, hpSlider.fillRect.offsetMax.y);
            }
        }

        // Đảm bảo ẩn giao diện Game Over khi bắt đầu
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    // Được gọi bất cứ khi nào quái vật đi vào cổng
    public void TakeDamage()
    {
        if (playerHP <= 0) return; // Already game over

        playerHP -= 1;

        // Cập nhật thanh máu trực quan
        if (hpSlider != null)
        {
            hpSlider.value = playerHP;
        }

        // Kiểm tra Game Over
        if (playerHP <= 0)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        // 1. Hiển thị giao diện Game Over
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. Đóng băng toàn bộ vật lý và hoạt ảnh trong game
        Time.timeScale = 0f;
    }
}