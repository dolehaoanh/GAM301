# TÀI LIỆU KIỂM THỬ GAME (GAM301 / GAM302)

---

## CÂU 1: TESTCASE CHO CHỨC NĂNG NHẢY (2D PLATFORMER JUMP)

| ID | Mô tả | Điều kiện đầu vào | Kết quả mong đợi |
| :--- | :--- | :--- | :--- |
| **TC_JUMP_01** | Kiểm tra nhân vật nhảy khi đang đứng yên trên mặt đất (Positive Case) | Nhân vật đang đứng cố định trên mặt đất (`isGrounded = true`, `velocity.y = 0`). Người chơi nhấn phím `Space`. | Nhân vật nhảy lên đúng độ cao cố định đã thiết lập. Trạng thái nhân vật chuyển sang ở trên không (`isGrounded = false`). |
| **TC_JUMP_02** | Kiểm tra nhân vật nhảy khi vừa di chuyển vừa ở trên mặt đất (Positive Case) | Nhân vật đang di chuyển ngang trên mặt đất (nhấn `A`/`D` hoặc `Left`/`Right`, `isGrounded = true`). Người chơi nhấn phím `Space`. | Nhân vật thực hiện cú nhảy lên độ cao cố định kết hợp với đà di chuyển ngang theo hướng đang đi. |
| **TC_JUMP_03** | Kiểm tra nhân vật không thể nhảy khi đang ở trên không (Negative Case) | Nhân vật đang ở trạng thái trên không (`isGrounded = false`). Người chơi nhấn phím `Space`. | Phím `Space` không có tác dụng. Nhân vật không nhảy thêm lần nữa và giữ nguyên quỹ đạo rơi/di chuyển hiện tại. |
| **TC_JUMP_04** | Kiểm tra nhấn và giữ phím Space sau khi đã nhảy (Negative Case) | Nhân vật vừa thực hiện cú nhảy và đang ở trên không (`isGrounded = false`). Người chơi tiếp tục nhấn giữ phím `Space`. | Nhân vật đạt độ cao tối đa cố định rồi rơi xuống theo gia tốc trọng lực, không bay tiếp hoặc nhảy liên tục. |
| **TC_JUMP_05** | Kiểm tra nhấn phím Space khi nhân vật vừa rơi khỏi mép vực (Negative Case) | Nhân vật di chuyển ra khỏi mép bề mặt và rơi xuống (`isGrounded = false`). Người chơi nhấn phím `Space`. | Nhân vật không thực hiện cú nhảy, tiếp tục rơi tự do xuống dưới. |
| **TC_JUMP_06** | Kiểm tra khả năng nhảy lại sau khi nhân vật tiếp đất trở lại (Positive Case) | Nhân vật vừa tiếp đất trở lại từ cú nhảy trước (`isGrounded` chuyển từ `false` sang `true`). Người chơi nhấn phím `Space`. | Nhân vật thực hiện cú nhảy mới đạt độ cao cố định bình thường. |

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
