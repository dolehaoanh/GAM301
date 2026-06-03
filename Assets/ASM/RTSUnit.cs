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
    public bool isEnemy = false; // Phân biệt phe ta và phe địch

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

    [Header("Combat Settings")]
    public float attackRange = 2.5f;
    public float scanRange = 15f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0f;

    public RTSUnit combatTarget;
    public GameObject combatBuildingTarget;

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
        if (currentHP <= 0f) return;

        if (unitType == RTSUnitType.Farmer)
        {
            UpdateFarmer();
        }
        else if (unitType == RTSUnitType.Soldier)
        {
            UpdateSoldier();
        }
    }

    private void UpdateFarmer()
    {
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

                // Đi đến bãi tài nguyên
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    Vector3 targetPos = targetResourceNode.transform.position;
                    Vector3 currentDest = navAgent.destination;
                    bool needsNewPath = !navAgent.hasPath || 
                                        Mathf.Abs(currentDest.x - targetPos.x) > 0.1f || 
                                        Mathf.Abs(currentDest.z - targetPos.z) > 0.1f;
                    if (needsNewPath)
                    {
                        navAgent.SetDestination(targetPos);
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

                // Đi giao tài nguyên
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    Vector3 targetPos = targetTownCenter.transform.position;
                    Vector3 currentDest = navAgent.destination;
                    bool needsNewPath = !navAgent.hasPath || 
                                        Mathf.Abs(currentDest.x - targetPos.x) > 0.1f || 
                                        Mathf.Abs(currentDest.z - targetPos.z) > 0.1f;
                    if (needsNewPath)
                    {
                        navAgent.SetDestination(targetPos);
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

    private void UpdateSoldier()
    {
        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // TỰ ĐỘNG QUÉT ĐỊCH
        // 1. Quét tìm lính đối thủ trước
        if (combatTarget == null || combatTarget.currentHP <= 0f)
        {
            combatTarget = FindNearestEnemyUnit();
        }

        // 2. Nếu không có lính đối thủ, quét tìm nhà đối thủ (TownCenter hoặc Barracks)
        if (combatTarget == null && combatBuildingTarget == null)
        {
            combatBuildingTarget = FindNearestEnemyBuilding();
        }

        // 3. Tự động tấn công
        if (combatTarget != null)
        {
            float distance = Vector3.Distance(transform.position, combatTarget.transform.position);
            if (distance <= attackRange)
            {
                if (navAgent != null && navAgent.isOnNavMesh) navAgent.ResetPath();

                Vector3 lookDir = (combatTarget.transform.position - transform.position).normalized;
                lookDir.y = 0f;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;
                    TriggerAttackAnimation();
                    combatTarget.TakeDamage(attackDamage);
                }
            }
            else
            {
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(combatTarget.transform.position);
                }
            }
        }
        else if (combatBuildingTarget != null)
        {
            float distance = Vector3.Distance(transform.position, combatBuildingTarget.transform.position);
            float buildingAttackRange = attackRange + 1.5f;

            if (distance <= buildingAttackRange)
            {
                if (navAgent != null && navAgent.isOnNavMesh) navAgent.ResetPath();

                Vector3 lookDir = (combatBuildingTarget.transform.position - transform.position).normalized;
                lookDir.y = 0f;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;
                    TriggerAttackAnimation();

                    // Giả lập phá hủy nhà (in log, có thể phá hủy trực tiếp khi đánh đủ số lần)
                    Debug.Log($"[RTS Combat] Attacking enemy building: {combatBuildingTarget.name}!");
                }
            }
            else
            {
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(combatBuildingTarget.transform.position);
                }
            }
        }
    }

    private RTSUnit FindNearestEnemyUnit()
    {
        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude);
        RTSUnit nearest = null;
        float minDist = scanRange;

        foreach (RTSUnit unit in allUnits)
        {
            if (unit == null || unit == this || unit.currentHP <= 0f) continue;
            if (unit.isEnemy != this.isEnemy && unit.transform.position.y > -100f)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = unit;
                }
            }
        }
        return nearest;
    }

    private GameObject FindNearestEnemyBuilding()
    {
        float minDist = scanRange;
        GameObject nearest = null;

        // Quét TownCenters
        foreach (TownCenter tc in TownCenter.AllTownCenters)
        {
            if (tc == null) continue;
            if (tc.isEnemy != this.isEnemy)
            {
                float dist = Vector3.Distance(transform.position, tc.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = tc.gameObject;
                }
            }
        }

        // Quét Barracks
        foreach (Barracks b in Barracks.AllBarracks)
        {
            if (b == null) continue;
            if (b.isEnemy != this.isEnemy)
            {
                float dist = Vector3.Distance(transform.position, b.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = b.gameObject;
                }
            }
        }

        return nearest;
    }

    public void TakeDamage(float damage)
    {
        if (currentHP <= 0f) return;

        currentHP -= damage;
        if (currentHP < 0f) currentHP = 0f;

        Debug.Log($"[RTS Combat] {gameObject.name} took {damage} damage! HP remaining: {currentHP}/{maxHP}");

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[RTS Combat] {gameObject.name} has died!");
        Deselect();

        var col = GetComponent<Collider>();
        if (col != null) Destroy(col);
        if (navAgent != null) Destroy(navAgent);

        StartCoroutine(SinkAndDestroy());
    }

    private System.Collections.IEnumerator SinkAndDestroy()
    {
        float timer = 0f;
        while (timer < 1.5f)
        {
            transform.position += Vector3.down * Time.deltaTime * 0.6f;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void TriggerAttackAnimation()
    {
        Animator animator = GetComponentInChildren<Animator>();
        if (animator == null) return;

        bool hasWork = false;
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == "Work") { hasWork = true; break; }
        }
        if (hasWork)
        {
            animator.SetTrigger("Work");
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

    public void StartHarvesting(ResourceNode node)
    {
        if (unitType != RTSUnitType.Farmer) return;

        targetResourceNode = node;
        currentState = RTSUnitState.MovingToResource;
        
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
                visualBag.transform.SetParent(transform);
                visualBag.transform.localPosition = new Vector3(0f, 1.2f, -0.4f);
                visualBag.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                
                Destroy(visualBag.GetComponent<Collider>());
                
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
            selectionColor = new Color(0f, 1f, 0.2f, 0.9f);
            if (unitName == "Binh Sĩ") unitName = "Nông Dân";
        }
        else if (unitType == RTSUnitType.Soldier)
        {
            selectionColor = new Color(1f, 0.2f, 0f, 0.9f);
            if (unitName == "Binh Sĩ") unitName = "Chiến Binh";
        }

        ApplyFactionColors();
        CreateSelectionRing();
        Deselect();
    }

    private void ApplyFactionColors()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r == null || r is LineRenderer) continue;

            Material mat = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject)) continue;
                if (r.sharedMaterial == null) continue;
                
                if (!r.sharedMaterial.name.Contains("(Instance)"))
                {
                    Material instantiatedMat = new Material(r.sharedMaterial);
                    instantiatedMat.name = r.sharedMaterial.name + " (Instance)";
                    r.sharedMaterial = instantiatedMat;
                }
                mat = r.sharedMaterial;
            }
            else
#endif
            {
                mat = r.material;
            }

            if (mat != null)
            {
                // Kiểm tra xem renderer này có phải là Quad hiển thị Icon trên Minimap không
                bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") || 
                                     mat.name.Contains("MinimapIcon") || mat.name.Contains("Icon");
                
                if (isMinimapIcon)
                {
                    if (isEnemy)
                    {
                        // Quads hiển thị icon địch trên map phải là màu đỏ tươi sáng (bright red: 255, 0, 0)
                        mat.color = new Color(1f, 0f, 0f, 1f);
                    }
                    else
                    {
                        // Quads hiển thị icon ta trên map là màu xanh lá tươi sáng (bright green: 0, 255, 0)
                        mat.color = new Color(0f, 1f, 0f, 1f);
                    }
                }
                else
                {
                    if (isEnemy)
                    {
                        // Địch màu đỏ pastel nhẹ nhàng
                        mat.color = new Color(1.0f, 0.6f, 0.6f, 1f);
                    }
                    else
                    {
                        // Phe ta
                        if (unitType == RTSUnitType.Soldier)
                        {
                            // Nhuộm xanh lam pastel dịu dàng cho Chiến binh
                            mat.color = new Color(0.55f, 0.75f, 1.0f, 1f);
                        }
                        else if (unitType == RTSUnitType.Farmer)
                        {
                            // Nhuộm xanh lá pastel thanh thoát cho Nông dân
                            mat.color = new Color(0.6f, 0.9f, 0.7f, 1f);
                        }
                    }
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(r);
                }
#endif
            }
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
#endif
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // Trong Editor tự động tô màu ngay lập tức
            ApplyFactionColors();
        }
    }
#endif

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