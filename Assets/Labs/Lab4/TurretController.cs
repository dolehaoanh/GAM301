using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Thiết lập Đối tượng")]
    public Transform partToRotate;  // Kéo phần đầu/thân tháp canh có thể xoay vào đây
    public Transform firePoint;     // Kéo đầu nòng súng vào đây (điểm bắn đạn)
    public GameObject bulletPrefab; // Kéo BulletPrefab của bạn vào đây

    [Header("Thuộc tính")]
    public float range = 8f;          // Tầm bắn
    public float fireRate = 1f;       // Tốc độ bắn (1 viên mỗi giây)
    public float rotationSpeed = 10f;  // Tốc độ nhắm/xoay đầu về phía mục tiêu

    private Transform target;
    private float fireCountdown = 0f;

    void Start()
    {
        // Tối ưu hóa: Tìm mục tiêu gần nhất 5 lần mỗi giây thay vì 60 lần để tiết kiệm CPU!
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

        // 1. Xoay đầu tháp canh mượt mà để hướng về phía kẻ địch (chỉ xoay theo trục Y)
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // 2. Đếm ngược và bắn
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        // Tạo viên đạn tại vị trí nòng súng và theo hướng bắn
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    // Vẽ hình cầu hiển thị tầm bắn xung quanh đầu tháp canh thay vì điểm gốc lệch của đối tượng cha!
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (partToRotate != null)
        {
            Gizmos.DrawWireSphere(partToRotate.position, range);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}