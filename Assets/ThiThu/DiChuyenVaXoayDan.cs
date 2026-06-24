using System.Collections;
using UnityEngine;

public class DiChuyenVaXoayDan : MonoBehaviour
{
    public Transform viTriA;
    public Transform viTriB;
    public float thoiGianDiChuyen = 2f;
    public float gocXoay = 90f;
    public float thoiGianXoay = 1f;
    public float thoiGianChoSauDiChuyen = 1f;
    public float thoiGianChoSauXoay = 1f;

    private void Start()
    {
        StartCoroutine(ChuoiHanhDong());
    }

    private IEnumerator ChuoiHanhDong()
    {
        transform.position = viTriA.position;
        float thoiGianTroiQua = 0f;
        while (thoiGianTroiQua < thoiGianDiChuyen)
        {
            transform.position = Vector3.Lerp(viTriA.position, viTriB.position, thoiGianTroiQua / thoiGianDiChuyen);
            thoiGianTroiQua += Time.deltaTime;
            yield return null;
        }
        transform.position = viTriB.position;

        yield return new WaitForSeconds(thoiGianChoSauDiChuyen);

        Quaternion gocBatDau = transform.rotation;
        Quaternion gocKetThuc = gocBatDau * Quaternion.Euler(0f, gocXoay, 0f);
        thoiGianTroiQua = 0f;
        while (thoiGianTroiQua < thoiGianXoay)
        {
            transform.rotation = Quaternion.Slerp(gocBatDau, gocKetThuc, thoiGianTroiQua / thoiGianXoay);
            thoiGianTroiQua += Time.deltaTime;
            yield return null;
        }
        transform.rotation = gocKetThuc;

        yield return new WaitForSeconds(thoiGianChoSauXoay);

        Debug.Log("Fire");
    }
}
