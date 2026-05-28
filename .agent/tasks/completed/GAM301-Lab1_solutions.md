# Lời giải Lab 1 - GAM301

## Bài tập 1: Sinh đối tượng sau một khoảng thời gian

### 1. Mã nguồn hoàn chỉnh (`ObjectSpawner.cs`)
```csharp
using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject prefabToSpawn; // Prefab của đối tượng muốn sinh (ví dụ: Cube)
    public float spawnInterval = 2f; // Thời gian chờ giữa mỗi lần sinh (2 giây)
    public int maxSpawns = 10;        // Số lượng tối đa là 10

    [Header("Spawn Area")]
    public Vector3 spawnRangeMin;    // Tọa độ nhỏ nhất của khu vực sinh (X, Y, Z)
    public Vector3 spawnRangeMax;    // Tọa độ lớn nhất của khu vực sinh (X, Y, Z)

    private int currentSpawnCount = 0; // Biến đếm số lượng đối tượng hiện tại

    void Start()
    {
        // Bắt đầu chạy Coroutine tên là SpawnRoutine
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Vòng lặp chạy cho đến khi sinh đủ số lượng tối đa
        while (currentSpawnCount < maxSpawns)
        {
            // 1. Tính toán vị trí ngẫu nhiên trong vùng xác định
            float randomX = Random.Range(spawnRangeMin.x, spawnRangeMax.x);
            float randomY = Random.Range(spawnRangeMin.y, spawnRangeMax.y);
            float randomZ = Random.Range(spawnRangeMin.z, spawnRangeMax.z);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            // 2. Sinh đối tượng tại vị trí ngẫu nhiên
            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);

            // 3. Tăng biến đếm lên 1
            currentSpawnCount++;

            // 4. Tạm dừng Coroutine trong 2 giây rồi mới lặp tiếp
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
```

### 2. Các bước cài đặt trên Unity Editor
1. Tạo một GameObject trống đặt tên là `Spawner`.
2. Kéo thả script `ObjectSpawner.cs` vào `Spawner`.
3. Tạo một Prefab đại diện cho đối tượng sinh ra (ví dụ: một 3D Cube) và kéo thả nó vào biến `Prefab To Spawn` trên Inspector.
4. Cấu hình vùng sinh ngẫu nhiên `Spawn Range Min` (ví dụ: `-5, 0, -5`) và `Spawn Range Max` (ví dụ: `5, 0, 5`).
5. Chạy game để kiểm chứng đối tượng được sinh ra ngẫu nhiên mỗi 2 giây và dừng lại ở đối tượng thứ 10.

---

## Bài tập 2: Làm mờ dần một đối tượng theo thời gian

### 1. Mã nguồn hoàn chỉnh (`ObjectFader.cs`)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Thư viện của New Input System

public class ObjectFader : MonoBehaviour
{
    public float fadeDuration = 5f; // Thời gian làm mờ (5 giây)
    
    private Renderer objectRenderer;  // Lưu trữ component Renderer của đối tượng
    private bool isFading = false;    // Biến cờ hiệu để kiểm tra trạng thái mờ

    void Start()
    {
        // Lấy component Renderer được gắn trên cùng GameObject này
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer == null)
        {
            Debug.LogError("GameObject này thiếu component Renderer để làm mờ!");
        }
    }

    void Update()
    {
        // Chỉ kích hoạt khi phím Space được nhấn VÀ đối tượng chưa ở trạng thái đang làm mờ
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !isFading)
        {
            StartCoroutine(FadeOutRoutine());
        }
    }

    IEnumerator FadeOutRoutine()
    {
        isFading = true; // Đánh dấu là đang làm mờ
        
        Color originalColor = objectRenderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime; // Tính thời gian trôi qua
            
            // Tính tỷ lệ từ 0 đến 1 dựa trên thời gian
            float t = elapsedTime / fadeDuration; 
            
            // Giảm alpha từ 1f về 0f theo tỷ lệ t
            float newAlpha = Mathf.Lerp(1f, 0f, t);
            
            // Cập nhật màu mới với alpha mới
            objectRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            
            yield return null; // Chờ 1 frame tiếp theo để tạo chuyển động mượt mà
        }

        // Đảm bảo alpha về đúng bằng 0 sau khi kết thúc
        objectRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        isFading = false; // Mở khóa cho phép làm mờ tiếp nếu muốn
    }
}
```

### 2. Các bước cài đặt trên Unity Editor
1. Đảm bảo dự án đã cài đặt **Input System Package** mới.
2. Tạo một đối tượng 3D (ví dụ: Cube) trong Scene.
3. Tạo một Material mới, đặt **Surface Type** (hoặc **Render Mode**) thành **Transparent** (hoặc **Fade**) để cho phép hiển thị độ trong suốt. Kéo thả Material này vào đối tượng 3D.
4. Gắn script `ObjectFader.cs` vào đối tượng 3D đó.
5. Chạy game, nhấn phím **Space** trên bàn phím và quan sát đối tượng mờ dần rồi biến mất hoàn toàn trong 5 giây.
