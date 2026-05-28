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

    // Tham chiếu tự động tới HUD Controller
    private RTSHUDController hudController;

    private void Start()
    {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();

        // Tự động tìm HUD Controller trong Scene
        hudController = FindObjectOfType<RTSHUDController>();
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

        RTSUnit[] allUnits = FindObjectsOfType<RTSUnit>();

        foreach (RTSUnit unit in allUnits)
        {
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
        }
    }

    private void MoveGroupInGrid(List<RTSUnit> group, Vector3 centerPoint, Quaternion rotation)
    {
        int count = group.Count;
        if (count == 0) return;

        int rows = (count <= 2) ? 1 : 2;
        int cols = Mathf.CeilToInt((float)count / rows);

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
            if (agent != null)
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
            hudController = FindObjectOfType<RTSHUDController>();
        }

        if (hudController != null)
        {
            if (selectedUnits.Count > 0)
            {
                // Hiển thị thông số của con lính đầu tiên trong danh sách đang chọn
                RTSUnit firstSelected = selectedUnits[0];
                hudController.ShowUnitSelection(
                    firstSelected.portrait, 
                    firstSelected.unitName, 
                    firstSelected.currentHP, 
                    firstSelected.maxHP,
                    firstSelected.unitType // <-- Gửi thêm loại quân để đồng bộ khuôn mặt 3D Portrait
                );
            }
            else
            {
                // Ẩn bảng đi nếu click đất trống (không chọn ai)
                hudController.HideSelectionPanel();
            }
        }
    }

    public void DeselectAll()
    {
        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit != null) unit.Deselect();
        }
        selectedUnits.Clear();
        UpdateHUD(); // <-- Ẩn HUD đi
    }
}