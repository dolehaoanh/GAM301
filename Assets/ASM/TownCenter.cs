using UnityEngine;
using System.Collections.Generic;

public class TownCenter : MonoBehaviour
{
    // Danh sách toàn bộ Nhà Chính/Điểm nhận tài nguyên trong Scene
    public static List<TownCenter> AllTownCenters = new List<TownCenter>();

    public float deliverRange = 3.5f; // Khoảng cách nông dân cần tiếp cận để giao hàng

    private void OnEnable()
    {
        if (!AllTownCenters.Contains(this))
        {
            AllTownCenters.Add(this);
        }
    }

    private void OnDisable()
    {
        AllTownCenters.Remove(this);
    }

    // Tìm nhà chính gần nhất với nông dân
    public static TownCenter FindNearest(Vector3 position)
    {
        if (AllTownCenters.Count == 0) return null;

        TownCenter nearest = null;
        float minDistance = float.MaxValue;

        foreach (TownCenter tc in AllTownCenters)
        {
            if (tc == null) continue;
            float dist = Vector3.Distance(position, tc.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = tc;
            }
        }

        return nearest;
    }
}
