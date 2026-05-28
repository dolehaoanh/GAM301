# Lời giải Lab 3 - GAM301

## Bài tập 1: Tạo nhân vật di chuyển

### 1. Mô tả giải pháp
Sử dụng công cụ **Timeline** kết hợp với **Animator** để tự động di chuyển nhân vật (khối lập phương `Player`) khi scene bắt đầu.

### 2. Các bước cài đặt chi tiết
1.  **Tạo nhân vật:** Tạo một đối tượng 3D **Cube** trong Hierarchy, đặt tên là **`Player`**, reset Transform.
2.  **Tạo Animation:** 
    *   Mở cửa sổ **Animation** (Ctrl+6 / Cmd+6), chọn `Player` và nhấn **Create** để tạo file `WalkAnimation.anim`.
    *   Nhấn nút **Record (đỏ 🔴)**, ghi lại tọa độ xuất phát ở giây `0:00` và tọa độ tiến lên theo trục Z ở giây `1:00`. Tắt ghi hình.
    *   *Lưu ý:* Thao tác này cũng tự động tạo một component **Animator** trên Player.
3.  **Tạo Timeline:**
    *   Tạo một **Timeline Asset** đặt tên là `Lab3Timeline`.
    *   Tạo một Empty GameObject đặt tên là `TimelineManager`, thêm component **Playable Director** và gán `Lab3Timeline` vào ô Playable.
4.  **Cấu hình Animation Track:**
    *   Mở cửa sổ Timeline, chọn `TimelineManager`.
    *   Thêm một **Animation Track**, kéo đối tượng `Player` từ Hierarchy vào đầu track để liên kết.
    *   Kéo file `WalkAnimation.anim` từ Project thả vào timeline track này.
5.  **Tự động phát:** Bật thuộc tính **Play On Awake** trên component Playable Director.

---

## Bài tập 2: Cutscene camera chuyển cảnh

### 1. Mô tả giải pháp
Sử dụng **Cinemachine v3** tích hợp với **Timeline** để chuyển đổi và blend mượt mà giữa 3 góc quay camera ảo độc lập, đồng thời tự động dõi mắt theo Player khi di chuyển.

### 2. Các bước cài đặt chi tiết
1.  **Thiết lập Main Camera:** Thêm component **`Cinemachine Brain`** vào Main Camera để biến nó thành camera vật lý chính chịu sự quản lý của Cinemachine.
2.  **Tạo 3 Camera ảo độc lập (Cinemachine Camera):**
    *   Tạo camera 1: **`VCam_Wide`** (Đặt vị trí cao `Y: 6`, lùi xa `Z: -10`, cúi góc `X: 25`, tiêu cự FOV `60` để lấy toàn cảnh rộng).
    *   Tạo camera 2: **`VCam_CloseUp`** (Đặt rất gần Player `X: -2, Y: 1, Z: -2`, hướng nhìn hất lên `X: -10, Y: 45`, tiêu cự FOV `25` hoặc `30` để zoom cận cảnh cực kỳ ấn tượng).
    *   Tạo camera 3: **`VCam_Side`** (Đặt góc hông phải `X: 8, Y: 3, Z: 3`, xoay ngang `X: 15, Y: -90`, FOV `45` để bắt trọn chiều di chuyển).
3.  **Cài đặt dõi mắt theo mục tiêu (Tracking Target):**
    *   Tại mỗi VCam, kéo đối tượng **`Player`** vào ô **`Tracking Target`**.
    *   Dưới mục **`Procedural Components`**, đổi thiết lập **`Rotation Control`** từ *None* sang **`Rotation Composer`**. Điều này giúp camera tự xoay nhìn theo Player một cách êm ái khi nhân vật di chuyển.
4.  **Thiết lập Timeline & Cinemachine Track:**
    *   Tạo một Timeline mới hoặc dùng Timeline hiện tại của `CameraController`.
    *   Thêm một **Cinemachine Track**, kéo thả **`Main Camera`** (đối tượng chứa component *Cinemachine Brain*) vào đầu track để liên kết.
    *   Nhấp chuột phải trên track chọn **Add Cinemachine Shot Clip** 3 lần. Lần lượt kéo 3 đối tượng camera ảo `VCam_Wide`, `VCam_CloseUp`, và `VCam_Side` từ Hierarchy vào ô **`Virtual Camera`** tương ứng trong Inspector của từng clip.
5.  **Hiệu ứng Blend mượt mà:**
    *   Kéo các clip camera ảo đè gối lên nhau khoảng 1 giây (ví dụ: VCam 2 đè lên đuôi VCam 1, VCam 3 đè lên đuôi VCam 2). Một vệt giao nhau hình chữ X sẽ xuất hiện biểu thị quá trình chuyển tiếp góc quay mềm mại.
