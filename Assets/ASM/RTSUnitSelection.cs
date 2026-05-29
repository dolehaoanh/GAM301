using UnityEngine;
using System.Collections.Generic;

public class RTSUnitSelection : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color boxColor = new Color(0.12f, 0.7f, 1f, 0.15f); 
    public Color borderColor = new Color(0.12f, 0.7f, 1f, 0.8f);

    [Header("Formation Settings")]
    public float formationSpacing = 1.8f;
    public float classGapDistance = 4.5f;

    private Texture2D whiteTexture;
    private Vector3 startMousePosition;
    private bool isDrawing = false;

    // Danh sách toàn bộ lính đang được chọn hiện tại
    public List<RTSUnit> selectedUnits = new List<RTSUnit>();
    public TownCenter selectedTownCenter;
    public Barracks selectedBarracks;

    // Tham chiếu tự động tới HUD Controller
    private RTSHUDController hudController;

    private void Start()
    {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();

        // Tự động tìm HUD Controller trong Scene
        hudController = FindAnyObjectByType<RTSHUDController>();
    }

    private void Update()
    {
        // 1. Nhấp chuột trái xuống (chỉ bắt đầu vẽ hộp chọn nếu không nhấp trên UI)
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            startMousePosition = Input.mousePosition;
            isDrawing = true;
        }

        // 2. Thả chuột trái (chỉ xử lý chọn quân nếu trước đó đang vẽ hộp chọn)
        if (Input.GetMouseButtonUp(0))
        {
            if (isDrawing)
            {
                isDrawing = false;
                SelectUnitsInBox();
            }
        }

        // 3. Nhấp chuột phải di chuyển (bỏ qua nếu nhấp trên UI, vd: nhấp nút bấm)
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            MoveSelectedUnits();
        }
    }

    private void OnGUI()
    {
        if (isDrawing && startMousePosition != Input.mousePosition)
        {
            var rect = GetScreenRect(startMousePosition, Input.mousePosition);
            GUI.color = boxColor;
            GUI.DrawTexture(rect, whiteTexture);

            GUI.color = borderColor;
            DrawScreenRectBorder(rect, 1.5f);
            
            GUI.color = Color.white;
        }
    }

    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        
        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    private void DrawScreenRectBorder(Rect rect, float thickness)
    {
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), whiteTexture);
    }

    private void SelectUnitsInBox()
    {
        if (Vector3.Distance(startMousePosition, Input.mousePosition) < 4f)
        {
            SingleClickSelect();
            return;
        }

        DeselectAll();

        float minX = Mathf.Min(startMousePosition.x, Input.mousePosition.x);
        float maxX = Mathf.Max(startMousePosition.x, Input.mousePosition.x);
        float minY = Mathf.Min(startMousePosition.y, Input.mousePosition.y);
        float maxY = Mathf.Max(startMousePosition.y, Input.mousePosition.y);

        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude);

        foreach (RTSUnit unit in allUnits)
        {
            if (unit == null || unit.transform.position.y < -100f) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > minX && screenPos.x < maxX && screenPos.y > minY && screenPos.y < maxY)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }
        
        Debug.Log($"[RTS Selection] Đã chọn thành công {selectedUnits.Count} quân lính!");
        UpdateHUD(); // <-- Cập nhật hiển thị lên HUD Canvas
    }

    private void SingleClickSelect()
    {
        DeselectAll();
        selectedTownCenter = null;
        selectedBarracks = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Sử dụng GetComponentInParent để hỗ trợ chọn các mô hình con có Collider bên trong nhóm cha
            RTSUnit unit = hit.collider.GetComponentInParent<RTSUnit>();
            if (unit == null)
            {
                unit = hit.collider.GetComponent<RTSUnit>();
            }

            if (unit != null)
            {
                unit.Select();
                selectedUnits.Add(unit);
                Debug.Log($"[RTS Selection] Đã chọn 1 quân: {unit.gameObject.name}");
            }
            else
            {
                // Thử tìm chọn Nhà Chính (Town Center)
                TownCenter tc = hit.collider.GetComponentInParent<TownCenter>();
                if (tc == null)
                {
                    tc = hit.collider.GetComponent<TownCenter>();
                }

                if (tc != null)
                {
                    selectedTownCenter = tc;
                    Debug.Log($"[RTS Selection] Đã chọn Nhà Chính: {tc.gameObject.name}");
                }
                else
                {
                    // Thử tìm chọn Nhà Lính (Barracks)
                    Barracks b = hit.collider.GetComponentInParent<Barracks>();
                    if (b == null)
                    {
                        b = hit.collider.GetComponent<Barracks>();
                    }

                    if (b != null)
                    {
                        selectedBarracks = b;
                        Debug.Log($"[RTS Selection] Đã chọn Nhà Lính: {b.gameObject.name}");
                    }
                }
            }
        }
        
        UpdateHUD(); // <-- Cập nhật hiển thị lên HUD Canvas
    }

    private void MoveSelectedUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            int unitCount = selectedUnits.Count;
            if (unitCount == 0) return;

            // Kiểm tra xem người chơi nhấp chuột phải trúng Mỏ Vàng hoặc Cây Gỗ không
            ResourceNode clickedNode = hit.collider.GetComponentInParent<ResourceNode>();
            if (clickedNode == null)
            {
                clickedNode = hit.collider.GetComponent<ResourceNode>();
            }

            if (clickedNode != null)
            {
                int farmerGatherers = 0;
                foreach (RTSUnit unit in selectedUnits)
                {
                    if (unit != null && unit.unitType == RTSUnitType.Farmer)
                    {
                        unit.StartHarvesting(clickedNode);
                        farmerGatherers++;
                    }
                }

                if (farmerGatherers > 0)
                {
                    Debug.Log($"[RTS Command] Đã điều {farmerGatherers} nông dân đi khai thác {clickedNode.resourceType}!");
                    SetRTSCursor(RTSCursorState.Default);
                    return;
                }
            }

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

            if (soldiers.Count > 0 && farmers.Count > 0)
            {
                Vector3 soldierCenter = hit.point;
                Vector3 farmerCenter = hit.point - (formationRotation * Vector3.forward * classGapDistance);

                MoveGroupInGrid(soldiers, soldierCenter, formationRotation);
                MoveGroupInGrid(farmers, farmerCenter, formationRotation);
            }
            else
            {
                MoveGroupInGrid(selectedUnits, hit.point, formationRotation);
            }

            // Đặt lại con trỏ chuột về mặc định sau khi đã ra lệnh di chuyển thành công
            SetRTSCursor(RTSCursorState.Default);
        }
    }

    private void MoveGroupInGrid(List<RTSUnit> group, Vector3 centerPoint, Quaternion rotation)
    {
        int count = group.Count;
        if (count == 0) return;

        // Tự động phân loại: Nếu là Chiến Binh (Soldier), xếp thành 1 hàng ngang dàn trận cạnh nhau (Single Row)
        bool isSoldierGroup = (group[0] != null && group[0].unitType == RTSUnitType.Soldier);

        int rows = 1;
        int cols = count;

        if (isSoldierGroup)
        {
            // Hàng ngang xếp dàn trận cạnh nhau vuông góc hướng đi
            rows = 1;
            cols = count;
        }
        else
        {
            // Nông dân giữ nguyên đội hình khối hộp (Grid) mặc định ban đầu
            rows = (count <= 2) ? 1 : 2;
            cols = Mathf.CeilToInt((float)count / rows);
        }

        for (int i = 0; i < count; i++)
        {
            if (group[i] == null) continue;

            int row = i / cols;
            int col = i % cols;

            float xOffset = (col - (cols - 1) / 2.0f) * formationSpacing;
            float zOffset = (row - (rows - 1) / 2.0f) * formationSpacing;

            Vector3 localOffset = new Vector3(xOffset, 0f, zOffset);
            Vector3 rotatedOffset = rotation * localOffset;

            Vector3 finalDestination = centerPoint + rotatedOffset;

            UnityEngine.AI.NavMeshAgent agent = group[i].GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(finalDestination);
            }
        }
    }

    // Gửi thông số lính đang chọn lên HUD
    private void UpdateHUD()
    {
        if (hudController == null)
        {
            hudController = FindAnyObjectByType<RTSHUDController>();
        }

        if (hudController != null)
        {
            // Gửi toàn bộ danh sách đang chọn kèm theo Nhà Chính và Nhà Lính để HUD xử lý
            hudController.ShowSelection(selectedUnits, selectedTownCenter, selectedBarracks);
        }
    }

    public void DeselectAll()
    {
        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit != null) unit.Deselect();
        }
        selectedUnits.Clear();
        selectedTownCenter = null;
        selectedBarracks = null;
        UpdateHUD(); // <-- Ẩn HUD đi
    }

    // ==========================================
    // ⚔️ RTS PANEL COMMAND BUTTON FUNCTIONS ⚔️
    // ==========================================

    [Header("Custom Cursor Textures")]
    [Tooltip("Ảnh con trỏ mặc định (Bàn tay/Mũi tên)")]
    public Texture2D defaultCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Di Chuyển (Move)")]
    public Texture2D moveCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Tấn Công (Attack)")]
    public Texture2D attackCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Khai Thác (Gather)")]
    public Texture2D gatherCursor;

    [Tooltip("Tâm của con trỏ chuột (Hotspot)")]
    public Vector2 cursorHotspot = Vector2.zero;

    public enum RTSCursorState
    {
        Default,
        Move,
        Attack,
        Gather
    }

    // Hàm thay đổi hình dáng con trỏ chuột
    public void SetRTSCursor(RTSCursorState state)
    {
        Texture2D activeTexture = null;
        switch (state)
        {
            case RTSCursorState.Default:
                activeTexture = defaultCursor;
                break;
            case RTSCursorState.Move:
                activeTexture = moveCursor;
                 break;
            case RTSCursorState.Attack:
                activeTexture = attackCursor;
                break;
            case RTSCursorState.Gather:
                activeTexture = gatherCursor;
                break;
        }

        // Thay đổi con trỏ chuột trong Unity ở dạng ForceSoftware để ngăn HĐH phóng to khi Maximize
        Cursor.SetCursor(activeTexture, cursorHotspot, CursorMode.ForceSoftware);
    }

    [Header("Command SFX Settings")]
    public AudioClip stopCommandSFX;
    public AudioClip attackCommandSFX;

    // 1. Lệnh Di Chuyển (Move)
    public void OnCommandMove()
    {
        if (selectedUnits.Count == 0) return;
        
        // Kích hoạt con trỏ lệnh di chuyển
        SetRTSCursor(RTSCursorState.Move);
        Debug.Log($"[RTS Command] DI CHUYỂN BỘ BINH ({selectedUnits.Count} quân)!");
    }

    // 2. Lệnh Dừng Lại (Stop) - Dừng mọi chuyển động ngay lập tức
    public void OnCommandStop()
    {
        if (selectedUnits.Count == 0) return;

        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit == null) continue;

            UnityEngine.AI.NavMeshAgent agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath(); // Hủy đường đi hiện tại để đứng yên
            }
        }

        SetRTSCursor(RTSCursorState.Default); // Trở về mặc định
        Debug.Log($"[RTS Command] DỪNG QUÂN NGAY LẬP TỨC ({selectedUnits.Count} quân)!");
    }

    // 3. Lệnh Tấn Công (Attack)
    public void OnCommandAttack()
    {
        if (selectedUnits.Count == 0) return;
        
        // Kích hoạt con trỏ lệnh tấn công
        SetRTSCursor(RTSCursorState.Attack);
        Debug.Log($"[RTS Command] XUẤT BINH TẤN CÔNG ĐỊCH ({selectedUnits.Count} quân)!");
    }

    // 4. Lệnh Giữ Vị Trí (Hold Position)
    public void OnCommandHold()
    {
        if (selectedUnits.Count == 0) return;

        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit == null) continue;

            UnityEngine.AI.NavMeshAgent agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath(); // Hủy đường để giữ vị trí cố định
            }
        }

        SetRTSCursor(RTSCursorState.Default); // Trở về mặc định
        Debug.Log($"[RTS Command] THỦ THẾ / GIỮ VỮNG ĐỘI HÌNH ({selectedUnits.Count} quân)!");
    }

    // 5. Lệnh Tuần Tra (Patrol)
    public void OnCommandPatrol()
    {
        if (selectedUnits.Count == 0) return;
        Debug.Log($"[RTS Command] TUẦN TRA QUANH ĐỊA BÀN ({selectedUnits.Count} quân)!");
    }

    // 6. Lệnh Khai Thác / Xây Dựng (Gather / Build) - Cho phép Nông dân bắt đầu cuốc đất
    public void OnCommandGather()
    {
        if (selectedUnits.Count == 0) return;

        // Kích hoạt con trỏ khai thác
        SetRTSCursor(RTSCursorState.Gather);

        int farmerCount = 0;
        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit == null) continue;

            // Nếu là nông dân, kích hoạt trigger "Work" (cuốc đất / chặt cây)
            if (unit.unitType == RTSUnitType.Farmer)
            {
                Animator animator = unit.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    bool hasWorkParam = false;
                    foreach (AnimatorControllerParameter param in animator.parameters)
                    {
                        if (param.name == "Work")
                        {
                            hasWorkParam = true;
                            break;
                        }
                    }
                    if (hasWorkParam)
                    {
                        animator.SetTrigger("Work"); // Kích hoạt trigger Work trong Animator!
                    }
                }
                farmerCount++;
            }
        }

        Debug.Log($"[RTS Command] KHAI THÁC TÀI NGUYÊN (Kích hoạt cuốc đất cho {farmerCount} nông dân)!");
    }
}