using System.Collections.Generic;
using UnityEngine;

public class QuanLyObjectPool : MonoBehaviour
{
    public static QuanLyObjectPool Instance { get; private set; }

    public Dictionary<GameObject, Queue<GameObject>> hangDoiPool = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject LayDoiTuong(GameObject prefab, Vector3 viTri, Quaternion xoay)
    {
        if (prefab == null) return null;

        if (!hangDoiPool.ContainsKey(prefab))
        {
            hangDoiPool[prefab] = new Queue<GameObject>();
        }

        GameObject obj = null;
        if (hangDoiPool[prefab].Count > 0)
        {
            obj = hangDoiPool[prefab].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.position = viTri;
        obj.transform.rotation = xoay;
        obj.SetActive(true);
        return obj;
    }

    public void ThuHoiDoiTuong(GameObject prefab, GameObject obj)
    {
        if (obj == null || prefab == null) return;

        obj.SetActive(false);
        if (!hangDoiPool.ContainsKey(prefab))
        {
            hangDoiPool[prefab] = new Queue<GameObject>();
        }
        hangDoiPool[prefab].Enqueue(obj);
    }
}
