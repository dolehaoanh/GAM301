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
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
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
        if (nhanVat != null && TinhKhoangCachHaiChieu(transform.position, nhanVat.position) <= khoangCachDuoi)
        {
            ChuyenTrangThai(TrangThaiQuai.DuoiTheo);
            return;
        }

        if (diemTuanTraHienTai != null)
        {
            Vector3 viTriMoi = Vector3.MoveTowards(transform.position, diemTuanTraHienTai.position, tocDo * Time.deltaTime);
            viTriMoi = CanBangDoCaoTerrain(viTriMoi);
            transform.position = viTriMoi;
            HuongVe(diemTuanTraHienTai.position);

            if (TinhKhoangCachHaiChieu(transform.position, diemTuanTraHienTai.position) < 0.5f)
            {
                diemTuanTraHienTai = (diemTuanTraHienTai == diemTuanTraA) ? diemTuanTraB : diemTuanTraA;
            }
        }
    }

    private void XuLyDuoiTheo()
    {
        if (nhanVat == null) return;

        float khoangCach = TinhKhoangCachHaiChieu(transform.position, nhanVat.position);

        if (khoangCach > khoangCachDuoi)
        {
            ChuyenTrangThai(TrangThaiQuai.TuanTra);
            return;
        }

        Vector3 viTriMoi = Vector3.MoveTowards(transform.position, nhanVat.position, tocDo * Time.deltaTime);
        viTriMoi = CanBangDoCaoTerrain(viTriMoi);
        transform.position = viTriMoi;
        HuongVe(nhanVat.position);
    }

    private float TinhKhoangCachHaiChieu(Vector3 v1, Vector3 v2)
    {
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
    }

    private Vector3 CanBangDoCaoTerrain(Vector3 viTri)
    {
        if (Terrain.activeTerrain != null)
        {
            Vector3 viTriCucBo = viTri - Terrain.activeTerrain.transform.position;
            viTri.y = Terrain.activeTerrain.SampleHeight(viTriCucBo) + Terrain.activeTerrain.transform.position.y;
        }
        return viTri;
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
