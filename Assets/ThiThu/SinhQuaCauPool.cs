using System.Collections.Generic;
using UnityEngine;

public class SinhQuaCauPool : MonoBehaviour
{
    public GameObject mauQuaCau;
    public int soLuongPool = 20;
    public float chuKySinh = 0.2f;
    public float lucBanMin = 8f;
    public float lucBanMax = 15f;
    public float saiLechViTriMax = 0.3f;
    public float tiLeKichThuocMin = 0.5f;
    public float tiLeKichThuocMax = 1.5f;
    public float doRongGocBan = 0.8f;

    private List<GameObject> danhSachQuaCau;
    private float boDemThoiGian;

    private void Start()
    {
        danhSachQuaCau = new List<GameObject>();
        for (int i = 0; i < soLuongPool; i++)
        {
            GameObject quaCau = Instantiate(mauQuaCau);
            quaCau.SetActive(false);
            danhSachQuaCau.Add(quaCau);
        }
    }

    private void Update()
    {
        boDemThoiGian += Time.deltaTime;
        if (boDemThoiGian >= chuKySinh)
        {
            boDemThoiGian = 0f;
            SinhQuaCau();
        }
    }

    private void SinhQuaCau()
    {
        GameObject quaCau = LayQuaCauTuPool();
        if (quaCau == null) return;

        float saiLechX = Random.Range(-saiLechViTriMax, saiLechViTriMax);
        float saiLechZ = Random.Range(-saiLechViTriMax, saiLechViTriMax);
        quaCau.transform.position = transform.position + new Vector3(saiLechX, 0f, saiLechZ);
        quaCau.transform.rotation = Quaternion.identity;

        float tiLeKichThuoc = Random.Range(tiLeKichThuocMin, tiLeKichThuocMax);
        quaCau.transform.localScale = Vector3.one * tiLeKichThuoc;

        quaCau.SetActive(true);

        Rigidbody rb = quaCau.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Vector3 huongNgauNhien = new Vector3(Random.Range(-doRongGocBan, doRongGocBan), 1f, Random.Range(-doRongGocBan, doRongGocBan)).normalized;
            float lucBan = Random.Range(lucBanMin, lucBanMax);
            rb.AddForce(huongNgauNhien * lucBan, ForceMode.Impulse);
        }
    }

    private GameObject LayQuaCauTuPool()
    {
        for (int i = 0; i < danhSachQuaCau.Count; i++)
        {
            if (!danhSachQuaCau[i].activeInHierarchy)
            {
                return danhSachQuaCau[i];
            }
        }

        GameObject quaCauMoi = Instantiate(mauQuaCau);
        quaCauMoi.SetActive(false);
        danhSachQuaCau.Add(quaCauMoi);
        return quaCauMoi;
    }
}
