# Lời giải Lab 4 - GAM301

## Bài tập 1: Tạo Navigation và AI di chuyển

### 1. Mã nguồn hoàn chỉnh (`EnemyAI.cs`)
```csharp
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform destination;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (destination != null)
        {
            agent.SetDestination(destination.position);
        }
    }

    public void InitializeDestination(Transform target)
    {
        destination = target;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null && destination != null)
        {
            agent.SetDestination(destination.position);
        }
    }
}
```

### 2. Các bước cài đặt trên Unity Editor
1.  **Thiết lập Bản đồ (Bake NavMesh):**
    *   Kéo mô hình `CubeTer.fbx` vào cảnh, gắn Material sử dụng texture `texture.png`.
    *   Tích chọn **Static** (Yes, change children) trên `CubeTer`.
    *   Mở **Window > AI > Navigation (Obsolete)**, chọn tab **Bake** và click **Bake**. Đường đi màu vàng sẽ phủ màu xanh NavMesh.
2.  **Tạo Cổng kết thúc (End Portal & Game Over):**
    *   Tạo GameObject rỗng đặt tên là `EndPortal`, gắn component **Box Collider** (tích chọn **Is Trigger**).
    *   Gắn script `GameOverTrigger.cs` để ghi log `GAME OVER` khi phát hiện vật thể có tag `"Enemy"` đi vào.
3.  **Tạo Enemy:**
    *   Tạo khối Sphere đặt tên là `Monster`, gắn các component: **NavMesh Agent**, **Rigidbody** (bỏ *Use Gravity*, tích *Is Kinematic*), và script `EnemyAI.cs`.
    *   Đặt Tag cho `Monster` là **`Enemy`**. Kéo `EndPortal` vào biến `Destination` của script `EnemyAI`.

---

## Bài tập 2: Súng máy (Defense Turret)

### 1. Mã nguồn hoàn chỉnh (`TurretController.cs` và `Bullet.cs`)

#### Script điều khiển Tháp pháo (`TurretController.cs`)
```csharp
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Setup Fields")]
    public Transform partToRotate;  // Phần đầu xoay của tháp
    public Transform firePoint;     // Điểm nòng súng bắn đạn ra
    public GameObject bulletPrefab;

    [Header("Attributes")]
    public float range = 8f;          // Tầm bắn
    public float fireRate = 1f;       // Số viên đạn bắn ra mỗi giây
    public float rotationSpeed = 10f;  // Tốc độ xoay ngắm mục tiêu

    private Transform target;
    private float fireCountdown = 0f;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null) return;

        // Xoay đầu tháp ngắm mục tiêu mượt mà
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // Bắn đạn
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (partToRotate != null)
        {
            Gizmos.DrawWireSphere(partToRotate.position, range);
        }
    }
}
```

#### Script chuyển động của đạn (`Bullet.cs`)
```csharp
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public GameObject hitEffectPrefab; // Hiệu ứng va chạm

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Tìm điểm va chạm gần nhất trên bề mặt
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            }

            MonsterHP monsterHealth = other.GetComponent<MonsterHP>();
            if (monsterHealth != null)
            {
                monsterHealth.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }
}
```

---

## Bài tập 3: Viết cấu trúc FSM cho Quái

### 1. Mã nguồn hoàn chỉnh (`MonsterFSM.cs`)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    public enum FSMState { NormalWalk, ActionTriggered, SpeedBoost, Jump }

    [Header("State Machine")]
    public FSMState currentState = FSMState.NormalWalk;

    [Header("Journey Tracking")]
    public Transform destination;
    private Vector3 startPosition;
    private float totalDistance;
    private bool hasTriggeredAction = false;

    private NavMeshAgent agent;
    private float normalSpeed;
    private float normalAcceleration;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = agent.speed;
        normalAcceleration = agent.acceleration;
        agent.autoTraverseOffMeshLink = false;
        startPosition = transform.position;

        if (destination != null)
        {
            totalDistance = Vector3.Distance(startPosition, destination.position);
            agent.SetDestination(destination.position);
        }
    }

    public void InitializeDestination(Transform target)
    {
        destination = target;
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        normalSpeed = agent.speed;
        normalAcceleration = agent.acceleration;
        startPosition = transform.position;

        if (agent != null && destination != null)
        {
            totalDistance = Vector3.Distance(startPosition, destination.position);
            agent.SetDestination(destination.position);
        }
    }

    void Update()
    {
        if (destination == null) return;

        if (agent != null && agent.isOnOffMeshLink && currentState != FSMState.Jump)
        {
            StartCoroutine(LinkJumpRoutine());
        }

        switch (currentState)
        {
            case FSMState.NormalWalk:
                MonitorDistance();
                break;
        }
    }

    void MonitorDistance()
    {
        if (destination == null || hasTriggeredAction) return;

        float remainingDistance = Vector3.Distance(transform.position, destination.position);
        float percentageCompleted = 1f - (remainingDistance / totalDistance);

        if (percentageCompleted >= 0.33f)
        {
            hasTriggeredAction = true;
            currentState = FSMState.ActionTriggered;
            TriggerRandomAction();
        }
    }

    void TriggerRandomAction()
    {
        int choice = Random.Range(0, 2);
        if (choice == 0) StartCoroutine(SpeedBoostRoutine());
        else StartCoroutine(JumpRoutine());
    }

    IEnumerator SpeedBoostRoutine()
    {
        currentState = FSMState.SpeedBoost;
        agent.acceleration = 9999f; // Tăng tốc độ phản hồi tức thì
        agent.speed = normalSpeed * 4.44f;
        yield return new WaitForSeconds(2f);
        agent.speed = normalSpeed;
        agent.acceleration = normalAcceleration;
        currentState = FSMState.NormalWalk;
    }

    IEnumerator JumpRoutine()
    {
        currentState = FSMState.Jump;
        agent.enabled = false;
        Vector3 jumpStart = transform.position;
        Vector3 jumpEnd = jumpStart + transform.forward * 3f;

        float elapsedTime = 0f;
        float jumpDuration = 1f;
        float jumpHeight = 2.5f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            Vector3 currentPos = Vector3.Lerp(jumpStart, jumpEnd, t);
            currentPos.y = Mathf.Lerp(jumpStart.y, jumpEnd.y, t) + (4f * jumpHeight * t * (1f - t));
            transform.position = currentPos;
            yield return null;
        }

        transform.position = jumpEnd;
        agent.enabled = true;
        if (destination != null) agent.SetDestination(destination.position);
        currentState = FSMState.NormalWalk;
    }

    IEnumerator LinkJumpRoutine()
    {
        currentState = FSMState.Jump;
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 jumpStart = transform.position;
        Vector3 jumpEnd = data.endPos;

        float elapsedTime = 0f;
        float jumpDuration = 0.8f;
        float jumpHeight = 2.0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            Vector3 currentPos = Vector3.Lerp(jumpStart, jumpEnd, t);
            currentPos.y = Mathf.Lerp(jumpStart.y, jumpEnd.y, t) + (4f * jumpHeight * t * (1f - t));
            transform.position = currentPos;
            yield return null;
        }

        transform.position = jumpEnd;
        agent.CompleteOffMeshLink();
        currentState = FSMState.NormalWalk;
    }
}
```

---

## Bài tập 4: Vượt chướng ngại vật & Hố ảo (NavMesh Links)

### 1. Ý tưởng giải pháp
*   **Hố ảo (Virtual Gap):** Đặt một Cube rỗng có gắn **NavMesh Obstacle** (bật **Carve**, tắt *Carve Only Stationary*, tăng `Size Y` lên 5) đè lên đường đi. Ẩn Renderer của Cube để tạo cảm giác có hố cắt ngang qua đường đi.
*   **Chướng ngại vật (Fence Obstacle):** Đặt Cube chắn ngang đường, đặt **Static** và Bake lại NavMesh để tạo vùng trống không thể đi qua.
*   **Nhảy vượt chướng ngại vật:** Sử dụng component **NavMesh Link** nối giữa điểm trước và điểm sau của hố/vật cản. Nhờ sự kiện `agent.isOnOffMeshLink` được phát hiện trong `MonsterFSM.cs`, quái sẽ tự động thực hiện một cú nhảy parabol mượt mà khi đi qua các liên kết này!
