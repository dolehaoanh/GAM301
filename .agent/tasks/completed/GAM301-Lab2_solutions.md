# Lời giải Lab 2 - GAM301

## Bài tập 1: Áp dụng Interpolation cho Rigidbody

### 1. Khái niệm và Báo cáo So sánh
Khi một đối tượng di chuyển bằng Rigidbody dưới tác động của vật lý (`FixedUpdate`), tần số cập nhật vật lý mặc định của Unity là 50Hz (0.02 giây/lần), trong khi màn hình game thường có tần số quét cao hơn (60Hz, 120Hz, hoặc cao hơn). Điều này tạo ra sự lệch pha, khiến mắt người nhìn thấy chuyển động của vật thể bị giật (jitter).

*   **Trước khi áp dụng Interpolation (Interpolation = None):**
    *   Vật thể chuyển động bị rung lắc, giật nhẹ khi di chuyển nhanh, đặc biệt là khi camera đi theo đối tượng.
*   **Sau khi áp dụng Interpolation:**
    *   **Interpolate (Nội suy):** Unity sẽ làm mịn chuyển động dựa trên vị trí của vật thể ở các frame trước đó. Chuyển động cực kỳ mượt mà, nhưng sẽ có độ trễ cực kỳ nhỏ (1 frame vật lý). Thường dùng cho nhân vật chính hoặc camera đi theo nhân vật.
    *   **Extrapolate (Ngoại suy):** Unity sẽ dự đoán vị trí tiếp theo của vật thể dựa trên vận tốc hiện tại. Chuyển động mượt mà và không có độ trễ, nhưng có thể bị sai lệch nhỏ nếu vật thể đổi hướng đột ngột do va chạm. Thường dùng cho các vật thể di chuyển tuyến tính đơn giản như đạn bay, xe chạy thẳng.

### 2. Các bước thiết lập trên Unity Editor
1.  Chọn GameObject có thành phần **Rigidbody** trong cảnh.
2.  Trong bảng **Inspector**, tìm component **Rigidbody**.
3.  Tại mục **Interpolate**, đổi từ **None** sang **Interpolate** (hoặc **Extrapolate** tùy theo đặc tính chuyển động).

---

## Bài tập 2: Áp dụng FrameRate cho game

### 1. Mã nguồn hoàn chỉnh (`FrameRateController.cs`)
```csharp
using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    public int targetFPS = 120; // FPS tối đa mong muốn

    void Awake()
    {
        // 1. Tắt VSync (Đồng bộ dọc) để có thể tùy chỉnh giới hạn FPS
        QualitySettings.vSyncCount = 0;

        // 2. Thiết lập FPS mục tiêu của ứng dụng
        Application.targetFrameRate = targetFPS;

        Debug.Log($"FPS target set to: {targetFPS}");
    }
}
```

### 2. Các bước cài đặt trên Unity Editor
1.  Tạo một GameObject trống đặt tên là `GameManager`.
2.  Gắn script `FrameRateController.cs` vào `GameManager`.
3.  Chạy game và cảm nhận sự mượt mà khi FPS đạt tối đa theo cấu hình phần cứng.

---

## Bài tập 3: Áp dụng Constant Force cho game

### 1. Mã nguồn hoàn chỉnh (`PlayerSpinController.cs`)
```csharp
using UnityEngine;

public class PlayerSpinController : MonoBehaviour
{
    private ConstantForce constantForceComponent;
    public float spinTorque = 100f; // Lực quay trục Y

    void Start()
    {
        constantForceComponent = GetComponent<ConstantForce>();
        if (constantForceComponent == null)
        {
            Debug.LogWarning("Vui lòng gắn component Constant Force vào GameObject này!");
        }
    }

    // Hàm gọi khi nhân vật mất máu để bắt đầu quay tròn
    public void StartSpinning()
    {
        if (constantForceComponent != null)
        {
            // Thiết lập mô-men xoắn (torque) trên trục Y để quay tròn nhân vật liên tục
            constantForceComponent.torque = new Vector3(0, spinTorque, 0);
            Debug.Log("Player is damaged! Spin torque applied via ConstantForce.");
        }
    }
}
```

### 2. Các bước cài đặt trên Unity Editor
1.  Chọn đối tượng **Player**.
2.  Thêm component **Constant Force** và component **Rigidbody** (nếu chưa có).
3.  Gắn script `PlayerSpinController.cs` vào **Player**.
4.  Khi có sự kiện mất máu, gọi hàm `StartSpinning()` để áp dụng lực xoay liên tục trục Y từ Constant Force.

---

## Bài 4: Tìm GameObject sử dụng MeshCollider và đổi thành Compound Collider

### 1. Phương pháp tối ưu va chạm
*   **MeshCollider** là bộ va chạm khớp chính xác theo từng lưới đa giác của model 3D. Rất tốn tài nguyên xử lý vật lý (CPU), đặc biệt khi có va chạm với các MeshCollider phức tạp khác.
*   **Compound Collider (Collider Phức hợp):** Thay thế một MeshCollider phức tạp bằng cách tổ hợp nhiều Collider cơ bản đơn giản (Box Collider, Sphere Collider, Capsule Collider) xếp chồng/gắn vào các đối tượng con.

### 2. Các bước thực hiện
1.  Tìm các GameObject phức tạp sử dụng **MeshCollider** (ví dụ: mô hình lâu đài `Castle.fbx` hoặc địa hình phức tạp).
2.  Xóa component **MeshCollider** khỏi đối tượng.
3.  Tạo các GameObject con rỗng (Empty Child) nằm dưới đối tượng cha đó.
4.  Gắn các **Box Collider** hoặc **Capsule Collider** cơ bản vào các GameObject con này, điều chỉnh kích thước và vị trí của chúng sao cho bao quanh chính xác các vùng va chạm thực tế của đối tượng cha.
5.  Kết quả: Game hoạt động ổn định, va chạm chuẩn xác và tăng đáng kể hiệu năng xử lý vật lý.
