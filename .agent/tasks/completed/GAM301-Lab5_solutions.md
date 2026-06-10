# Lời giải Lab 5 - GAM301

## Bài tập 1: Hiệu ứng Năng lượng cho đạn bắn

### 1. Cấu hình Trail Renderer trên Bullet Prefab
- **Component**: Gắn **Trail Renderer** vào `BulletPrefab` thông qua Inspector.
- **Time**: Thiết lập thời gian vệt sáng tồn tại khoảng `0.3` - `0.5` giây.
- **Width**: Đặt `Start Width` là `0.15` - `0.2` và kéo điểm cuối đồ thị về `0` (`End Width`) để tạo đuôi nhọn.
- **Color**: Đặt màu Gradient chuyển dần từ màu sáng đậm (như xanh neon) sang mờ dần hoặc trong suốt ở đuôi.
- **Material**: Gán `BulletTrailMaterial` đã tạo vào ô **Material**.

### 2. Cấu trúc Shader Graph (`BulletTrailShader`)
- **Loại Shader**: URP > Unlit Shader Graph.
- **Graph Settings**:
  - **Surface Type**: Transparent
  - **Blending Mode**: Additive
- **Các thuộc tính (Properties)**:
  - `Trail Color` (Color - HDR Mode)
  - `Emission Intensity` (Float - Default: `2.0`)
- **Cơ chế Node**:
  - Kết nối `Trail Color` và `Emission Intensity` qua một **Multiply Node**.
  - Kết nối kết quả của phép nhân trên với node **Vertex Color** qua một **Multiply Node** thứ hai (giúp nhận màu động từ Trail Renderer).
  - Nối cổng **Out(4)** của phép nhân thứ hai trực tiếp vào cổng **Base Color(3)** của Fragment.
  - Sử dụng một **Split Node**, nối cổng **Out(4)** vào cổng **In** của Split, rồi lấy cổng **A(1)** (Alpha) nối vào cổng **Alpha(1)** của Fragment để tạo hiệu ứng mờ dần ở đuôi.

---

## Bài tập 2: Hiệu ứng Nổ khi Đạn Va Chạm

### 1. Thiết lập VFX Graph (`ExplosionVFX`)
- **Spawn System**: Sử dụng khối **Single Burst** thay vì Constant Spawn, thiết lập `Count` là `100` và `Delay` là `0`.
- **Initialize**: Sử dụng khối **Set Position Shape (Sphere)** với `Shape: Sphere`, `Position Mode: Surface`, `Spawn Mode: Random`.
- **Output**: Sử dụng khối **Set Color By Speed** với `Sample Mode: By Speed`, `Color Mode: Color And Alpha`, `Speed Range: x: 0, y: 1` cùng dải màu từ vàng sáng phát sáng sang cam/tối dần và trong suốt ở đuôi.

### 2. Cấu trúc Script điều khiển va chạm (`BulletCollision.cs`)
```csharp
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public GameObject explosionEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (explosionEffect != null)
            {
                GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            Destroy(gameObject);
        }
    }
}
```

---

## Bài tập 3: Tạo hiệu ứng tuyết rơi cho Game

### 1. Thiết lập VFX Graph (`SnowVFX`)
- **Spawn**: Thiết lập **Constant Spawn Rate** với `Rate` là `300` hạt/giây.
- **Initialize**:
  - **Capacity**: Chỉnh thành `3000`.
  - **Position (AABB)** (hoặc `Set Position Shape Box`): Thiết lập `Size X: 30, Y: 1, Z: 30` và `Center X: 0, Y: 15, Z: 0` để tuyết rơi bao phủ bản đồ từ trên cao.
  - **Set Lifetime Random**: `Min` = `5`, `Max` = `8` giây.
  - **Set Velocity Random**: `Min: X: -0.5, Y: -3.0, Z: -0.5`, `Max: X: 0.5, Y: -1.5, Z: 0.5`.
  - **Set Size**: `Size` = `0.08`.
- **Update**: Thêm khối **Turbulence** với `Intensity` là `0.8` để tạo độ chao liệng ngẫu nhiên cho bông tuyết rơi tự nhiên.
- **Output**: Sử dụng **Output Particle Unlit Quad**, kết nối với khối **Set Color Over Life** với dải màu trắng có Alpha mờ dần ở đầu/cuối đời bông tuyết để nó biến mất mượt mà.

---

## Bài tập 4: Tạo hiệu ứng Sticker hay Decal

### 1. Thiết lập URP Renderer Features
- Mở `Mobile_Renderer` và `PC_Renderer` trong thư mục cài đặt Settings.
- Nhấp **Add Renderer Feature** > chọn **Decal** trên cả hai tệp để kích hoạt tính năng chiếu Decal trong URP.

### 2. Tạo Material và Projector
- **Decal Material**: Tạo Material mới tên `FPTLogoMaterial`, đổi Shader sang `Universal Render Pipeline > Decal` và gán ảnh logo FPT vào thuộc tính `Base Map`.
- **Decal Projector**: Tạo một Empty GameObject đặt tên `FPTDecal`, gắn component **URP Decal Projector**. Gán `FPTLogoMaterial` vào ô Material, xoay mũi tên màu xanh lam (Trục Z) đâm thẳng góc vào mặt tường/hộp Cube và kéo sát hộp bao quanh chạm vào bề mặt để hiển thị.
