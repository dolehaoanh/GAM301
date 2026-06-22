using System.Collections;
using UnityEngine;

public class DiChuyenObjectThiThu1Cau1 : MonoBehaviour
{
    public Transform viTriA;
    public Transform viTriB;
    public float thoiGianDiChuyen = 1f;
    public float thoiGianCho = 1f;
    public float thoiGianXoay = 2f;
    public float gocXoay = 180f;

    private void Start()
    {
        if (viTriA != null && viTriB != null)
        {
            StartCoroutine(ChuoiDiChuyenVaXoay());
        }
    }

    private IEnumerator ChuoiDiChuyenVaXoay()
    {
        transform.position = viTriA.position;
        float thoiGianTroiQua = 0f;

        while (thoiGianTroiQua < thoiGianDiChuyen)
        {
            if (viTriA != null && viTriB != null)
            {
                transform.position = Vector3.Lerp(viTriA.position, viTriB.position, thoiGianTroiQua / thoiGianDiChuyen);
            }
            thoiGianTroiQua += Time.deltaTime;
            yield return null;
        }

        if (viTriB != null)
        {
            transform.position = viTriB.position;
        }

        yield return new WaitForSeconds(thoiGianCho);

        Quaternion gocXoayBatDau = transform.rotation;
        Quaternion gocXoayDich = gocXoayBatDau * Quaternion.Euler(0f, gocXoay, 0f);
        thoiGianTroiQua = 0f;

        while (thoiGianTroiQua < thoiGianXoay)
        {
            transform.rotation = Quaternion.Slerp(gocXoayBatDau, gocXoayDich, thoiGianTroiQua / thoiGianXoay);
            thoiGianTroiQua += Time.deltaTime;
            yield return null;
        }
        transform.rotation = gocXoayDich;

        Debug.Log("Completed");
    }
}