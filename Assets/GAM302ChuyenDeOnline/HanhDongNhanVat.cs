using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HanhDongNhanVat : NetworkBehaviour
{
    private ChiSoNhanVat chiSo;
    
    public AudioSource nguonAmThanh;
    public AudioClip amThanhNhay;
    public AudioClip amThanhTanCong;
    public GameObject prefabHieuUngVaCham;

    public float thoiGianTanCong = 0.5f;
    public int satThuongTanCong = 15;
    public float banKinhTanCong = 2.0f;

    private float thoiGianTanCongConLai = 0f;
    
    private void Awake()
    {
        chiSo = GetComponent<ChiSoNhanVat>();
        nguonAmThanh = GetComponent<AudioSource>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority || Object.HasStateAuthority)
        {
            if (chiSo != null && chiSo.HP <= 0)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Rpc_Nhay();
            }
            
            bool phimTanCongDown = Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0);
            
            if (phimTanCongDown)
            {
                if (chiSo != null && !chiSo.DangNhay && !chiSo.DangTanCong)
                {
                    Rpc_TanCong();
                }
                else if (chiSo == null)
                {
                    Rpc_TanCong();
                }
            }

            if (chiSo != null && chiSo.DangTanCong)
            {
                thoiGianTanCongConLai -= Runner.DeltaTime;
                if (thoiGianTanCongConLai <= 0f)
                {
                    chiSo.DangTanCong = false;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.G) && chiSo != null)
            {
                chiSo.Rpc_CongDiem(1);
            }

            if (Input.GetKeyUp(KeyCode.Space) && chiSo != null) chiSo.DangNhay = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Nhay()
    {
        if (chiSo != null)
        {
            chiSo.DangNhay = true;
        }
        
        if (nguonAmThanh != null && amThanhNhay != null)
        {
            nguonAmThanh.PlayOneShot(amThanhNhay);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TanCong()
    {
        if (chiSo != null)
        {
            chiSo.DangTanCong = true;
            thoiGianTanCongConLai = thoiGianTanCong;
        }
        
        if (nguonAmThanh != null && amThanhTanCong != null)
        {
            nguonAmThanh.PlayOneShot(amThanhTanCong);
        }

        Vector3 viTriTanCong = transform.position + transform.forward * 1.0f;
        Rpc_TaoHieuUngVaCham(viTriTanCong);

        if (Object.HasStateAuthority)
        {
            KiemTraGâySatThuongBuTre(viTriTanCong);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TaoHieuUngVaCham(Vector3 viTri)
    {
        if (prefabHieuUngVaCham != null)
        {
            if (QuanLyObjectPool.Instance != null)
            {
                QuanLyObjectPool.Instance.LayDoiTuong(prefabHieuUngVaCham, viTri, Quaternion.identity);
            }
            else
            {
                Instantiate(prefabHieuUngVaCham, viTri, Quaternion.identity);
            }
        }
    }

    private void KiemTraGâySatThuongBuTre(Vector3 viTriTanCong)
    {
        bool daGaySatThuong = false;

        if (Runner != null && Runner.LagCompensation != null)
        {
            List<LagCompensatedHit> danhSachHit = new List<LagCompensatedHit>();
            int hitCount = Runner.LagCompensation.OverlapSphere(
                viTriTanCong,
                banKinhTanCong,
                Object.InputAuthority,
                danhSachHit,
                options: HitOptions.IncludePhysX
            );

            if (hitCount > 0)
            {
                foreach (var hit in danhSachHit)
                {
                    GameObject objHit = hit.GameObject != null ? hit.GameObject : (hit.Hitbox != null ? hit.Hitbox.gameObject : null);
                    if (objHit == null) continue;

                    ChiSoNhanVat mucTieuChiSo = objHit.GetComponentInParent<ChiSoNhanVat>();
                    if (mucTieuChiSo == null)
                    {
                        mucTieuChiSo = objHit.GetComponent<ChiSoNhanVat>();
                    }

                    if (mucTieuChiSo != null && mucTieuChiSo.Object != this.Object)
                    {
                        mucTieuChiSo.Rpc_NhanSatThuong(satThuongTanCong, transform.position);
                        if (chiSo != null)
                        {
                            chiSo.Rpc_CongDiem(1);
                        }
                        daGaySatThuong = true;
                    }
                }
            }
        }

        if (!daGaySatThuong)
        {
            Collider[] hitColliders = Physics.OverlapSphere(viTriTanCong, banKinhTanCong);
            foreach (var hitCollider in hitColliders)
            {
                ChiSoNhanVat mucTieuChiSo = hitCollider.GetComponentInParent<ChiSoNhanVat>();
                if (mucTieuChiSo == null)
                {
                    mucTieuChiSo = hitCollider.GetComponent<ChiSoNhanVat>();
                }

                if (mucTieuChiSo != null && mucTieuChiSo.Object != this.Object)
                {
                    mucTieuChiSo.Rpc_NhanSatThuong(satThuongTanCong, transform.position);
                    if (chiSo != null)
                    {
                        chiSo.Rpc_CongDiem(1);
                    }
                    daGaySatThuong = true;
                }
            }
        }

        if (!daGaySatThuong)
        {
            ChiSoNhanVat[] tatCaNhanVat = UnityEngine.Object.FindObjectsByType<ChiSoNhanVat>(FindObjectsInactive.Exclude);
            foreach (var nv in tatCaNhanVat)
            {
                if (nv.Object != null && nv.Object != this.Object)
                {
                    float khoangCach = Vector3.Distance(transform.position, nv.transform.position);
                    if (khoangCach <= banKinhTanCong + 1.0f)
                    {
                        nv.Rpc_NhanSatThuong(satThuongTanCong, transform.position);
                        if (chiSo != null)
                        {
                            chiSo.Rpc_CongDiem(1);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1.0f, banKinhTanCong);
    }
}
