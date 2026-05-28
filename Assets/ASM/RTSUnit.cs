using UnityEngine;

public enum RTSUnitType
{
    Farmer,
    Soldier
}

public class RTSUnit : MonoBehaviour
{
    [Header("Unit Settings")]
    [Tooltip("Phân loại quân lính (Dân hay Lính)")]
    public RTSUnitType unitType = RTSUnitType.Soldier;

    [Header("Selection Visual Settings")]
    [Tooltip("Bán kính của vòng tròn chọn dưới chân")]
    public float selectionRingRadius = 0.8f;
    [Tooltip("Độ dày của đường vẽ vòng tròn")]
    public float selectionRingWidth = 0.08f;
    [Tooltip("Màu sắc phát sáng của vòng chọn")]
    public Color selectionColor = new Color(0f, 1f, 0.5f, 0.9f); // Màu xanh ngọc phát sáng

    private LineRenderer selectionLine;
    private bool isSelected = false;

    private void Start()
    {
        // Tự động phân biệt màu sắc vòng tròn dựa trên phân loại quân lính
        if (unitType == RTSUnitType.Farmer)
        {
            selectionColor = new Color(0f, 1f, 0.2f, 0.9f); // Màu xanh cỏ phát sáng cho Dân
        }
        else if (unitType == RTSUnitType.Soldier)
        {
            selectionColor = new Color(1f, 0.2f, 0f, 0.9f);  // Màu đỏ cam phát sáng cho Lính chiến
        }

        CreateSelectionRing();
        Deselect();
    }

    // Thuật toán tự động sinh vòng tròn phát sáng bằng LineRenderer sát mặt đất
    private void CreateSelectionRing()
    {
        // 1. Tạo một GameObject con tự động
        GameObject ringObject = new GameObject("SelectionCircle_Auto");
        ringObject.transform.SetParent(transform);
        
        // Tọa độ Y = -0.95f nằm ngay sát đáy của Capsule cao 2m (tránh bị chìm dưới đất - Z-fighting)
        ringObject.transform.localPosition = new Vector3(0f, -0.95f, 0f);
        ringObject.transform.localRotation = Quaternion.identity;

        // 2. Thêm và cấu hình Component LineRenderer
        selectionLine = ringObject.AddComponent<LineRenderer>();
        selectionLine.useWorldSpace = false; // Sử dụng tọa độ Local để quay theo lính
        selectionLine.loop = true;
        selectionLine.startWidth = selectionRingWidth;
        selectionLine.endWidth = selectionRingWidth;

        // 3. Sử dụng Shader Sprites mặc định để tự động phát sáng bất kể điều kiện ánh sáng
        Shader defaultShader = Shader.Find("Sprites/Default");
        if (defaultShader != null)
        {
            selectionLine.material = new Material(defaultShader);
        }
        selectionLine.startColor = selectionColor;
        selectionLine.endColor = selectionColor;

        // 4. Vẽ vòng tròn 360 độ bằng công thức toán lượng giác (36 phân đoạn)
        int segments = 36;
        selectionLine.positionCount = segments;
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin(angle) * selectionRingRadius;
            float z = Mathf.Cos(angle) * selectionRingRadius;
            selectionLine.SetPosition(i, new Vector3(x, 0f, z));
            angle += (2f * Mathf.PI) / segments;
        }

        // Tắt tính năng đổ bóng đổ để đường vẽ sáng rõ nét
        selectionLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        selectionLine.receiveShadows = false;
    }

    // Hàm kích hoạt khi quân lính được quét trúng
    public void Select()
    {
        isSelected = true;
        if (selectionLine != null)
        {
            selectionLine.enabled = true;
        }
    }

    // Hàm kích hoạt khi bỏ chọn quân lính
    public void Deselect()
    {
        isSelected = false;
        if (selectionLine != null)
        {
            selectionLine.enabled = false;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}