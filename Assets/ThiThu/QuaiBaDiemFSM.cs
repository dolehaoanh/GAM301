using UnityEngine;

public enum TrangThaiQuaiBaDiem
{
    TuanTra,
    DungLaiNhin
}

public class QuaiBaDiemFSM : MonoBehaviour
{
    public Transform nhanVat;
    public Transform[] diemTuanTra = new Transform[3];
    public float tocDo = 3f;
    public float khoangCachPhatHien = 5f;

    private TrangThaiQuaiBaDiem trangThai = TrangThaiQuaiBaDiem.TuanTra;
    private int chiSoDiemHienTai = 0;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float khoangCach = TinhKhoangCach(transform.position, nhanVat != null ? nhanVat.position : transform.position);

        if (nhanVat != null && khoangCach <= khoangCachPhatHien)
        {
            trangThai = TrangThaiQuaiBaDiem.DungLaiNhin;
        }
        else
        {
            trangThai = TrangThaiQuaiBaDiem.TuanTra;
        }

        switch (trangThai)
        {
            case TrangThaiQuaiBaDiem.TuanTra:
                XuLyTuanTra();
                break;
            case TrangThaiQuaiBaDiem.DungLaiNhin:
                XuLyDungLaiNhin();
                break;
        }
    }

    private void XuLyTuanTra()
    {
        if (diemTuanTra == null || diemTuanTra.Length < 3) return;

        Transform diemMucTieu = diemTuanTra[chiSoDiemHienTai];
        if (diemMucTieu == null) return;

        Vector3 huongDi = diemMucTieu.position - transform.position;
        huongDi.y = 0f;
        Vector3 viTriMoi = transform.position + huongDi.normalized * tocDo * Time.deltaTime;

        if (rb != null)
        {
            rb.MovePosition(viTriMoi);
        }
        else
        {
            transform.position = viTriMoi;
        }

        HuongVe(diemMucTieu.position);

        if (TinhKhoangCach(transform.position, diemMucTieu.position) < 0.5f)
        {
            chiSoDiemHienTai = (chiSoDiemHienTai + 1) % diemTuanTra.Length;
        }
    }

    private void XuLyDungLaiNhin()
    {
        if (nhanVat != null)
        {
            HuongVe(nhanVat.position);
        }
    }

    private float TinhKhoangCach(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(new Vector3(v1.x, 0f, v1.z), new Vector3(v2.x, 0f, v2.z));
    }

    private void HuongVe(Vector3 viTriDich)
    {
        Vector3 huong = viTriDich - transform.position;
        huong.y = 0f;
        if (huong != Vector3.zero)
        {
            transform.forward = huong;
        }
    }
}
