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

    
    public List<RTSUnit> selectedUnits = new List<RTSUnit>();
    public TownCenter selectedTownCenter;
    public Barracks selectedBarracks;

    
    private RTSHUDController hudController;
    private RTSCursorState activeCommand = RTSCursorState.Default;
    private Vector3 firstPatrolPoint;
    private bool waitingForSecondPatrolPoint = false;
    private LineRenderer patrolLineRenderer;

    
    private List<RTSUnit>[] controlGroups = new List<RTSUnit>[10];
    private void Start()
    {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();

        
        hudController = FindAnyObjectByType<RTSHUDController>();

        
        for (int i = 0; i < 10; i++)
        {
            controlGroups[i] = new List<RTSUnit>();
        }

        
        GameObject lineObj = new GameObject("PatrolPathPreviewLine");
        lineObj.transform.SetParent(transform);
        patrolLineRenderer = lineObj.AddComponent<LineRenderer>();
        patrolLineRenderer.startWidth = 0.05f;
        patrolLineRenderer.endWidth = 0.05f;
        patrolLineRenderer.positionCount = 0;
        patrolLineRenderer.useWorldSpace = true;
        
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        patrolLineRenderer.material = lineMaterial;
        patrolLineRenderer.startColor = Color.green;
        patrolLineRenderer.endColor = Color.green;
        patrolLineRenderer.enabled = false;
    }

    private void Update()
    {
        
        HandleControlGroups();

        
        // 1. Left Click down
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (BuildingPlacer.Instance != null && BuildingPlacer.Instance.IsPlacing)
            {
                return;
            }

            if (activeCommand != RTSCursorState.Default)
            {
                HandleCommandClick();
                return;
            }

            startMousePosition = Input.mousePosition;
            isDrawing = true;
        }

        // 2. Left Click up
        if (Input.GetMouseButtonUp(0))
        {
            if (isDrawing)
            {
                isDrawing = false;
                SelectUnitsInBox();
            }
        }

        // 3. Right Click down
        if (Input.GetMouseButtonDown(1))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (BuildingPlacer.Instance != null && BuildingPlacer.Instance.IsPlacing)
            {
                // Let BuildingPlacer handle cancellation
                return;
            }

            if (activeCommand != RTSCursorState.Default)
            {
                SetRTSCursor(RTSCursorState.Default);
                activeCommand = RTSCursorState.Default;
                waitingForSecondPatrolPoint = false;
                return;
            }

            if (selectedUnits.Count > 0)
            {
                MoveSelectedUnits();
            }
        }

        
        if (activeCommand == RTSCursorState.Patrol && waitingForSecondPatrolPoint && patrolLineRenderer != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                patrolLineRenderer.positionCount = 2;
                patrolLineRenderer.SetPosition(0, firstPatrolPoint + Vector3.up * 0.15f);
                patrolLineRenderer.SetPosition(1, hit.point + Vector3.up * 0.15f);
                patrolLineRenderer.enabled = true;
            }
            else
            {
                patrolLineRenderer.enabled = false;
            }
        }
        else
        {
            if (patrolLineRenderer != null) patrolLineRenderer.enabled = false;
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
            if (unit == null || unit.transform.position.y < -100f || unit.isEnemy) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > minX && screenPos.x < maxX && screenPos.y > minY && screenPos.y < maxY)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }

        UpdateHUD(); 
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
            
            RTSUnit unit = hit.collider.GetComponentInParent<RTSUnit>();
            if (unit == null)
            {
                unit = hit.collider.GetComponent<RTSUnit>();
            }

            if (unit != null && !unit.isEnemy)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
            else
            {
                
                TownCenter tc = hit.collider.GetComponentInParent<TownCenter>();
                if (tc == null)
                {
                    tc = hit.collider.GetComponent<TownCenter>();
                }

                if (tc != null)
                {
                    selectedTownCenter = tc;
                }
                else
                {
                    
                    Barracks b = hit.collider.GetComponentInParent<Barracks>();
                    if (b == null)
                    {
                        b = hit.collider.GetComponent<Barracks>();
                    }

                    if (b != null)
                    {
                        selectedBarracks = b;
                    }
                }
            }
        }

        UpdateHUD(); 
    }

    private void SpawnIndicator(Vector3 position, Color color)
    {
        GameObject indicatorObj = new GameObject("CommandIndicator");
        indicatorObj.transform.position = position + Vector3.up * 0.05f;
        MoveIndicator indicator = indicatorObj.AddComponent<MoveIndicator>();
        indicator.SetColor(color);
    }

    private void MoveSelectedUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            int unitCount = selectedUnits.Count;
            if (unitCount == 0) return;

            
            RTSUnit clickedEnemyUnit = hit.collider.GetComponentInParent<RTSUnit>();
            if (clickedEnemyUnit == null) clickedEnemyUnit = hit.collider.GetComponent<RTSUnit>();
            if (clickedEnemyUnit != null && clickedEnemyUnit.isEnemy)
            {
                foreach (RTSUnit unit in selectedUnits)
                {
                    if (unit != null && unit.unitType == RTSUnitType.Soldier)
                    {
                        unit.AttackTarget(clickedEnemyUnit.gameObject);
                    }
                }
                SpawnIndicator(hit.point, Color.red);
                SetRTSCursor(RTSCursorState.Default);
                return;
            }

            TownCenter clickedTC = hit.collider.GetComponentInParent<TownCenter>();
            if (clickedTC == null) clickedTC = hit.collider.GetComponent<TownCenter>();
            if (clickedTC != null && clickedTC.isEnemy)
            {
                foreach (RTSUnit unit in selectedUnits)
                {
                    if (unit != null && unit.unitType == RTSUnitType.Soldier)
                    {
                        unit.AttackTarget(clickedTC.gameObject);
                    }
                }
                SpawnIndicator(hit.point, Color.red);
                SetRTSCursor(RTSCursorState.Default);
                return;
            }

            Barracks clickedB = hit.collider.GetComponentInParent<Barracks>();
            if (clickedB == null) clickedB = hit.collider.GetComponent<Barracks>();
            if (clickedB != null && clickedB.isEnemy)
            {
                foreach (RTSUnit unit in selectedUnits)
                {
                    if (unit != null && unit.unitType == RTSUnitType.Soldier)
                    {
                        unit.AttackTarget(clickedB.gameObject);
                    }
                }
                SpawnIndicator(hit.point, Color.red);
                SetRTSCursor(RTSCursorState.Default);
                return;
            }

            
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
                    SpawnIndicator(hit.point, new Color(1f, 0.8f, 0f));
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

            SpawnIndicator(hit.point, Color.green);
            SetRTSCursor(RTSCursorState.Default);
        }
    }

    private void MoveGroupInGrid(List<RTSUnit> group, Vector3 centerPoint, Quaternion rotation)
    {
        int count = group.Count;
        if (count == 0) return;

        
        bool isSoldierGroup = (group[0] != null && group[0].unitType == RTSUnitType.Soldier);

        int rows = 1;
        int cols = count;

        if (isSoldierGroup)
        {
            
            rows = 1;
            cols = count;
        }
        else
        {
            
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

            if (group[i] != null)
            {
                group[i].MoveToDestination(finalDestination);
            }
        }
    }

    
    private void UpdateHUD()
    {
        if (hudController == null)
        {
            hudController = FindAnyObjectByType<RTSHUDController>();
        }

        if (hudController != null)
        {
            
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
        UpdateHUD(); 
    }

    
    
    

    [Header("Custom Cursor Textures")]
    [Tooltip("Ảnh con trỏ mặc định (Bàn tay/Mũi tên)")]
    public Texture2D defaultCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Di Chuyển (Move)")]
    public Texture2D moveCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Tấn Công (Attack)")]
    public Texture2D attackCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Khai Thác (Gather)")]
    public Texture2D gatherCursor;
    [Tooltip("Ảnh con trỏ khi chọn lệnh Bảo Vệ (Guard)")]
    public Texture2D guardCursor;

    [Tooltip("Tâm của con trỏ chuột (Hotspot)")]
    public Vector2 cursorHotspot = Vector2.zero;

    public enum RTSCursorState
    {
        Default,
        Move,
        Attack,
        Gather,
        Patrol,
        Guard
    }

    // FSM #2 -- CÁC STATE CỦA CON TRỎ CHUỘT
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
            case RTSCursorState.Patrol:
                activeTexture = moveCursor;
                break;
            case RTSCursorState.Guard:
                activeTexture = guardCursor != null ? guardCursor : attackCursor;
                break;
        }

        
        Cursor.SetCursor(activeTexture, cursorHotspot, CursorMode.ForceSoftware);
    }

    [Header("Command SFX Settings")]
    public AudioClip stopCommandSFX;
    public AudioClip attackCommandSFX;

    
    public void OnCommandMove()
    {
        if (selectedUnits.Count == 0) return;

        activeCommand = RTSCursorState.Move;
        SetRTSCursor(RTSCursorState.Move);
    }

    
    public void OnCommandStop()
    {
        if (selectedUnits.Count == 0) return;

        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit == null) continue;
            unit.StopUnit();
        }

        activeCommand = RTSCursorState.Default;
        SetRTSCursor(RTSCursorState.Default); 
        waitingForSecondPatrolPoint = false;
    }

    
    public void OnCommandAttack()
    {
        if (selectedUnits.Count == 0) return;

        activeCommand = RTSCursorState.Attack;
        SetRTSCursor(RTSCursorState.Attack);
    }

    
    public void OnCommandHold()
    {
        if (selectedUnits.Count == 0) return;

        activeCommand = RTSCursorState.Guard;
        SetRTSCursor(RTSCursorState.Guard); 
        waitingForSecondPatrolPoint = false;
    }

    
    public void OnCommandPatrol()
    {
        if (selectedUnits.Count == 0) return;

        activeCommand = RTSCursorState.Patrol;
        waitingForSecondPatrolPoint = false;
        SetRTSCursor(RTSCursorState.Patrol);
    }

    
    public void OnCommandGather()
    {
        if (selectedUnits.Count == 0) return;

        activeCommand = RTSCursorState.Gather;
        SetRTSCursor(RTSCursorState.Gather);
    }

    
    private void HandleControlGroups()
    {
        
        bool isModifierHeld = Input.GetKey(KeyCode.LeftControl) ||
                                Input.GetKey(KeyCode.RightControl) ||
                                Input.GetKey(KeyCode.LeftCommand) ||
                                Input.GetKey(KeyCode.RightCommand);

        
        for (int i = 0; i < 10; i++)
        {
            
            KeyCode key = KeyCode.Alpha0 + i;

            if (Input.GetKeyDown(key))
            {
                if (isModifierHeld)
                {
                    
                    SaveControlGroup(i);
                }
                else
                {
                    
                    RecallControlGroup(i);
                }
                break; 
            }
        }
    }

    
    private void SaveControlGroup(int groupIndex)
    {
        controlGroups[groupIndex].Clear();

        foreach (RTSUnit unit in selectedUnits)
        {
            if (unit != null)
            {
                controlGroups[groupIndex].Add(unit);
            }
        }
    }

    
    private void RecallControlGroup(int groupIndex)
    {
        
        controlGroups[groupIndex].RemoveAll(unit => unit == null);

        
        DeselectAll();

        
        foreach (RTSUnit unit in controlGroups[groupIndex])
        {
            if (unit != null)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }

        
        UpdateHUD();
    }

    private void PlayClickSFX()
    {
        var audioCtrl = FindAnyObjectByType<AssignmentAudioController>();
        if (audioCtrl != null)
        {
            audioCtrl.PlayClickSFX();
        }
    }

    private void MoveSelectedUnitsToPoint(Vector3 point)
    {
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

        Vector3 travelDirection = point - groupCenter;
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
            Vector3 soldierCenter = point;
            Vector3 farmerCenter = point - (formationRotation * Vector3.forward * classGapDistance);

            MoveGroupInGrid(soldiers, soldierCenter, formationRotation);
            MoveGroupInGrid(farmers, farmerCenter, formationRotation);
        }
        else
        {
            MoveGroupInGrid(selectedUnits, point, formationRotation);
        }

        SpawnIndicator(point, Color.green);
    }

    private void HandleCommandClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            PlayClickSFX();

            if (activeCommand == RTSCursorState.Move)
            {
                MoveSelectedUnitsToPoint(hit.point);
                SetRTSCursor(RTSCursorState.Default);
                activeCommand = RTSCursorState.Default;
            }
            else if (activeCommand == RTSCursorState.Attack)
            {
                RTSUnit clickedEnemyUnit = hit.collider.GetComponentInParent<RTSUnit>();
                if (clickedEnemyUnit == null) clickedEnemyUnit = hit.collider.GetComponent<RTSUnit>();

                TownCenter clickedTC = hit.collider.GetComponentInParent<TownCenter>();
                if (clickedTC == null) clickedTC = hit.collider.GetComponent<TownCenter>();

                Barracks clickedB = hit.collider.GetComponentInParent<Barracks>();
                if (clickedB == null) clickedB = hit.collider.GetComponent<Barracks>();

                GameObject target = null;
                if (clickedEnemyUnit != null && clickedEnemyUnit.isEnemy) target = clickedEnemyUnit.gameObject;
                else if (clickedTC != null && clickedTC.isEnemy) target = clickedTC.gameObject;
                else if (clickedB != null && clickedB.isEnemy) target = clickedB.gameObject;

                if (target != null)
                {
                    foreach (RTSUnit unit in selectedUnits)
                    {
                        if (unit != null && unit.unitType == RTSUnitType.Soldier)
                        {
                            unit.AttackTarget(target);
                        }
                    }
                }
                else
                {
                    MoveSelectedUnitsToPoint(hit.point);
                }
                SetRTSCursor(RTSCursorState.Default);
                activeCommand = RTSCursorState.Default;
            }
            else if (activeCommand == RTSCursorState.Guard)
            {
                MoveSelectedUnitsToPoint(hit.point);
                SetRTSCursor(RTSCursorState.Default);
                activeCommand = RTSCursorState.Default;
            }
            else if (activeCommand == RTSCursorState.Gather)
            {
                ResourceNode clickedNode = hit.collider.GetComponentInParent<ResourceNode>();
                if (clickedNode == null) clickedNode = hit.collider.GetComponent<ResourceNode>();

                if (clickedNode != null)
                {
                    foreach (RTSUnit unit in selectedUnits)
                    {
                        if (unit != null && unit.unitType == RTSUnitType.Farmer)
                        {
                            unit.StartHarvesting(clickedNode);
                        }
                    }
                }
                SetRTSCursor(RTSCursorState.Default);
                activeCommand = RTSCursorState.Default;
            }
            else if (activeCommand == RTSCursorState.Patrol)
            {
                if (!waitingForSecondPatrolPoint)
                {
                    firstPatrolPoint = hit.point;
                    waitingForSecondPatrolPoint = true;
                }
                else
                {
                    Vector3 secondPatrolPoint = hit.point;
                    waitingForSecondPatrolPoint = false;

                    foreach (RTSUnit unit in selectedUnits)
                    {
                        if (unit != null)
                        {
                            unit.StartPatrolling(firstPatrolPoint, secondPatrolPoint);
                        }
                    }
                    SetRTSCursor(RTSCursorState.Default);
                    activeCommand = RTSCursorState.Default;
                }
            }
        }
    }
}