using System.Collections;
using UnityEngine;

public class CameraQuayCutscene : MonoBehaviour
{
    public GameObject cameraNhanVat;
    public GameObject cameraCutscene;
    public float thoiGianCutscene = 2f;

    private Coroutine tienTrinhCutscene;

    private void Start()
    {
        if (cameraNhanVat != null) cameraNhanVat.SetActive(true);
        if (cameraCutscene != null) cameraCutscene.SetActive(false);
    }

    public void ChayQuayCutscene(Transform viTriItem)
    {
        if (tienTrinhCutscene != null)
        {
            StopCoroutine(tienTrinhCutscene);
        }
        tienTrinhCutscene = StartCoroutine(ChuoiCutscene(viTriItem));
    }

    private IEnumerator ChuoiCutscene(Transform viTriItem)
    {
        if (cameraCutscene != null)
        {
            cameraCutscene.transform.position = viTriItem.position;
            cameraCutscene.SetActive(true);
        }
        if (cameraNhanVat != null) cameraNhanVat.SetActive(false);

        yield return new WaitForSeconds(thoiGianCutscene);

        if (cameraCutscene != null) cameraCutscene.SetActive(false);
        if (cameraNhanVat != null) cameraNhanVat.SetActive(true);

        tienTrinhCutscene = null;
    }
}
