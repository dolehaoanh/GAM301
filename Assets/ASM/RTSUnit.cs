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

    [Header("Unit Stats Settings")]
    [Tooltip("Tên hiển thị của quân")]
    public string unitName = "Binh Sĩ";
    [Tooltip("Ảnh chân dung đại diện hiển thị trên UI")]
    public Sprite portrait;
    [Tooltip("Lượng máu tối đa")]
    public float maxHP = 100f;
    [Tooltip("Lượng máu hiện tại")]
    public float currentHP = 100f;

    [Header("Selection Visual Settings")]
    public float selectionRingRadius = 0.8f;
    public float selectionRingWidth = 0.08f;
    public Color selectionColor = new Color(0f, 1f, 0.5f, 0.9f); 

    private LineRenderer selectionLine;
    private bool isSelected = false;

    private void Start()
    {
        // Tự động phân biệt màu sắc và tên mặc định dựa trên loại quân
        if (unitType == RTSUnitType.Farmer)
        {
            selectionColor = new Color(0f, 1f, 0.2f, 0.9f); // Màu xanh cỏ cho Dân
            if (unitName == "Binh Sĩ") unitName = "Nông Dân";
        }
        else if (unitType == RTSUnitType.Soldier)
        {
            selectionColor = new Color(1f, 0.2f, 0f, 0.9f);  // Màu đỏ cam cho Lính chiến
            if (unitName == "Binh Sĩ") unitName = "Chiến Binh";
        }

        CreateSelectionRing();
        Deselect();
    }

    private void CreateSelectionRing()
    {
        GameObject ringObject = new GameObject("SelectionCircle_Auto");
        ringObject.transform.SetParent(transform);
        ringObject.transform.localPosition = new Vector3(0f, -0.95f, 0f);
        ringObject.transform.localRotation = Quaternion.identity;

        selectionLine = ringObject.AddComponent<LineRenderer>();
        selectionLine.useWorldSpace = false; 
        selectionLine.loop = true;
        selectionLine.startWidth = selectionRingWidth;
        selectionLine.endWidth = selectionRingWidth;

        Shader defaultShader = Shader.Find("Sprites/Default");
        if (defaultShader != null)
        {
            selectionLine.material = new Material(defaultShader);
        }
        selectionLine.startColor = selectionColor;
        selectionLine.endColor = selectionColor;

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

        selectionLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        selectionLine.receiveShadows = false;
    }

    public void Select()
    {
        isSelected = true;
        if (selectionLine != null)
        {
            selectionLine.enabled = true;
        }
    }

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