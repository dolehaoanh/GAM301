using UnityEngine;

public enum TrangThaiQuai
{
    NgungNghi,
    TuanTra,
    DuoiTheo
}

public class QuaiFSM : MonoBehaviour
{
    public Transform nhanVat;
    public Transform diemTuanTraA;
    public Transform diemTuanTraB;
    public float tocDo = 3f;
    public float khoangCachDuoi = 6f;

    private TrangThaiQuai trangThai = TrangThaiQuai.NgungNghi;
    private Transform diemTuanTraHienTai;

    private void Start()
    {
        diemTuanTraHienTai = diemTuanTraA;
        ChuyenTrangThai(TrangThaiQuai.TuanTra);
    }

    private void Update()
    {
        switch (trangThai)
        {
            case TrangThaiQuai.NgungNghi:
                XuLyNgungNghi();
                break;
            case TrangThaiQuai.TuanTra:
                XuLyTuanTra();
                break;
            case TrangThaiQuai.DuoiTheo:
                XuLyDuoiTheo();
                break;
        }
    }

    private void XuLyNgungNghi()
    {
        if (nhanVat != null && Vector3.Distance(transform.position, nhanVat.position) <= khoangCachDuoi)
        {
            ChuyenTrangThai(TrangThaiQuai.DuoiTheo);
        }
    }

    private void XuLyTuanTra()
    {
        if (nhanVat != null && Vector3.Distance(transform.position, nhanVat.position) <= khoangCachDuoi)
        {
            ChuyenTrangThai(TrangThaiQuai.DuoiTheo);
            return;
        }

        if (diemTuanTraHienTai != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, diemTuanTraHienTai.position, tocDo * Time.deltaTime);
            HuongVe(diemTuanTraHienTai.position);

            if (Vector3.Distance(transform.position, diemTuanTraHienTai.position) < 0.2f)
            {
                diemTuanTraHienTai = (diemTuanTraHienTai == diemTuanTraA) ? diemTuanTraB : diemTuanTraA;
            }
        }
    }

    private void XuLyDuoiTheo()
    {
        if (nhanVat == null) return;

        float khoangCach = Vector3.Distance(transform.position, nhanVat.position);

        if (khoangCach > khoangCachDuoi)
        {
            ChuyenTrangThai(TrangThaiQuai.TuanTra);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, nhanVat.position, tocDo * Time.deltaTime);
        HuongVe(nhanVat.position);
    }

    private void HuongVe(Vector3 viTriDich)
    {
        Vector3 huong = (viTriDich - transform.position).normalized;
        huong.y = 0f;
        if (huong != Vector3.zero)
        {
            transform.forward = huong;
        }
    }

    private void ChuyenTrangThai(TrangThaiQuai trangThaiMoi)
    {
        trangThai = trangThaiMoi;
    }
}
