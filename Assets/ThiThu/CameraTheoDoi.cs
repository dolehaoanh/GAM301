using System.Collections;
using UnityEngine;

public class CameraTheoDoi : MonoBehaviour
{
    public GameObject cameraNhanVat;
    public GameObject cameraKeThu;
    public float thoiGianQuanSat = 3f;

    private Coroutine tienTrinhQuanSat;

    public void QuanSatKeThuTamThoi()
    {
        if (tienTrinhQuanSat != null)
        {
            StopCoroutine(tienTrinhQuanSat);
        }
        tienTrinhQuanSat = StartCoroutine(ChuoiQuanSatKeThu());
    }

    private IEnumerator ChuoiQuanSatKeThu()
    {
        if (cameraKeThu != null)
        {
            cameraKeThu.SetActive(true);
        }

        yield return new WaitForSeconds(thoiGianQuanSat);

        if (cameraKeThu != null)
        {
            cameraKeThu.SetActive(false);
        }

        tienTrinhQuanSat = null;
    }
}
