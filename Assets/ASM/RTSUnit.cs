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
    public bool isEnemy = false; 

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

    [Header("Audio Settings")]
    public AudioClip attackClip;
    public AudioClip hurtClip;
    private AudioSource sfxAudioSource;

    public enum RTSUnitState
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingToDeliver,
        Chasing,       
        Attacking,     
        Dead,          
        Moving,
        Patrolling
    }
    public RTSUnitState currentState = RTSUnitState.Idle;

    [Header("Patrol Settings")]
    public Vector3 patrolPointA;
    public Vector3 patrolPointB;
    private bool patrolGoingToB = true;

    
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

            case RTSUnitState.Patrolling:
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    bool reached = !navAgent.pathPending && 
                                   (navAgent.remainingDistance <= 2.2f || 
                                    (navAgent.remainingDistance <= 5.0f && navAgent.velocity.sqrMagnitude < 0.1f));
                    if (reached)
                    {
                        patrolGoingToB = !patrolGoingToB;
                        navAgent.SetDestination(patrolGoingToB ? patrolPointB : patrolPointA);
                    }
                }
                break;

            case RTSUnitState.MovingToResource:
                if (targetResourceNode == null)
                {
                    currentState = RTSUnitState.Idle;
                    break;
                }

                
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
                if (gatherTimer >= 1.2f) 
                {
                    gatherTimer = 0f;

                    
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
                        Vector3 sparkPos = (transform.position + targetResourceNode.transform.position) * 0.5f + Vector3.up * 1f;
                        RTSEffects.SpawnHarvestEffect(sparkPos, targetResourceNode.resourceType);
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

                
                float effectiveDeliverRange = Mathf.Max(targetTownCenter.deliverRange, 3.5f);

                bool reachedTC = distToTC <= effectiveDeliverRange ||
                                 (navAgent != null && navAgent.isOnNavMesh && !navAgent.pathPending && navAgent.hasPath && navAgent.remainingDistance <= effectiveDeliverRange);

                if (reachedTC)
                {
                    
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

            case RTSUnitState.Patrolling:
                currentTarget = FindPrioritizedEnemyTarget();
                if (currentTarget != null)
                {
                    currentState = RTSUnitState.Chasing;
                    break;
                }

                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    bool reached = !navAgent.pathPending && 
                                   (navAgent.remainingDistance <= 2.2f || 
                                    (navAgent.remainingDistance <= 5.0f && navAgent.velocity.sqrMagnitude < 0.1f));
                    if (reached)
                    {
                        patrolGoingToB = !patrolGoingToB;
                        navAgent.SetDestination(patrolGoingToB ? patrolPointB : patrolPointA);
                    }
                }
                break;

            case RTSUnitState.Chasing:
                if (currentTarget == null)
                {
                    currentState = RTSUnitState.Idle;
                    break;
                }

                
                rescanTimer -= Time.deltaTime;
                if (rescanTimer <= 0f)
                {
                    rescanTimer = 1.0f; 
                    GameObject betterTarget = FindPrioritizedEnemyTarget();
                    if (betterTarget != null && betterTarget != currentTarget)
                    {
                        currentTarget = betterTarget;
                    }
                }

                
                RTSUnit targetUnit = currentTarget.GetComponent<RTSUnit>();
                if (targetUnit != null && targetUnit.currentState == RTSUnitState.Dead)
                {
                    currentTarget = null;
                    currentState = RTSUnitState.Idle;
                    break;
                }

                
                if (navAgent != null && navAgent.isOnNavMesh)
                {
                    navAgent.SetDestination(currentTarget.transform.position);
                }

                
                float effectiveRange = attackRange;
                
                
                float distToTarget;
                Collider targetCollider = currentTarget.GetComponent<Collider>();

                
                Vector3 flatSelf = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 flatTarget = new Vector3(currentTarget.transform.position.x, 0f, currentTarget.transform.position.z);

                if (targetUnit == null && targetCollider != null) 
                {
                    Vector3 flatClosestPoint = targetCollider.ClosestPoint(transform.position);
                    flatClosestPoint.y = 0f;
                    distToTarget = Vector3.Distance(flatSelf, flatClosestPoint);
                }
                else
                {
                    distToTarget = Vector3.Distance(flatSelf, flatTarget);
                }

                
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

                
                float currentRange = attackRange;
            
            float distance;
            Collider targetColliderAtk = currentTarget.GetComponent<Collider>();
            
            Vector3 flatSelfAtk = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 flatTargetAtk = new Vector3(currentTarget.transform.position.x, 0f, currentTarget.transform.position.z);

            if (targetUnitAtk == null && targetColliderAtk != null) 
            {
                Vector3 flatClosestPointAtk = targetColliderAtk.ClosestPoint(transform.position);
                flatClosestPointAtk.y = 0f;
                distance = Vector3.Distance(flatSelfAtk, flatClosestPointAtk);
            }
            else
            {
                distance = Vector3.Distance(flatSelfAtk, flatTargetAtk);
            }

            
            if (distance > currentRange + 0.8f)
            {
                currentState = RTSUnitState.Chasing;
                break;
            }

                
                Vector3 lookDir = (currentTarget.transform.position - transform.position).normalized;
                lookDir.y = 0f;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                
                if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;

                    
                    Animator animator = GetComponentInChildren<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("Attack");
                    }

                    
                    if (sfxAudioSource != null && attackClip != null)
                    {
                        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
                        sfxAudioSource.PlayOneShot(attackClip, sfxVol);
                    }

                    
                    if (targetUnitAtk != null)
                    {
                        
                        targetUnitAtk.TakeDamage(attackDamage, this);
                        RTSEffects.SpawnImpactEffect(targetUnitAtk.transform.position + Vector3.up * 1f);
                    }
                    else
                    {
                        
                        if (currentTarget != null)
                        {
                            TownCenter tc = currentTarget.GetComponent<TownCenter>();
                            if (tc != null)
                            {
                                tc.TakeDamage(attackDamage);
                            }
                            else
                            {
                                Barracks b = currentTarget.GetComponent<Barracks>();
                                if (b != null)
                                {
                                    b.TakeDamage(attackDamage);
                                }
                            }
                            RTSEffects.SpawnImpactEffect(currentTarget.transform.position + Vector3.up * 1.5f);
                        }
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

        
        
        float buildingScanRange = scanRange;
        minDist = buildingScanRange;
        
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

        
        if (sfxAudioSource != null && hurtClip != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            sfxAudioSource.PlayOneShot(hurtClip, sfxVol);
        }


        
        Debug.Log($"[Combat Log] {gameObject.name} (isEnemy: {isEnemy}) hit by {(attacker != null ? attacker.name : "null")}. Current State: {currentState}");
        StartCoroutine(HitFlashRoutine());

        
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

    public void StartPatrolling(Vector3 pointA, Vector3 pointB)
    {
        if (currentState == RTSUnitState.Dead) return;

        currentTarget = null;
        combatTarget = null;
        combatBuildingTarget = null;
        targetResourceNode = null;
        targetTownCenter = null;

        patrolPointA = pointA;
        patrolPointB = pointB;
        patrolGoingToB = true;

        currentState = RTSUnitState.Patrolling;

        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(patrolPointB);
            navAgent.isStopped = false;
        }
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

        
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && !(renderers[i] is LineRenderer))
            {
                originalColors[i] = renderers[i].material.color;
                renderers[i].material.color = Color.white;
            }
        }

        yield return new WaitForSeconds(0.1f);

        
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

        
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        
        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        
        StartCoroutine(DeathSequenceRoutine());
    }

    private System.Collections.IEnumerator DeathSequenceRoutine()
    {
        
        Animator animator = GetComponentInChildren<Animator>();
        Debug.Log($"[Death Debug] {gameObject.name} is playing Death_B directly. Animator found: {(animator != null ? animator.name : "null")}");
        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.Play("Death_B");
        }

        
        yield return new WaitForSeconds(2.2f);

        
        if (animator != null)
        {
            animator.enabled = false;
        }

        
        yield return new WaitForSeconds(2.0f);

        
        float sinkDuration = 1.5f;
        float sinkSpeed = 0.6f; 
        float elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        
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
        
        currentHP = maxHP;
        currentState = RTSUnitState.Idle;
        this.isEnemy = isEnemy;
        currentTarget = null;
        combatTarget = null;
        combatBuildingTarget = null;
        carriedAmount = 0f;
        carriedType = RTSResourceType.None;

        
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        if (navAgent == null) navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.enabled = true;
            navAgent.isStopped = false;
        }

        
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Die");
            
            animator.Play("Movement", 0, 0f);
        }

        
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
        
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.spatialBlend = 0.5f; 
        sfxAudioSource.minDistance = 5f;
        sfxAudioSource.maxDistance = 50f;

        
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
                
                bool isMinimapIcon = r.name.Contains("Minimap") || r.name.Contains("Icon") || r.name.Contains("Quad") || 
                                     mat.name.Contains("MinimapIcon") || mat.name.Contains("Icon");

                if (isMinimapIcon)
                {
                    if (isEnemy)
                    {
                        
                        mat.color = new Color(1f, 0f, 0f, 1f);
                    }
                    else
                    {
                        
                        mat.color = new Color(0f, 1f, 0f, 1f);
                    }
                }
                else
                {
                    if (isEnemy)
                    {
                        
                        mat.color = new Color(1.0f, 0.6f, 0.6f, 1f);
                    }
                    else
                    {
                        
                        if (unitType == RTSUnitType.Soldier)
                        {
                            
                            mat.color = new Color(0.55f, 0.75f, 1.0f, 1f);
                        }
                        else if (unitType == RTSUnitType.Farmer)
                        {
                            
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