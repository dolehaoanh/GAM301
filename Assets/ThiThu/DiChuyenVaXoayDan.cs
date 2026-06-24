using System.Collections;
using UnityEngine;

public class DiChuyenVaXoayDan : MonoBehaviour
{
    public Transform viTriA;
    public Transform viTriB;
    public float thoiGianDiChuyen = 2f;
    public float gocXoay = 90f;

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

        yield return new WaitForSeconds(1f);

        transform.Rotate(0f, gocXoay, 0f);

        yield return new WaitForSeconds(1f);

        Debug.Log("Fire");
    }
}
