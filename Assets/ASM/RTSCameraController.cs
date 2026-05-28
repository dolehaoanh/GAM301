using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển của camera")]
    public float panSpeed = 25f;
    [Tooltip("Độ mượt/quán tính khi camera dừng lại")]
    public float panTime = 5f; 

    [Header("Edge Scrolling Settings")]
    [Tooltip("Bật/Tắt tính năng rê chuột sát viền màn hình để di chuyển")]
    public bool enableEdgeScrolling = true;
    [Tooltip("Độ rộng của đường viền (pixel) để bắt đầu cuộn camera")]
    public float edgeBoundary = 15f; 

    [Header("Zoom Settings")]
    [Tooltip("Tốc độ thu phóng camera")]
    public float zoomSpeed = 75f;
    [Tooltip("Độ cao tối thiểu khi zoom sát đất")]
    public float minHeight = 5f;
    [Tooltip("Độ cao tối đa khi zoom lên mây")]
    public float maxHeight = 35f;

    [Header("Rotation Settings")]
    [Tooltip("Tốc độ xoay camera bằng phím Q/E")]
    public float keyboardRotationSpeed = 100f;
    [Tooltip("Tốc độ xoay camera bằng chuột")]
    public float mouseRotationSpeed = 300f;
    [Tooltip("Độ mượt khi xoay camera")]
    public float rotationTime = 5f;

    [Header("Map Boundaries")]
    [Tooltip("Tọa độ góc trái dưới của bản đồ")]
    public Vector2 minBounds = Vector2.zero;
    [Tooltip("Tọa độ góc phải trên của bản đồ")]
    public Vector2 maxBounds = new Vector2(128f, 128f); 
    [Tooltip("Khoảng lùi an toàn so với rìa bản đồ để tránh camera nhìn ra ngoài khoảng không")]
    public float boundaryPadding = 15f;

    private Vector3 targetPosition;
    private float targetZoomHeight;
    private float targetRotationY;
    
    // Lưu trữ góc nghiêng X ban đầu để tránh lỗi Quaternion Slerp khóa trục
    private float initialRotationX;

    private void Start()
    {
        // Khởi tạo các giá trị mục tiêu ban đầu từ trạng thái hiện tại của camera
        targetPosition = transform.position;
        targetZoomHeight = transform.position.y;
        targetRotationY = transform.eulerAngles.y;
        
        // Khóa và lưu góc nghiêng X (ví dụ 60 độ) tại đây
        initialRotationX = transform.eulerAngles.x;
    }

    private void Update()
    {
        HandleKeyboardMovement();
        
        if (enableEdgeScrolling)
        {
            HandleEdgeScrolling();
        }

        HandleZoom();
        HandleRotation();
        ApplyMovementAndLimits();
    }

    // 1. Di chuyển bằng phím bấm (WASD / Mũi tên) - Đã cập nhật di chuyển theo hướng nhìn của Camera
    private void HandleKeyboardMovement()
    {
        float x = Input.GetAxisRaw("Horizontal"); // Phím A/D hoặc Trái/Phải
        float z = Input.GetAxisRaw("Vertical");   // Phím W/S hoặc Lên/Xuống

        // Lấy hướng tiến của camera chiếu phẳng lên mặt đất (bỏ qua trục Y)
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Lấy hướng sang phải của camera chiếu phẳng lên mặt đất (bỏ qua trục Y)
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        // Tính toán hướng di chuyển thực tế dựa trên góc quay của camera
        Vector3 direction = (forward * z + right * x).normalized;
        targetPosition += direction * panSpeed * Time.deltaTime;
    }

    // 2. Di chuyển khi rê chuột sát viền màn hình - Đã cập nhật di chuyển theo hướng nhìn của Camera
    private void HandleEdgeScrolling()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        Vector3 edgeDirection = Vector3.zero;

        // Rê chuột sát lề trái
        if (mouseX >= 0 && mouseX <= edgeBoundary)
        {
            edgeDirection.x = -1f;
        }
        // Rê chuột sát lề phải
        else if (mouseX >= Screen.width - edgeBoundary && mouseX <= Screen.width)
        {
            edgeDirection.x = 1f;
        }

        // Rê chuột sát lề dưới
        if (mouseY >= 0 && mouseY <= edgeBoundary)
        {
            edgeDirection.z = -1f;
        }
        // Rê chuột sát lề trên
        else if (mouseY >= Screen.height - edgeBoundary && mouseY <= Screen.height)
        {
            edgeDirection.z = 1f;
        }

        // Tương tự, quy đổi hướng rê chuột theo góc quay hiện tại của camera
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 direction = (forward * edgeDirection.z + right * edgeDirection.x).normalized;
        targetPosition += direction * panSpeed * Time.deltaTime;
    }

    // 3. Thu phóng camera bằng con lăn chuột
    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            targetZoomHeight -= scrollInput * zoomSpeed;
            targetZoomHeight = Mathf.Clamp(targetZoomHeight, minHeight, maxHeight);
        }
    }

    // 4. Lập trình 3 phương thức xoay camera song song
    private void HandleRotation()
    {
        // CÁCH 1: Xoay bằng phím Q và E
        if (Input.GetKey(KeyCode.Q))
        {
            targetRotationY -= keyboardRotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            targetRotationY += keyboardRotationSpeed * Time.deltaTime;
        }

        // CÁCH 2 & 3: Xoay bằng Chuột Giữa HOẶC giữ Alt + Chuột Phải
        bool isRotatingWithMouse = Input.GetMouseButton(2) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1));

        if (isRotatingWithMouse)
        {
            float mouseX = Input.GetAxis("Mouse X");
            targetRotationY += mouseX * mouseRotationSpeed * Time.deltaTime;
        }
    }

    // 5. Áp dụng di chuyển, xoay mềm mại và khống chế biên giới hạn
    private void ApplyMovementAndLimits()
    {
        // Cập nhật độ cao Y vào Vector vị trí mục tiêu
        targetPosition.y = targetZoomHeight;

        // Áp dụng khoảng lùi (padding) để khống chế biên giới hạn an toàn
        float minX = minBounds.x + boundaryPadding;
        float maxX = maxBounds.x - boundaryPadding;
        float minZ = minBounds.y + boundaryPadding;
        float maxZ = maxBounds.y - boundaryPadding;

        // Khống chế tọa độ X và Z lùi lại so với rìa bản đồ
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

        // Áp dụng di chuyển mềm mại vị trí (Position Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panTime);

        // Áp dụng xoay mềm mại góc nhìn (Rotation Lerp) - SỬ DỤNG initialRotationX CỐ ĐỊNH
        Quaternion targetRotation = Quaternion.Euler(initialRotationX, targetRotationY, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationTime);
    }
}