# Lời giải Lab 6 - GAM301

## Bài tập 1: Thiết kế rừng cây với núi tuyết
- **Cơ chế**: Sử dụng thuật toán Perlin Noise nhấp nhô tự nhiên kết hợp với chiều cao gốc.
- **Mã nguồn**: Tích hợp trong `Lab6Terrain.cs` thông qua hàm `AddPerlinNoise` để cộng dồn nhiễu tần số cao lên nền địa hình sẵn có.

## Bài tập 2: Thiết kế rừng cây có độ dốc
- **Cơ chế**: Kết hợp hàm dốc tuyến tính (Linear Gradient) theo trục X với Perlin Noise để tạo một dốc nghiêng 1 chiều nhấp nhô lồi lõm tự nhiên.
- **Mã nguồn**: Tích hợp trong `Lab6Terrain.cs` thông qua hàm `SetLinearGradientWithPerlin`.
