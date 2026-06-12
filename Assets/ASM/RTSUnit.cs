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
    private float rescanTimer = 0f;

    public RTSUnit combatTarget;
    public GameObject combatBuildingTarget;

    public enum RTSUnitState
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingToDeliver,
        Chasing,       // Them: Soldier chasing target
        Attacking,     // Tem: Soldier attacking target
        Dead,          // Them: Dead state (to disable operations)
        Moving         // Them: General moving state for soldiers/units
    }
    public RTSUnitState currentState = RTSUnitState.Idle;

    // A general target that can be either an enemy Unit or an enemy Building
    private GameObject currentTarget;

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

                Vector3 flatUnitPos = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 flatResourcePos = new Vector3(targetResourceNode.transform.position.x, 0f, targetResourceNode.transform.position.z);
                float distToResource = Vector3.Distance(flatUnitPos, flatResourcePos);

                // Sử dụng bán kính hiệu dụng tối thiểu là 3.0 để dễ tiếp cận các cây cao/to
                float effectiveRange = Mathf.Max(targetResourceNode.harvestRange, 3.0f);

                bool reachedResource = distToResource <= effectiveRange ||
                                     (navAgent != null && navAgent.isOnNavMesh && !navAgent.pathPending && navAgent.hasPath && navAgent.remainingDistance <= effectiveRange);

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

                Vector3 flatUnitPosTC = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 flatTCPos = new Vector3(targetTownCenter.transform.position.x, 0f, targetTownCenter.transform.position.z);
                float distToTC = Vector3.Distance(flatUnitPosTC, flatTCPos);

                // Sử dụng khoảng cách hiệu dụng tối thiểu là 3.5 để giao tài nguyên dễ dàng
                float effectiveDeliverRange = Mathf.Max(targetTownCenter.deliverRange, 3.5f);

                bool reachedTC = distToTC <= effectiveDeliverRange ||
                                 (navAgent != null && navAgent.isOnNavMesh && !navAgent.pathPending && navAgent.hasPath && navAgent.remainingDistance <= effectiveDeliverRange);

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
        if (currentState == RTSUnitState.Dead) return;

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case RTSUnitState.Idle:
                // Scan for the highest-priority enemy target in range
                currentTarget = FindPrioritizedEnemyTarget();
                if (currentTarget != null)
                {
                    currentState = RTSUnitState.Chasing;
                }
                break;

            case RTSUnitState.Moving:
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        currentState = RTSUnitState.Idle;
                    }
                }
                else
                {
                    currentState = RTSUnitState.Idle;
                }
                break;

            case RTSUnitState.Chasing:
                if (currentTarget == null)
                {
                    currentState = RTSUnitState.Idle;
                    break;
                }

                // Periodically rescan for closer/better targets to prevent target lock
                rescanTimer -= Time.deltaTime;
                if (rescanTimer <= 0f)
                {
                    rescanTimer = 1.0f; // Rescan every 1 second
                    GameObject betterTarget = FindPrioritizedEnemyTarget();
                    if (betterTarget != null && betterTarget != currentTarget)
                    {
                        currentTarget = betterTarget;
                    }
                }

                // Check if target has died/destroyed mid-chase
                RTSUnit targetUnit = currentTarget.GetComponent<RTSUnit>();
                if (targetUnit != null && targetUnit.currentState == RTSUnitState.Dead)
                {
                    currentTarget = null;
                    currentState = RTSUnitState.Idle;
                    break;
                }

                // Move agent toward the target
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(currentTarget.transform.position);
                }

                // Calculate ranges (buildings have larger attack offsets)
                float effectiveRange = attackRange;
                // Calculate ranges using building's collider if target is a building
                // Calculate ranges using building's collider if target is a building
                float distToTarget;
                Collider targetCollider = currentTarget.GetComponent<Collider>();

                // Ignore Y axis for distance checks
                Vector3 flatSelf = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 flatTarget = new Vector3(currentTarget.transform.position.x, 0f, currentTarget.transform.position.z);

                if (targetUnit == null && targetCollider != null) // It's a building
                {
                    Vector3 flatClosestPoint = targetCollider.ClosestPoint(transform.position);
                    flatClosestPoint.y = 0f;
                    distToTarget = Vector3.Distance(flatSelf, flatClosestPoint);
                }
                else
                {
                    distToTarget = Vector3.Distance(flatSelf, flatTarget);
                }

                // Transition to attacking if in range
                if (isEnemy)
                {
                    Debug.Log($"[Combat Distance Debug] {gameObject.name} Chasing target {currentTarget.name}: distToTarget = {distToTarget:F2}, effectiveRange = {effectiveRange:F2}");
                }

                if (distToTarget <= effectiveRange)
                {
                    if (navAgent != null && navAgent.isOnNavMesh) navAgent.ResetPath();
                    currentState = RTSUnitState.Attacking;
                }
                break;

            case RTSUnitState.Attacking:
                if (currentTarget == null)
                {
                    currentState = RTSUnitState.Idle;
                    break;
                }

                RTSUnit targetUnitAtk = currentTarget.GetComponent<RTSUnit>();
                if (targetUnitAtk != null && targetUnitAtk.currentState == RTSUnitState.Dead)
                {
                    currentTarget = null;
                    currentState = RTSUnitState.Idle;
                    break;
                }

                // Check if target moved out of range
                float currentRange = attackRange;
            // Check if target moved out of range (ignore Y axis)
            float distance;
            Collider targetColliderAtk = currentTarget.GetComponent<Collider>();
            
            Vector3 flatSelfAtk = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 flatTargetAtk = new Vector3(currentTarget.transform.position.x, 0f, currentTarget.transform.position.z);

            if (targetUnitAtk == null && targetColliderAtk != null) // It's a building
            {
                Vector3 flatClosestPointAtk = targetColliderAtk.ClosestPoint(transform.position);
                flatClosestPointAtk.y = 0f;
                distance = Vector3.Distance(flatSelfAtk, flatClosestPointAtk);
            }
            else
            {
                distance = Vector3.Distance(flatSelfAtk, flatTargetAtk);
            }

            // Add a 0.8f buffer so small physics/rounding changes don't cancel the attack instantly
            if (distance > currentRange + 0.8f)
            {
                currentState = RTSUnitState.Chasing;
                break;
            }

                // Rotate to face target
                Vector3 lookDir = (currentTarget.transform.position - transform.position).normalized;
                lookDir.y = 0f;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                // Attack cooldown logic
                if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;

                    // Trigger the attack animation in the Animator
                    Animator animator = GetComponentInChildren<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("Attack");
                    }

                    // Deal damage
                    if (targetUnitAtk != null)
                    {
                        // Pass 'this' so the defender knows who to attack back
                        targetUnitAtk.TakeDamage(attackDamage, this);
                    }
                    else
                    {
                        // Handle building damage if it has a script, or simulate building attack log
                        Debug.Log($"[RTS Combat] {gameObject.name} dealing damage to building: {currentTarget.name}");
                    }
                }
                break;
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

    private GameObject FindPrioritizedEnemyTarget()
    {
        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude);
        float minDist = scanRange;
        GameObject bestTarget = null;

        // 1. Scan for closest enemy Soldier (highest priority)
        foreach (RTSUnit unit in allUnits)
        {
            if (unit == null || unit.currentState == RTSUnitState.Dead || unit.isEnemy == this.isEnemy) continue;
            if (unit.unitType == RTSUnitType.Soldier)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    bestTarget = unit.gameObject;
                }
            }
        }
        if (bestTarget != null) return bestTarget;

        // 2. Scan for closest enemy Farmer (second priority)
        minDist = scanRange;
        foreach (RTSUnit unit in allUnits)
        {
            if (unit == null || unit.currentState == RTSUnitState.Dead || unit.isEnemy == this.isEnemy) continue;
            if (unit.unitType == RTSUnitType.Farmer)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    bestTarget = unit.gameObject;
                }
            }
        }
        if (bestTarget != null) return bestTarget;

        // 3. Scan for closest enemy Building (lowest priority)
        // If there are no enemy units alive on the map, expand scan range so soldiers clean up buildings
        float buildingScanRange = scanRange;
        bool anyEnemyUnitAlive = false;
        foreach (RTSUnit unit in allUnits)
        {
            if (unit != null && unit.currentState != RTSUnitState.Dead && unit.isEnemy != this.isEnemy)
            {
                anyEnemyUnitAlive = true;
                break;
            }
        }

        if (!anyEnemyUnitAlive)
        {
            buildingScanRange = 150f;
        }

        minDist = buildingScanRange;
        // Scan Town Centers
        foreach (TownCenter tc in TownCenter.AllTownCenters)
        {
            if (tc == null || tc.isEnemy == this.isEnemy) continue;
            float dist = Vector3.Distance(transform.position, tc.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                bestTarget = tc.gameObject;
            }
        }
        // Scan Barracks
        foreach (Barracks b in Barracks.AllBarracks)
        {
            if (b == null || b.isEnemy == this.isEnemy) continue;
            float dist = Vector3.Distance(transform.position, b.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                bestTarget = b.gameObject;
            }
        }

        return bestTarget;
    }

    public void TakeDamage(float damage, RTSUnit attacker = null)
    {
        if (currentState == RTSUnitState.Dead) return;

        currentHP -= damage;
        if (currentHP < 0f) currentHP = 0f;


        // Add this line to check what state the enemy is in when hit
        Debug.Log($"[Combat Log] {gameObject.name} (isEnemy: {isEnemy}) hit by {(attacker != null ? attacker.name : "null")}. Current State: {currentState}");
        StartCoroutine(HitFlashRoutine());

        // Retaliate: if idle, or if the attacker is closer than the current target, switch to it
        if (attacker != null)
        {
            bool shouldSwitch = false;
            if (currentState == RTSUnitState.Idle || currentState == RTSUnitState.Moving || currentTarget == null)
            {
                shouldSwitch = true;
            }
            else
            {
                float distToCurrent = Vector3.Distance(transform.position, currentTarget.transform.position);
                float distToAttacker = Vector3.Distance(transform.position, attacker.transform.position);
                // Switch if attacker is closer
                if (distToAttacker < distToCurrent)
                {
                    shouldSwitch = true;
                }
            }

            if (shouldSwitch)
            {
                currentTarget = attacker.gameObject;
                currentState = RTSUnitState.Chasing;
            }

            // Alert nearby allies
            AlertNearbyAllies(attacker);
        }

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void AlertNearbyAllies(RTSUnit attacker)
    {
        if (attacker == null) return;

        float alertRadius = 30f;
        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude);
        foreach (RTSUnit unit in allUnits)
        {
            if (unit == null || unit == this || unit.currentState == RTSUnitState.Dead) continue;
            if (unit.isEnemy == this.isEnemy && unit.unitType == RTSUnitType.Soldier)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist <= alertRadius)
                {
                    if (unit.currentState == RTSUnitState.Idle || unit.currentState == RTSUnitState.Moving || unit.currentTarget == null)
                    {
                        unit.currentTarget = attacker.gameObject;
                        unit.currentState = RTSUnitState.Chasing;
                        Debug.Log($"[Combat Alert] {unit.name} alerted by {this.name} to attack attacker {attacker.name} at distance {dist:F2}!");
                    }
                }
            }
        }
    }

    public void AttackTarget(GameObject target)
    {
        if (currentState == RTSUnitState.Dead) return;

        currentTarget = target;
        currentState = RTSUnitState.Chasing;
    }

    public void MoveToDestination(Vector3 destination)
    {
        if (currentState == RTSUnitState.Dead) return;

        currentTarget = null;
        combatTarget = null;
        combatBuildingTarget = null;
        targetResourceNode = null;
        targetTownCenter = null;

        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(destination);
            navAgent.isStopped = false;
        }

        currentState = RTSUnitState.Moving;
    }

    public void StopUnit()
    {
        if (currentState == RTSUnitState.Dead) return;

        currentTarget = null;
        combatTarget = null;
        combatBuildingTarget = null;
        targetResourceNode = null;
        targetTownCenter = null;

        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.ResetPath();
        }

        currentState = RTSUnitState.Idle;
    }

    private System.Collections.IEnumerator HitFlashRoutine()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        Color[] originalColors = new Color[renderers.Length];

        // Set all materials to solid white to indicate damage
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && !(renderers[i] is LineRenderer))
            {
                originalColors[i] = renderers[i].material.color;
                renderers[i].material.color = Color.white;
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Restore original faction colors
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && !(renderers[i] is LineRenderer))
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    private void Die()
    {
        Debug.Log($"[RTS Combat] {gameObject.name} has died!");
        currentState = RTSUnitState.Dead;
        Deselect();

        // 1. Disable collider so other units can walk through the corpse
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 2. Turn off NavMeshAgent so it stops pathfinding
        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        // 3. Start the unified death timeline coroutine
        StartCoroutine(DeathSequenceRoutine());
    }

    private System.Collections.IEnumerator DeathSequenceRoutine()
    {
        // A. Trigger the death animation
        Animator animator = GetComponentInChildren<Animator>();
        Debug.Log($"[Death Debug] {gameObject.name} is playing Death_B directly. Animator found: {(animator != null ? animator.name : "null")}");
        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.Play("Death_B");
        }

        // B. Wait for the fall animation to complete (approx 2.2s)
        yield return new WaitForSeconds(2.2f);

        // C. Freeze the animator pose
        if (animator != null)
        {
            animator.enabled = false;
        }

        // D. Lie on the ground for 2 seconds
        yield return new WaitForSeconds(2.0f);

        // E. Sink slowly into the ground for 1.5 seconds
        float sinkDuration = 1.5f;
        float sinkSpeed = 0.6f; // Units per second downwards
        float elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // F. Clean up: Object Pool return or fallback to Destroy
        if (UnitPoolManager.Instance != null)
        {
            UnitPoolManager.Instance.ReturnUnit(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetUnit(bool isEnemy)
    {
        // 1. Restore health and state
        currentHP = maxHP;
        currentState = RTSUnitState.Idle;
        this.isEnemy = isEnemy;
        currentTarget = null;
        combatTarget = null;
        combatBuildingTarget = null;
        carriedAmount = 0f;
        carriedType = RTSResourceType.None;

        // 2. Re-enable Physics and Pathfinding
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.enabled = true;
            navAgent.isStopped = false;
        }

        // 3. Re-enable and reset Animator
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Die");
            // Reset to the base Idle/Movement blend tree state
            animator.Play("Movement", 0, 0f);
        }

        // 4. Re-apply colors based on faction
        ApplyFactionColors();
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
        // Snap unit to NavMesh to ensure pre-placed units have isOnNavMesh=true and can navigate
        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 8f, UnityEngine.AI.NavMesh.AllAreas))
            {
                navAgent.enabled = false;
                transform.position = hit.position;
                navAgent.enabled = true;
            }
        }

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
                bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") || r.name.Contains("Quad") || 
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