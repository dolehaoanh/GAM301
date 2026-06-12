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

        
        farmerPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(farmerPrefab),
            actionOnGet: OnGetUnit,
            actionOnRelease: OnReleaseUnit,
            actionOnDestroy: Destroy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );

        
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

    
    public GameObject SpawnFarmer(Vector3 position, Quaternion rotation, bool isEnemy)
    {
        GameObject farmer = farmerPool.Get();
        farmer.transform.position = position;
        farmer.transform.rotation = rotation;

        
        RTSUnit unitScript = farmer.GetComponent<RTSUnit>();
        if (unitScript != null)
        {
            unitScript.ResetUnit(isEnemy);
        }

        return farmer;
    }

    
    public GameObject SpawnSoldier(Vector3 position, Quaternion rotation, bool isEnemy)
    {
        GameObject soldier = soldierPool.Get();
        soldier.transform.position = position;
        soldier.transform.rotation = rotation;

        
        RTSUnit unitScript = soldier.GetComponent<RTSUnit>();
        if (unitScript != null)
        {
            unitScript.ResetUnit(isEnemy);
        }

        return soldier;
    }

    
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