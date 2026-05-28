using UnityEngine;

public class MonsterHP : MonoBehaviour
{
    [Header("Cài đặt Máu")]
    public int currentHP;

    private int[] hpOptions = { 3, 9, 81 };

    void Start()
    {
        // 1. Chọn ngẫu nhiên một giá trị máu từ các tùy chọn: 3, 9, hoặc 81
        int randomIndex = Random.Range(0, hpOptions.Length);
        currentHP = hpOptions[randomIndex];

        // 2. Thay đổi màu sắc dựa trên lượng máu đã chọn
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (currentHP == 3)
            {
                rend.material.color = Color.green; // 3 HP = Xanh lá
            }
            else if (currentHP == 9)
            {
                rend.material.color = Color.blue;  // 9 HP = Xanh dương
            }
            else if (currentHP == 81)
            {
                rend.material.color = Color.red;   // 81 HP = Đỏ
            }
        }
    }

    // Được gọi bởi mã viên đạn để gây sát thương
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        Debug.Log($"{gameObject.name} đã nhận {damageAmount} sát thương. Máu hiện tại: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log("💀 Quái vật đã bị tiêu diệt!");
            Destroy(gameObject);
        }
    }
}