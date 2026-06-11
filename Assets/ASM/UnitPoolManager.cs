using UnityEngine;
using UnityEngine.Pool;

public class UnitPoolManager : MonoBehaviour
{
    public static UnitPoolManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject farmerPrefab;
    public GameObject soldierPrefab;

    [Header("Pool Capacities")]
    public int defaultCapacity = 10;
    public int maxPoolSize = 50;

    private IObjectPool<GameObject> farmerPool;
    private IObjectPool<GameObject> soldierPool;

    private void Awake()
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

        // Initialize Farmer Pool
        farmerPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(farmerPrefab),
            actionOnGet: OnGetUnit,
            actionOnRelease: OnReleaseUnit,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );

        // Initialize Soldier Pool
        soldierPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(soldierPrefab),
            actionOnGet: OnGetUnit,
            actionOnRelease: OnReleaseUnit,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );
    }

    private void OnGetUnit(GameObject unitObj)
    {
        unitObj.SetActive(true);
    }

    private void OnReleaseUnit(GameObject unitObj)
    {
        unitObj.SetActive(false);
    }

    // Call this to spawn a pooled Farmer
    public GameObject SpawnFarmer(Vector3 position, Quaternion rotation, bool isEnemy)
    {
        GameObject farmer = farmerPool.Get();
        farmer.transform.position = position;
        farmer.transform.rotation = rotation;

        // Initialize unit states
        RTSUnit unitScript = farmer.GetComponent<RTSUnit>();
        if (unitScript != null)
        {
            unitScript.ResetUnit(isEnemy);
        }

        return farmer;
    }

    // Call this to spawn a pooled Soldier
    public GameObject SpawnSoldier(Vector3 position, Quaternion rotation, bool isEnemy)
    {
        GameObject soldier = soldierPool.Get();
        soldier.transform.position = position;
        soldier.transform.rotation = rotation;

        // Initialize unit states
        RTSUnit unitScript = soldier.GetComponent<RTSUnit>();
        if (unitScript != null)
        {
            unitScript.ResetUnit(isEnemy);
        }

        return soldier;
    }

    // Call this to recycle the unit instead of Destroying it
    public void ReturnUnit(RTSUnit unit)
    {
        if (unit.unitType == RTSUnitType.Farmer)
        {
            farmerPool.Release(unit.gameObject);
        }
        else if (unit.unitType == RTSUnitType.Soldier)
        {
            soldierPool.Release(unit.gameObject);
        }
    }
}