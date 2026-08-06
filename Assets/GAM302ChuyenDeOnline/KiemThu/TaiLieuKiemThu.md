# TÀI LIỆU KIỂM THỬ GAME (GAM301 / GAM302)

---

## CÂU 1: TOÀN BỘ TESTCASE ĐẦY ĐỦ CHO CHỨC NĂNG NHẢY (2D PLATFORMER JUMP)

| ID | Loại Testcase | Mô tả | Điều kiện đầu vào | Kết quả mong đợi |
| :--- | :--- | :--- | :--- | :--- |
| **TC_JUMP_01** | Positive Case | Kiểm tra nhảy khi nhân vật đứng yên trên mặt đất phẳng | Nhân vật đứng cố định trên mặt đất (`isGrounded = true`, `velocity.y = 0`). Nhấn phím `Space`. | Nhân vật nhảy lên đúng độ cao cố định đã thiết lập. Trạng thái chuyển sang `isGrounded = false`. |
| **TC_JUMP_02** | Positive Case | Kiểm tra nhảy khi vừa di chuyển vừa ở trên mặt đất | Nhân vật đang di chuyển ngang (nhấn `A`/`D` hoặc `Left`/`Right`, `isGrounded = true`). Nhấn phím `Space`. | Nhân vật thực hiện cú nhảy lên độ cao cố định kết hợp với đà di chuyển ngang. |
| **TC_JUMP_03** | Positive Case | Kiểm tra nhảy khi đang ở trên bề mặt dốc lên (Upward Slope) | Nhân vật đứng/di chuyển trên bề mặt dốc nghiêng hướng lên (`isGrounded = true`). Nhấn phím `Space`. | Nhân vật thực hiện cú nhảy theo phương thẳng đứng đạt độ cao cố định bình thường. |
| **TC_JUMP_04** | Positive Case | Kiểm tra nhảy khi đang ở trên bề mặt dốc xuống (Downward Slope) | Nhân vật đứng/di chuyển trên bề mặt dốc nghiêng hướng xuống (`isGrounded = true`). Nhấn phím `Space`. | Nhân vật nhảy đạt độ cao cố định mà không bị kẹt mặt đất. |
| **TC_JUMP_05** | Positive Case | Kiểm tra nhảy khi đang đứng trên sàn di chuyển (Moving Platform) | Nhân vật đứng trên sàn di chuyển ngang hoặc thẳng đứng (`isGrounded = true`). Nhấn phím `Space`. | Nhân vật thực hiện cú nhảy đạt độ cao cố định và thừa hưởng vận tốc của sàn di chuyển. |
| **TC_JUMP_06** | Positive Case | Kiểm tra nhảy khi đứng trên vật thể vật lý động (Crate/Pushable Box) | Nhân vật đứng trên một khối hộp vật lý có thể đẩy (`isGrounded = true`). Nhấn phím `Space`. | Nhân vật nhảy bình thường, khối hộp chịu lực phản hồi ngược xuống dưới. |
| **TC_JUMP_07** | Negative Case | Kiểm tra không thể nhảy khi đang ở giai đoạn bay lên (Ascending Phase) | Nhân vật đã nhảy và đang ở giai đoạn bay lên (`isGrounded = false`, `velocity.y > 0`). Nhấn phím `Space`. | Phím `Space` không có tác dụng, nhân vật không nhảy thêm lần nữa. |
| **TC_JUMP_08** | Negative Case | Kiểm tra không thể nhảy khi ở đỉnh cú nhảy (Jump Apex) | Nhân vật đang ở điểm cao nhất của cú nhảy (`isGrounded = false`, `velocity.y ≈ 0`). Nhấn phím `Space`. | Phím `Space` không có tác dụng, nhân vật bắt đầu rơi xuống theo trọng lực. |
| **TC_JUMP_09** | Negative Case | Kiểm tra không thể nhảy khi đang ở giai đoạn rơi xuống (Falling Phase) | Nhân vật đang rơi từ trên cao xuống (`isGrounded = false`, `velocity.y < 0`). Nhấn phím `Space`. | Phím `Space` không có tác dụng, nhân vật tiếp tục rơi bình thường. |
| **TC_JUMP_10** | Negative Case | Kiểm tra không thể nhảy khi trượt rơi khỏi mép vực (Off-edge Fall) | Nhân vật đi ra khỏi mép nền tảng và tự rơi xuống mà không bấm nhảy (`isGrounded = false`). Nhấn phím `Space`. | Nhân vật không nhảy, tiếp tục rơi tự do. |
| **TC_JUMP_11** | Negative Case | Kiểm tra không thể nhảy khi bị hất văng lên không bởi kẻ thù (Knockback) | Nhân vật trúng đòn bị hất văng vào không trung (`isGrounded = false`). Nhấn phím `Space`. | Phím `Space` bị vô hiệu hóa, nhân vật không thể nhảy trong trạng thái bị trúng đòn. |
| **TC_JUMP_12** | Negative Case | Kiểm tra nhấn và giữ phím Space sau khi đã nhảy (Hold Space) | Nhân vật vừa nhảy khỏi mặt đất (`isGrounded = false`). Người chơi nhấn và tiếp tục giữ nguyên phím `Space`. | Nhân vật chỉ nhảy đúng độ cao cố định 1 lần duy nhất rồi rơi xuống, không bay tiếp hay tự nhảy lại. |
| **TC_JUMP_13** | Negative Case | Kiểm tra nhấp phím Space liên tục (Spam Space) khi ở trên không | Nhân vật đang ở trên không (`isGrounded = false`). Nhấn `Space` liên tục nhiều lần với tốc độ cao. | Tất cả các lần nhấn phím trên không đều bị bỏ qua cho đến khi nhân vật tiếp đất trở lại. |
| **TC_JUMP_14** | Positive Case | Kiểm tra nhấp phím Space liên tục khi tiếp đất | Nhân vật chạm đất trở lại (`isGrounded` chuyển sang `true`). Người chơi liên tục nhấp `Space`. | Nhân vật thực hiện cú nhảy mới ngay khi vừa tiếp đất. |
| **TC_JUMP_15** | Positive Case | Kiểm tra nhấn kết hợp phím Space + Phím di chuyển Trái/Phải cùng lúc | Nhân vật trên mặt đất. Nhấn đồng thời `Space` + `A` (hoặc `Space` + `Left Arrow`). | Nhân vật vừa nhảy lên vừa di chuyển sang trái. |
| **TC_JUMP_16** | Positive Case | Kiểm tra nhấn phím Space khi đang giữ phím Ngồi (Crouch + Space) | Nhân vật đang ở trạng thái ngồi trên mặt đất. Nhấn phím `Space`. | Nhân vật đứng dậy và thực hiện cú nhảy (hoặc không nhảy theo đúng thiết kế game). |
| **TC_JUMP_17** | Negative Case | Kiểm tra nhảy bên dưới trần nhà thấp (Ceiling Collision) | Nhân vật đứng bên dưới trần nhà thấp có Collider. Nhấn phím `Space`. | Đầu nhân vật va chạm với trần nhà, độ cao cú nhảy bị chặn lại và nhân vật bị đẩy rơi xuống ngay lập tức. |
| **TC_JUMP_18** | Positive Case | Kiểm tra nhảy khi đang áp sát bức tường thẳng đứng (Wall Touch) | Nhân vật đang đứng sát tường đứng (`isGrounded = true`). Nhấn phím `Space`. | Nhân vật nhảy thẳng lên theo chiều dọc mà không bị dính chặt vào tường. |
| **TC_JUMP_19** | Positive Case | Kiểm tra nhảy ngay rìa mép bề mặt (Edge Jump / Boundary Check) | Nhân vật đứng ở vị trí sát ranh giới mép nền tảng (Ground Check ở ranh giới). Nhấn phím `Space`. | Nhảy thành công nếu hệ thống Ground Check còn va chạm với mặt đất. |
| **TC_JUMP_20** | Negative Case | Kiểm tra nhảy khi đang đứng trên bẫy chông/gai (Hazard Ground) | Nhân vật chạm bề mặt gây sát thương/chết. Nhấn phím `Space`. | Hệ thống xử lý mất máu/chết trước hoặc không cho phép nhảy tùy theo quy tắc thiết kế. |
| **TC_JUMP_21** | Negative Case | Kiểm tra nhấn phím Space khi nhân vật bị Choáng/Băng giá (Stunned/Frozen) | Nhân vật đang ở trạng thái Choáng hoặc Đóng băng trên mặt đất. Nhấn phím `Space`. | Phím `Space` bị vô hiệu hóa, nhân vật giữ nguyên trạng thái choáng/băng. |
| **TC_JUMP_22** | Negative Case | Kiểm tra nhấn phím Space khi nhân vật hết máu / Chết (Dead State) | Nhân vật đã hết HP (lượng máu = 0) và phát hoạt ảnh chết. Nhấn phím `Space`. | Nhân vật không thực hiện thao tác nhảy. |
| **TC_JUMP_23** | Negative Case | Kiểm tra nhấn phím Space khi Game đang Tạm dừng (Pause Menu Active) | Game đang ở menu Pause (Time.timeScale = 0). Nhấn phím `Space`. | Nhân vật không nhảy trong background. |
| **TC_JUMP_24** | Negative Case | Kiểm tra nhấn phím Space khi đang trong đoạn phim cắt cảnh (Cutscene) | Game đang phát Cutscene/Hội thoại khóa điều khiển. Nhấn phím `Space`. | Thao tác nhảy bị vô hiệu hóa hoàn toàn. |

---

## CÂU 2: TEST PLAN CHO GAME BẮN SÚNG FPS ĐƠN GIẢN

### 1. Mục tiêu kiểm thử (Test Objectives)
- Đảm bảo các tính năng cốt lõi (Di chuyển, Bắn súng, Nạp đạn) hoạt động chính xác theo yêu cầu thiết kế.
- Đảm bảo trải nghiệm điều khiển mượt mà, phản hồi phím bấm chính xác và hệ thống tính đạn/gây sát thương không xảy ra lỗi logic.
- Phát hiện và loại bỏ các lỗi nghiêm trọng (crash, kẹt nhân vật, đạn vô hạn) trước khi phát hành.

### 2. Phạm vi kiểm thử (Test Scope)
- **Trong phạm vi (In-Scope):**
  - **Di chuyển nhân vật:** Đi tiến, lùi, trái, phải, xoay góc nhìn camera FPS, va chạm với vật cản/tường.
  - **Bắn súng:** Bắn đơn, bắn liên tục, giảm số lượng đạn trong băng, tia đạn (Raycast) chạm mục tiêu và gây sát thương.
  - **Nạp đạn:** Nạp đạn khi hết đạn trong băng, nạp đạn chủ động (phím `R`), cập nhật số lượng đạn dự trữ và băng đạn.
- **Ngoại phạm vi (Out-of-Scope):**
  - Tính năng chơi nhiều người (Multiplayer / Kết nối mạng).
  - Hệ thống mua sắm trang phục, giao diện shop.
  - Đồ họa nâng cao, hiệu ứng âm thanh vòm phức tạp.

### 3. Tài nguyên cần thiết (Resources)
- **Nhân sự:**
  - 01 QA Lead: Lập kế hoạch, theo dõi tiến độ và kiểm duyệt lỗi.
  - 02 QA Tester: Thực thi testcase thủ công, viết kịch bản test tự động và báo cáo lỗi.
- **Công cụ (Tools):**
  - Unity Editor & Unity Test Framework (Edit Mode & Play Mode).
  - JIRA / Trello (Quản lý và theo dõi lỗi).
  - Git / GitHub (Quản lý mã nguồn dự án test).
  - Máy tính thử nghiệm đáp ứng cấu hình tối thiểu của game.

### 4. Phương pháp kiểm thử (Test Methodology)
- **Manual Testing (Kiểm thử thủ công):**
  - Functional Testing: Kiểm tra từng tính năng di chuyển, bắn súng, nạp đạn theo kịch bản.
  - Boundary & Edge Cases: Kiểm thử các trường hợp biên như nhấn nạp đạn khi đạn đã đầy, bắn khi đạn bằng 0, di chuyển vào các mép bản đồ.
  - Exploratory Testing: Kiểm thử khám phá nhằm tìm kiếm lỗi phát sinh không ngờ tới.
- **Automated Testing (Kiểm thử tự động):**
  - Edit Mode Testing: Kiểm tra logic tính toán số lượng đạn, lượng sát thương trong code.
  - Play Mode Testing: Kiểm tra luồng tương tác giữa nhân vật, vũ khí và kẻ thù trong Scene.

### 5. Tiêu chí hoàn thành (Exit Criteria)
- 100% các testcase thuộc phạm vi đã được thực thi.
- Không còn lỗi ở mức độ **Critical / Blocker** và **High**.
- Các lỗi ở mức **Medium / Low** đã được khắc phục hoặc được phê duyệt hoãn xử lý cho phiên bản tiếp theo.
- Toàn bộ các bài kiểm thử tự động (Edit Mode & Play Mode Tests) chạy qua thành công (100% Pass).

---

## CÂU 3: AUTO TEST VỚI UNITY TEST FRAMEWORK

### 1. Mã nguồn các Script cần kiểm thử (Source Code)

#### Script Hệ thống chiến đấu (`CombatSystem.cs`):
```csharp
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public int CalculateDamage(int baseDamage, float multiplier)
    {
        return Mathf.FloorToInt(baseDamage * multiplier);
    }
}
```

#### Script Kẻ thù (`KeThu.cs`):
```csharp
using UnityEngine;

public class KeThu : MonoBehaviour
{
    public int luongMau = 100;

    public void nhanSatThuong(int satThuong)
    {
        luongMau -= satThuong;
    }
}
```

---

### 2. Edit Mode Test (`KiemThuEditModeChienDau.cs`)

**Mô tả:**
Edit Mode Test cho phép kiểm thử trực tiếp hàm tính toán `CalculateDamage` trong môi trường Unity Editor mà không cần khởi chạy Play Mode. Bài test thực hiện 2 trường hợp:
- `baseDamage = 10`, `multiplier = 1.5f` -> Kết quả mong đợi = 15.
- `baseDamage = 5`, `multiplier = 2.0f` -> Kết quả mong đợi = 10.

**Mã nguồn Code Test:**
```csharp
using NUnit.Framework;
using UnityEngine;

public class KiemThuEditModeChienDau
{
    [Test]
    public void KiemTraSatThuongTruongHopMot()
    {
        GameObject doiTuong = new GameObject();
        CombatSystem heThongChienDau = doiTuong.AddComponent<CombatSystem>();
        int satThuongCoBan = 10;
        float heSoNhan = 1.5f;
        int ketQuaMongDoi = 15;

        int ketQuaThucTe = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        Assert.AreEqual(ketQuaMongDoi, ketQuaThucTe);
        Object.DestroyImmediate(doiTuong);
    }

    [Test]
    public void KiemTraSatThuongTruongHopHai()
    {
        GameObject doiTuong = new GameObject();
        CombatSystem heThongChienDau = doiTuong.AddComponent<CombatSystem>();
        int satThuongCoBan = 5;
        float heSoNhan = 2.0f;
        int ketQuaMongDoi = 10;

        int ketQuaThucTe = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        Assert.AreEqual(ketQuaMongDoi, ketQuaThucTe);
        Object.DestroyImmediate(doiTuong);
    }
}
```

---

### 3. Play Mode Test (`KiemThuPlayModeChienDau.cs`)

**Mô tả cách thiết lập Scene:**
1. Bài test Play Mode sử dụng `IEnumerator` và thuộc tính `[UnityTest]` để kiểm tra hành vi runtime.
2. Thiết lập đối tượng trong Scene bằng code:
   - Tạo GameObject tên `"KeThu"` và gắn component `KeThu` với máu ban đầu = 100.
   - Tạo GameObject tên `"HeThongChienDau"` và gắn component `CombatSystem`.
3. Cho Scene chạy 1 frame (`yield return null`).
4. Gọi hàm `CalculateDamage(20, 1.5f)` kết quả sát thương là `30`. Truyền sát thương vào hàm `nhanSatThuong` của kẻ thù.
5. So sánh lượng máu sau khi trừ (`100 - 30 = 70`) bằng `Assert.AreEqual(70, keThu.luongMau)`.

**Mã nguồn Code Test:**
```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class KiemThuPlayModeChienDau
{
    [UnityTest]
    public IEnumerator KiemTraTruLuongMauKeThu()
    {
        GameObject doiTuongKeThu = new GameObject("KeThu");
        KeThu keThu = doiTuongKeThu.AddComponent<KeThu>();
        keThu.luongMau = 100;

        GameObject doiTuongChienDau = new GameObject("HeThongChienDau");
        CombatSystem heThongChienDau = doiTuongChienDau.AddComponent<CombatSystem>();

        yield return null;

        int satThuongCoBan = 20;
        float heSoNhan = 1.5f;
        int satThuongTinhDuoc = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);
        keThu.nhanSatThuong(satThuongTinhDuoc);

        int luongMauMongDoi = 70;
        Assert.AreEqual(luongMauMongDoi, keThu.luongMau);

        Object.Destroy(doiTuongKeThu);
        Object.Destroy(doiTuongChienDau);
    }
}
```
