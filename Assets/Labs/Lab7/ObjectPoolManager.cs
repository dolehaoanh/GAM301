using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pool Settings")]
    public GameObject bulletPrefab;
    public int defaultCapacity = 30;
    public int maxPoolSize = 100;

    [Header("Debug Info (Xem trong Runtime)")]
    [Tooltip("Số lượng đạn đang hoạt động ngoài Scene")]
    [SerializeField] private int activeBullets;
    [Tooltip("Số lượng đạn đang nằm chờ trong Pool")]
    [SerializeField] private int inactiveBullets;
    [Tooltip("Tổng số đạn đã được khởi tạo")]
    [SerializeField] private int totalBullets;

    private IObjectPool<GameObject> bulletPool;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bulletPool = new ObjectPool<GameObject>(
            createFunc: OnCreateBullet,
            actionOnGet: OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );
    }

    void Update()
    {
        // Cập nhật các chỉ số lên Inspector thời gian thực
        if (bulletPool != null)
        {
            var pool = bulletPool as ObjectPool<GameObject>;
            if (pool != null)
            {
                activeBullets = pool.CountActive;
                inactiveBullets = pool.CountInactive;
                totalBullets = pool.CountAll;
            }
        }
    }

    private GameObject OnCreateBullet()
    {
        GameObject bulletInstance = Instantiate(bulletPrefab);
        bulletInstance.SetActive(false);
        return bulletInstance;
    }

    private void OnGetBullet(GameObject bulletInstance)
    {
        bulletInstance.SetActive(true);
    }

    private void OnReleaseBullet(GameObject bulletInstance)
    {
        bulletInstance.SetActive(false);
    }

    private void OnDestroyBullet(GameObject bulletInstance)
    {
        Destroy(bulletInstance);
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = bulletPool.Get();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bulletPool.Release(bullet);
    }
}