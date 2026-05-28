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

### 2. Các bước cài đặt trên Unity Editor
1.  **Thiết lập Tháp pháo:**
    *   Kéo mô hình tháp `Turret.obj` vào, tạo Material và kéo texture `.png` tương ứng vào để hiển thị màu sắc voxel.
    *   Căn chỉnh **Pivot** của prefab: Tạo Empty GameObject con `TurretHead_Pivot` đặt đúng tâm xoay của đầu súng, kéo lưới đầu súng làm con của nó.
    *   Gắn `TurretController.cs` vào tháp, kéo `TurretHead_Pivot` vào ô `Part To Rotate`, tạo một Empty GameObject làm nòng súng kéo vào `Fire Point`.
2.  **Tạo Đạn (Bullet Prefab):**
    *   Tạo khối Sphere nhỏ làm đạn, gắn script `Bullet.cs`, gắn component **Rigidbody** (bật *Is Kinematic*, tắt *Use Gravity*), kéo làm Prefab và gắn vào biến `Bullet Prefab` của Tháp.
3.  **Tạo Hiệu ứng va chạm (Hit Particle Effect):**
    *   Tạo một **Particle System** phát tia lửa màu vàng/cam khi bắn trúng, cấu hình thời gian ngắn (0.4s), tắt *Looping*, bật *Emission Burst (15 hạt)*, đặt *Stop Action* thành **Destroy** để tự động giải phóng bộ nhớ. Lưu thành Prefab và gắn vào biến `Hit Effect Prefab` trên viên đạn.
