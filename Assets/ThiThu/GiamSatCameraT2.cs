using System.Collections;
using UnityEngine;

public class GiamSatCameraT2 : MonoBehaviour
{
    public GameObject cameraNhanVat;
    public GameObject cameraCube;
    public float thoiGianChuyenCanh = 1f;
    public float thoiGianGiuhinh = 2f;

    private Coroutine tienTrinhCutscene;

    private void Start()
    {
        if (cameraNhanVat != null) cameraNhanVat.SetActive(true);
        if (cameraCube != null) cameraCube.SetActive(false);
    }

    public void BatDauCutscene()
    {
        if (tienTrinhCutscene != null)
        {
            StopCoroutine(tienTrinhCutscene);
        }
        tienTrinhCutscene = StartCoroutine(ChuoiCutscene());
    }

    private IEnumerator ChuoiCutscene()
    {
        if (cameraCube != null) cameraCube.SetActive(true);
        if (cameraNhanVat != null) cameraNhanVat.SetActive(false);

        yield return new WaitForSeconds(thoiGianChuyenCanh + thoiGianGiuhinh);

        if (cameraCube != null) cameraCube.SetActive(false);
        if (cameraNhanVat != null) cameraNhanVat.SetActive(true);

        tienTrinhCutscene = null;
    }
}
