using System.Collections;
using UnityEngine;

public class CameraTheoDoi : MonoBehaviour
{
    public Transform nhanVat;
    public Vector3 saiLechViTri = new Vector3(0, 5, -10);
    public Vector3 saiLechViTriKeThu = new Vector3(0, 6, -12);
    public float chieuCaoMucTieu = 1f;
    public float doMuotDiChuyen = 3f;
    public float doMuotXoay = 3f;
    public float thoiGianQuanSat = 3f;

    private Transform mucTieuHienTai;
    private Coroutine tienTrinhQuanSat;
    private RTSCameraController boDieuKhienRTS;

    private void Start()
    {
        boDieuKhienRTS = GetComponent<RTSCameraController>();
        mucTieuHienTai = nhanVat;
    }

    private void LateUpdate()
    {
        if (mucTieuHienTai != null && (boDieuKhienRTS == null || !boDieuKhienRTS.enabled))
        {
            Vector3 saiLech = (mucTieuHienTai == nhanVat) ? saiLechViTri : saiLechViTriKeThu;
            Vector3 viTriDich = mucTieuHienTai.position + saiLech;
            transform.position = Vector3.Lerp(transform.position, viTriDich, Time.deltaTime * doMuotDiChuyen);

            Vector3 viTriNhin = mucTieuHienTai.position + Vector3.up * chieuCaoMucTieu;
            Vector3 huongNhin = viTriNhin - transform.position;
            if (huongNhin != Vector3.zero)
            {
                Quaternion gocXoayDich = Quaternion.LookRotation(huongNhin);
                transform.rotation = Quaternion.Slerp(transform.rotation, gocXoayDich, Time.deltaTime * doMuotXoay);
            }
        }
    }

    public void QuanSatKeThuTamThoi(Transform keThu)
    {
        if (tienTrinhQuanSat != null)
        {
            StopCoroutine(tienTrinhQuanSat);
        }
        tienTrinhQuanSat = StartCoroutine(ChuoiQuanSatKeThu(keThu));
    }

    private IEnumerator ChuoiQuanSatKeThu(Transform keThu)
    {
        if (boDieuKhienRTS != null)
        {
            boDieuKhienRTS.enabled = false;
        }

        mucTieuHienTai = keThu;
        yield return new WaitForSeconds(thoiGianQuanSat);

        if (boDieuKhienRTS != null)
        {
            boDieuKhienRTS.TargetPosition = transform.position;
            boDieuKhienRTS.enabled = true;
        }

        mucTieuHienTai = nhanVat;
        tienTrinhQuanSat = null;
    }
}
