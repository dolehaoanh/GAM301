using UnityEngine;
using System.Collections.Generic;

public class RTSUnitSelection : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("Màu nền của khung quét chữ nhật (trong suốt)")]
    public Color boxColor = new Color(0.12f, 0.7f, 1f, 0.15f); // Màu xanh lam trong suốt
    [Tooltip("Màu viền của khung quét chữ nhật")]
    public Color borderColor = new Color(0.12f, 0.7f, 1f, 0.8f);

    [Header("Formation Settings")]
    [Tooltip("Khoảng cách giãn cách giữa các quân lính khi xếp hàng tại điểm đích")]
    public float formationSpacing = 1.8f;
    [Tooltip("Khoảng cách khoảng trống phân cấp giữa nhóm Lính (phía trước) và nhóm Dân (phía sau)")]
    public float classGapDistance = 4.5f;

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

        // 3. Khi nhấn Chuột Phải: Di chuyển các quân đang chọn theo đội hình Grid
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

    // Lệnh di chuyển toàn bộ quân lính đang chọn về điểm đích - NÂNG CẤP ĐỘI HÌNH TÁC CHIẾN LÍNH ĐI TRƯỚC, DÂN ĐI SAU
    private void MoveSelectedUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            int unitCount = selectedUnits.Count;
            if (unitCount == 0) return;

            // 1. Phân loại quân lính được chọn thành 2 nhóm riêng biệt: Lính (Soldiers) và Dân (Farmers)
            List<RTSUnit> soldiers = new List<RTSUnit>();
            List<RTSUnit> farmers = new List<RTSUnit>();
            Vector3 groupCenter = Vector3.zero;
            int activeUnits = 0;

            foreach (RTSUnit unit in selectedUnits)
            {
                if (unit == null) continue;
                
                groupCenter += unit.transform.position;
                activeUnits++;

                if (unit.unitType == RTSUnitType.Soldier)
                {
                    soldiers.Add(unit);
                }
                else
                {
                    farmers.Add(unit);
                }
            }
            if (activeUnits > 0) groupCenter /= activeUnits;

            // 2. Tính toán hướng di chuyển và hướng xoay đội hình tương ứng
            Vector3 travelDirection = hit.point - groupCenter;
            travelDirection.y = 0f;

            Quaternion formationRotation;
            if (travelDirection.sqrMagnitude > 0.2f)
            {
                formationRotation = Quaternion.LookRotation(travelDirection.normalized);
            }
            else
            {
                formationRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
            }

            // 3. Thực hiện phân vùng dàn quân chiến thuật:
            // Nếu cả lính và dân đều đang được chọn: Lính sẽ đứng ở tâm điểm click chuột (Tiền tuyến), 
            // Dân sẽ lùi lại phía sau một khoảng trống an toàn (Hậu phương)
            if (soldiers.Count > 0 && farmers.Count > 0)
            {
                // Điểm trung tâm đội lính (ở trước)
                Vector3 soldierCenter = hit.point;
                // Điểm trung tâm đội dân (ở sau) = Điểm click trừ đi vector hướng nhìn hướng ra phía sau
                Vector3 farmerCenter = hit.point - (formationRotation * Vector3.forward * classGapDistance);

                MoveGroupInGrid(soldiers, soldierCenter, formationRotation);
                MoveGroupInGrid(farmers, farmerCenter, formationRotation);
            }
            else
            {
                // Nếu chỉ chọn riêng 1 loại quân: di chuyển bình thường theo đội hình chuẩn tại điểm click chuột
                MoveGroupInGrid(selectedUnits, hit.point, formationRotation);
            }
        }
    }

    // Hàm phụ trợ: Sắp xếp một nhóm quân cụ thể thành lưới 2 hàng tại vị trí trung tâm chỉ định
    private void MoveGroupInGrid(List<RTSUnit> group, Vector3 centerPoint, Quaternion rotation)
    {
        int count = group.Count;
        if (count == 0) return;

        // Thiết lập đội hình tối đa 2 hàng ngang
        int rows = (count <= 2) ? 1 : 2;
        int cols = Mathf.CeilToInt((float)count / rows);

        for (int i = 0; i < count; i++)
        {
            if (group[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            // Tính vị trí lệch
            float xOffset = (col - (cols - 1) / 2.0f) * formationSpacing;
            float zOffset = (row - (rows - 1) / 2.0f) * formationSpacing;

            Vector3 localOffset = new Vector3(xOffset, 0f, zOffset);
            Vector3 rotatedOffset = rotation * localOffset;

            Vector3 finalDestination = centerPoint + rotatedOffset;

            UnityEngine.AI.NavMeshAgent agent = group[i].GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(finalDestination);
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