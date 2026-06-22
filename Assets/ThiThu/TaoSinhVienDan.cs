using System.Collections.Generic;
using UnityEngine;

public class TaoSinhVienDan : MonoBehaviour
{
    public GameObject vienDanMau;
    public int soLuongPool = 10;
    public float chuKyBan = 0.333f;

    private List<GameObject> danhSachDan;
    private float boDemThoiGian;

    private void Start()
    {
        danhSachDan = new List<GameObject>();
        for (int i = 0; i < soLuongPool; i++)
        {
            GameObject dan = Instantiate(vienDanMau);
            dan.SetActive(false);
            danhSachDan.Add(dan);
        }
    }

    private void Update()
    {
        boDemThoiGian += Time.deltaTime;
        if (boDemThoiGian >= chuKyBan)
        {
            boDemThoiGian = 0f;
            BanDan();
        }
    }

    private void BanDan()
    {
        GameObject dan = LayDanTuPool();
        if (dan != null)
        {
            dan.transform.position = transform.position;
            dan.transform.rotation = transform.rotation;
            dan.SetActive(true);
        }
    }

    private GameObject LayDanTuPool()
    {
        for (int i = 0; i < danhSachDan.Count; i++)
        {
            if (!danhSachDan[i].activeInHierarchy)
            {
                return danhSachDan[i];
            }
        }

        GameObject danMoi = Instantiate(vienDanMau);
        danMoi.SetActive(false);
        danhSachDan.Add(danMoi);
        return danMoi;
    }
}
