# Lời giải Lab 7 - GAM301

## Bài tập 1: Áp dụng Object Pooling cho bài Lab 5
- **Cơ chế**: Thay vì liên tục gọi `Instantiate` và `Destroy` cho các vật thể có tần suất sử dụng cao như đạn (`Bullet`), ta sử dụng `UnityEngine.Pool.IObjectPool<GameObject>` để quản lý vòng đời của chúng. Đạn sau khi bắn hoặc hết thời gian sống sẽ được đưa ngược lại vào Pool thay vì hủy hoàn toàn khỏi bộ nhớ.
- **Mã nguồn**: Tích hợp thông qua lớp `ObjectPoolManager.cs` và cập nhật logic hồi trả trong `Bullet.cs`.

## Bài tập 2: Áp dụng Occlusion Culling cho bài Lab 6
- **Cơ chế**: Unity Occlusion Culling giúp ẩn các Renderer nằm phía sau chướng ngại vật hoặc nằm ngoài tầm quan sát (không được Camera ghi hình), giúp giảm số lượng Draw Calls (Batches) và khối lượng dựng hình đáng kể.
- **Cách làm**:
  - Gán nhãn các đối tượng địa hình, nhà cửa, núi đá tĩnh là **Static Occluder** và **Static Occludee**.
  - Mở cửa sổ **Occlusion Culling** (Window > Rendering > Occlusion Culling) và nhấn **Bake**.

## Bài 3: Áp dụng Texture Compression
- **Cơ chế**: Nén các hình ảnh, bản đồ vân bề mặt (Texture) để giảm thiểu kích thước lưu trữ trên ổ đĩa và bộ nhớ GPU.
- **Cách làm**:
  - Chọn file Texture trong thư mục `Assets`.
  - Cấu hình các tùy chọn trong thẻ `Inspector`:
    - **Max Size**: Điều chỉnh lại kích thước phù hợp (ví dụ: `2048`, `1024` hoặc thấp hơn).
    - **Resize Algorithm**: Chọn `Mitchell` hoặc `Bilinear`.
    - **Compression**: Chọn chất lượng nén `Normal Quality` hoặc `High Quality`.
    - Bật **Use Crunch Compression** và điều chỉnh thanh trượt **Quality** để giảm dung lượng file xuống mức tối thiểu mà vẫn đảm bảo độ nét cần thiết.

## Bài 4: Áp dụng Sound Compression
- **Cơ chế**: Nén dung lượng tệp tin âm thanh và tối ưu hóa cách thức tải dữ liệu tương ứng với đặc thù phát âm thanh của chúng.
- **Cách làm**:
  - Chọn tệp âm thanh (ví dụ: tệp `.mp3` hoặc `.wav`).
  - Chọn **Force To Mono** để giảm kênh nếu tệp không cần hiệu ứng không gian 3D phức tạp.
  - Chọn **Load Type**:
    - **Decompress On Load**: Thích hợp cho các tệp âm thanh ngắn, lặp lại nhiều lần (như tiếng súng, tiếng bước chân).
    - **Compressed In Memory**: Cho âm thanh vừa phải.
    - **Streaming**: Cho nhạc nền dài, giảm tải bộ nhớ RAM tối đa.
  - Điều chỉnh thông số **Quality** xuống khoảng 50-70 để đạt tỷ lệ nén tốt nhất mà không gây méo âm thanh.

## Bài 5: Giảng viên giao thêm bài tập
- **Kết quả**: Tất cả các yêu cầu cải tiến hiệu năng bao gồm Object Pooling, Occlusion Culling, nén Texture và Sound đã được thực hiện và nghiệm thu thành công.
