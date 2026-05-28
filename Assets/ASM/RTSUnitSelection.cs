using UnityEngine;
using System.Collections.Generic;

public class RTSUnitSelection : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("Màu nền của khung quét chữ nhật (trong suốt)")]
    public Color boxColor = new Color(0.12f, 0.7f, 1f, 0.15f); // Màu xanh lam trong suốt
    [Tooltip("Màu viền của khung quét chữ nhật")]
    public Color borderColor = new Color(0.12f, 0.7f, 1f, 0.8f);

    private Texture2D whiteTexture;
    private Vector3 startMousePosition;
    private bool isDrawing = false;

    // Danh sách toàn bộ lính đang được chọn hiện tại
    public List<RTSUnit> selectedUnits = new List<RTSUnit>();

    private void Start()
    {
        // Tạo một Texture 1x1 pixel màu trắng động để vẽ khung quét không cần file ảnh ngoài
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private void Update()
    {
        // 1. Khi nhấn Chuột Trái: Bắt đầu quét khung
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            isDrawing = true;
        }

        // 2. Khi thả Chuột Trái: Hoàn tất quét và chọn quân
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            SelectUnitsInBox();
        }

        // 3. Khi nhấn Chuột Phải: Di chuyển các quân đang chọn
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            MoveSelectedUnits();
        }
    }

    // Hàm vẽ giao diện khung quét 2D trong OnGUI
    private void OnGUI()
    {
        if (isDrawing && startMousePosition != Input.mousePosition)
        {
            // Tọa độ màn hình Unity có Y=0 ở dưới cùng, nhưng GUI có Y=0 ở trên cùng.
            // Chúng ta quy đổi tọa độ để vẽ chuẩn xác:
            var rect = GetScreenRect(startMousePosition, Input.mousePosition);
            
            // Vẽ nền trong suốt
            GUI.color = boxColor;
            GUI.DrawTexture(rect, whiteTexture);

            // Vẽ 4 đường viền xung quanh
            GUI.color = borderColor;
            DrawScreenRectBorder(rect, 1.5f);
            
            // Trả lại màu GUI mặc định
            GUI.color = Color.white;
        }
    }

    // Thuật toán tính toán hình chữ nhật quét màn hình
    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Di chuyển góc tọa độ từ trái-dưới lên trái-trên
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        
        // Tính toán các điểm góc
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        
        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    // Hàm vẽ đường viền
    private void DrawScreenRectBorder(Rect rect, float thickness)
    {
        // Viền trên
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), whiteTexture);
        // Viền dưới
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), whiteTexture);
        // Viền trái
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), whiteTexture);
        // Viền phải
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), whiteTexture);
    }

    // Thuật toán trung tâm: Quét quân nằm trong hộp
    private void SelectUnitsInBox()
    {
        // Nếu click nhẹ một điểm thay vì kéo hộp quét rộng
        if (Vector3.Distance(startMousePosition, Input.mousePosition) < 4f)
        {
            SingleClickSelect();
            return;
        }

        // Bỏ chọn toàn bộ lính cũ trước
        DeselectAll();

        // Xác định biên của hộp quét 2D trong không gian màn hình
        float minX = Mathf.Min(startMousePosition.x, Input.mousePosition.x);
        float maxX = Mathf.Max(startMousePosition.x, Input.mousePosition.x);
        float minY = Mathf.Min(startMousePosition.y, Input.mousePosition.y);
        float maxY = Mathf.Max(startMousePosition.y, Input.mousePosition.y);

        // Tìm tất cả các quân lính RTSUnit có trên bản đồ hiện tại
        RTSUnit[] allUnits = FindObjectsOfType<RTSUnit>();

        foreach (RTSUnit unit in allUnits)
        {
            // Chuyển đổi vị trí 3D của lính sang tọa độ màn hình 2D
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            // Kiểm tra xem tọa độ 2D của lính có nằm bên trong hộp quét không
            if (screenPos.x > minX && screenPos.x < maxX && screenPos.y > minY && screenPos.y < maxY)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }
        
        Debug.Log($"[RTS Selection] Đã chọn thành công {selectedUnits.Count} quân lính!");
    }

    // Tính năng nhấp chuột trái đơn lẻ để chọn 1 mục tiêu
    private void SingleClickSelect()
    {
        DeselectAll();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            RTSUnit unit = hit.collider.GetComponent<RTSUnit>();
            if (unit != null)
            {
                unit.Select();
                selectedUnits.Add(unit);
                Debug.Log($"[RTS Selection] Đã chọn 1 quân: {unit.gameObject.name}");
            }
        }
    }

    // Lệnh di chuyển toàn bộ quân lính đang chọn về điểm đích
    private void MoveSelectedUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (RTSUnit unit in selectedUnits)
            {
                UnityEngine.AI.NavMeshAgent agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }

    // Hàm giải phóng tất cả quân lính khỏi trạng thái được chọn
    public void DeselectAll()
    {
        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit != null) unit.Deselect();
        }
        selectedUnits.Clear();
    }
}