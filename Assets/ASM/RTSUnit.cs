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

    [Header("Gatherer Settings")]
    public float carriedAmount = 0f;
    public float maxCapacity = 10f;
    public RTSResourceType carriedType = RTSResourceType.None;

    public enum RTSUnitState
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingToDeliver
    }
    public RTSUnitState currentState = RTSUnitState.Idle;

    public ResourceNode targetResourceNode;
    public TownCenter targetTownCenter;

    private float gatherTimer = 0f;
    private GameObject visualBag;
    private UnityEngine.AI.NavMeshAgent navAgent;

    private LineRenderer selectionLine;
    private bool isSelected = false;

    private void Update()
    {
        if (unitType != RTSUnitType.Farmer) return;

        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        switch (currentState)
        {
            case RTSUnitState.Idle:
                break;

            case RTSUnitState.MovingToResource:
                if (targetResourceNode == null)
                {
                    currentState = RTSUnitState.Idle;
                    break;
                }

                // Đi đến bãi tài nguyên (Chỉ gọi SetDestination một lần hoặc khi đích đến thay đổi)
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    if (!navAgent.hasPath || navAgent.destination != targetResourceNode.transform.position)
                    {
                        navAgent.SetDestination(targetResourceNode.transform.position);
                    }
                }

                float distToResource = Vector3.Distance(transform.position, targetResourceNode.transform.position);
                bool reachedResource = distToResource <= targetResourceNode.harvestRange || 
                                     (navAgent != null && navAgent.isOnNavMesh && !navAgent.pathPending && navAgent.hasPath && navAgent.remainingDistance <= targetResourceNode.harvestRange);

                if (reachedResource)
                {
                    currentState = RTSUnitState.Gathering;
                    gatherTimer = 0f;
                    if (navAgent != null && navAgent.isOnNavMesh) navAgent.ResetPath();
                }
                break;

            case RTSUnitState.Gathering:
                if (targetResourceNode == null)
                {
                    // Nếu tài nguyên bị khai thác hết, tìm bãi khác gần đó hoặc đi tìm nhà chính giao hàng nếu có mang tài nguyên
                    if (carriedAmount > 0)
                    {
                        GoToDeliver();
                    }
                    else
                    {
                        currentState = RTSUnitState.Idle;
                    }
                    break;
                }

                gatherTimer += Time.deltaTime;
                if (gatherTimer >= 1.2f) // Cứ mỗi 1.2 giây khai thác 1 lần
                {
                    gatherTimer = 0f;

                    // Kích hoạt animation cuốc đất/chặt cây nếu có tham số
                    Animator animator = GetComponentInChildren<Animator>();
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
                            animator.SetTrigger("Work");
                        }
                    }

                    int gathered = targetResourceNode.Gather(2);
                    if (gathered > 0)
                    {
                        carriedAmount += gathered;
                        carriedType = targetResourceNode.resourceType;
                        UpdateVisualBag();
                    }

                    if (carriedAmount >= maxCapacity)
                    {
                        GoToDeliver();
                    }
                }
                break;

            case RTSUnitState.MovingToDeliver:
                if (targetTownCenter == null)
                {
                    targetTownCenter = TownCenter.FindNearest(transform.position);
                    if (targetTownCenter == null)
                    {
                        currentState = RTSUnitState.Idle;
                        break;
                    }
                }

                // Đi giao tài nguyên (Chỉ gọi SetDestination một lần hoặc khi đích đến thay đổi)
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    if (!navAgent.hasPath || navAgent.destination != targetTownCenter.transform.position)
                    {
                        navAgent.SetDestination(targetTownCenter.transform.position);
                    }
                }

                float distToTC = Vector3.Distance(transform.position, targetTownCenter.transform.position);
                bool reachedTC = distToTC <= targetTownCenter.deliverRange ||
                                 (navAgent != null && navAgent.isOnNavMesh && !navAgent.pathPending && navAgent.hasPath && navAgent.remainingDistance <= targetTownCenter.deliverRange);

                if (reachedTC)
                {
                    // Giao tài nguyên
                    if (PlayerResourceManager.Instance != null)
                    {
                        if (carriedType == RTSResourceType.Gold)
                        {
                            PlayerResourceManager.Instance.AddGold((int)carriedAmount);
                        }
                        else if (carriedType == RTSResourceType.Wood)
                        {
                            PlayerResourceManager.Instance.AddWood((int)carriedAmount);
                        }
                    }

                    carriedAmount = 0;
                    carriedType = RTSResourceType.None;
                    UpdateVisualBag();

                    // Quay lại bãi tài nguyên tiếp tục khai thác nếu bãi vẫn còn
                    if (targetResourceNode != null)
                    {
                        currentState = RTSUnitState.MovingToResource;
                    }
                    else
                    {
                        currentState = RTSUnitState.Idle;
                    }
                }
                break;
        }
    }

    private void GoToDeliver()
    {
        targetTownCenter = TownCenter.FindNearest(transform.position);
        if (targetTownCenter != null)
        {
            currentState = RTSUnitState.MovingToDeliver;
        }
        else
        {
            currentState = RTSUnitState.Idle;
        }
    }

    // Hàm gọi từ bên ngoài khi người chơi click chuột phải vào mỏ vàng/cây gỗ
    public void StartHarvesting(ResourceNode node)
    {
        if (unitType != RTSUnitType.Farmer) return;

        targetResourceNode = node;
        currentState = RTSUnitState.MovingToResource;
        
        // Nếu đang mang tài nguyên khác loại, xóa đi để lấy loại mới
        if (carriedType != RTSResourceType.None && carriedType != node.resourceType)
        {
            carriedAmount = 0;
            carriedType = RTSResourceType.None;
            UpdateVisualBag();
        }
    }

    private void UpdateVisualBag()
    {
        if (unitType != RTSUnitType.Farmer) return;

        if (carriedAmount > 0)
        {
            if (visualBag == null)
            {
                visualBag = GameObject.CreatePrimitive(PrimitiveType.Cube);
                visualBag.name = "CarriedResourceBag_Visual";
                // Gắn vào lưng nông dân
                visualBag.transform.SetParent(transform);
                visualBag.transform.localPosition = new Vector3(0f, 1.2f, -0.4f); // Vị trí trên lưng
                visualBag.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                
                // Xóa bỏ Collider để không gây va chạm NavMesh
                Destroy(visualBag.GetComponent<Collider>());
                
                // Đổi màu tương ứng loại tài nguyên mang theo
                Renderer r = visualBag.GetComponent<Renderer>();
                if (r != null)
                {
                    r.material = new Material(Shader.Find("Sprites/Default"));
                    r.material.color = carriedType == RTSResourceType.Gold ? new Color(1f, 0.85f, 0f) : new Color(0.55f, 0.35f, 0.15f);
                }
            }
        }
        else
        {
            if (visualBag != null)
            {
                Destroy(visualBag);
                visualBag = null;
            }
        }
    }

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