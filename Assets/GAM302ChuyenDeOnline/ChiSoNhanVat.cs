using Fusion;
using UnityEngine;

public class ChiSoNhanVat : NetworkBehaviour
{
    [Networked] public int HP { get; set; }
    [Networked] public int MP { get; set; }
    [Networked] public int DiemSo { get; set; }

    [Networked] public bool DangChay { get; set; }
    [Networked] public bool DangTanCong { get; set; }
    [Networked] public bool DangNhay { get; set; }
    [Networked] public bool DangBiThuong { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            HP = 100;
            MP = 50;
            DiemSo = 0;
        }
    }

    private float thoiGianBiThuongConLai = 0f;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            float diChuyenX = Input.GetAxis("Horizontal");
            float diChuyenZ = Input.GetAxis("Vertical");
            DangChay = (Mathf.Abs(diChuyenX) > 0.1f || Mathf.Abs(diChuyenZ) > 0.1f);
        }

        if (DangBiThuong)
        {
            thoiGianBiThuongConLai -= Runner.DeltaTime;
            if (thoiGianBiThuongConLai <= 0f)
            {
                DangBiThuong = false;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_NhanSatThuong(int satThuong, Vector3 viTriNguoiTanCong)
    {
        if (Object.HasStateAuthority)
        {
            HP = Mathf.Max(HP - satThuong, 0);
            Rpc_PhatHoatAnhBiThuong();
            
            // Knockback
            Vector3 huongDayLui = (transform.position - viTriNguoiTanCong);
            huongDayLui.y = 0;
            if (huongDayLui.sqrMagnitude < 0.01f) huongDayLui = -transform.forward;
            
            DiChuyenNhanVat diChuyen = GetComponent<DiChuyenNhanVat>();
            if (diChuyen != null)
            {
                diChuyen.AppDungLucDayLui(huongDayLui, 12f);
            }

            Debug.Log($"Nhan vat {Object.Id} nhan {satThuong} sat thuong! HP con lai: {HP}");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_PhatHoatAnhBiThuong()
    {
        DangBiThuong = true;
        thoiGianBiThuongConLai = 0.5f;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_CongDiem(int diem)
    {
        if (Object.HasStateAuthority)
        {
            DiemSo += diem;
        }
    }
}
