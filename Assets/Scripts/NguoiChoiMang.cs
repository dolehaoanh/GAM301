using UnityEngine;
using Fusion;
using Unity.Cinemachine;

public class NguoiChoiMang : NetworkBehaviour
{
    public float vanToc = 6f;
    public float lucNhay = 5f;
    public float trongLuc = -9.81f;
    public LayerMask lopPhuDich;
    public GameObject prefabDan;
    public GameObject prefabHieuUngVaCham;
    public Transform viTriBan;

    public KeyCode len = KeyCode.W;
    public KeyCode duoi = KeyCode.S;
    public KeyCode trai = KeyCode.A;
    public KeyCode phai = KeyCode.D;
    public KeyCode nhayKey = KeyCode.Space;
    public KeyCode banKey = KeyCode.Mouse0;

    [Networked] public Vector3 viTriXuatPhat { get; set; }
    [Networked] public bool daDenDich { get; set; }
    [Networked] public int idNguoiChoi { get; set; }
    [Networked] public NetworkString<_32> tagVungDich { get; set; }
    [Networked] public float mauHienTai { get; set; } = 100f;
    [Networked] public float mauToiDa { get; set; } = 100f;
    [Networked] public int diemSo { get; set; } = 0;
    [Networked] public bool dangDiChuyen { get; set; }
    [Networked] public bool dangNhay { get; set; }
    [Networked] public bool dangTanCong { get; set; }

    public QuanLyTroChoi quanLyTroChoi;
    private CharacterController characterController;
    private Vector3 vanTocY;
    private Animator animator;

    public override void Spawned()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        quanLyTroChoi = FindObjectOfType<QuanLyTroChoi>();

        if (HasInputAuthority)
        {
            GanCinemachineCamera();
        }

        if (quanLyTroChoi != null)
        {
            if (idNguoiChoi == 1) quanLyTroChoi.nguoiChoi1 = this;
            else if (idNguoiChoi == 2) quanLyTroChoi.nguoiChoi2 = this;
        }
    }

    public void GanCinemachineCamera()
    {
        CinemachineCamera cinemachineCam = FindObjectOfType<CinemachineCamera>();
        if (cinemachineCam != null)
        {
            cinemachineCam.Follow = transform;
            cinemachineCam.LookAt = transform;
        }
    }

    private bool daAnNhay;
    private bool daAnBan;

    private void Update()
    {
        if (HasInputAuthority)
        {
            if (Input.GetKeyDown(nhayKey)) daAnNhay = true;
            if (Input.GetKeyDown(banKey) || Input.GetKeyDown(KeyCode.J)) daAnBan = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority || HasStateAuthority)
        {
            XuLyDiChuyen();
            XuLyBanSungWithLagCompensation();
        }

        CapNhatAnimationState();
    }

    private void XuLyDiChuyen()
    {
        Vector3 huongDi = Vector3.zero;

        if (Input.GetKey(len)) huongDi.z += 1f;
        if (Input.GetKey(duoi)) huongDi.z -= 1f;
        if (Input.GetKey(trai)) huongDi.x -= 1f;
        if (Input.GetKey(phai)) huongDi.x += 1f;

        huongDi = huongDi.normalized;
        dangDiChuyen = huongDi.sqrMagnitude > 0.01f;

        if (characterController != null)
        {
            if (characterController.isGrounded)
            {
                if (vanTocY.y < 0) vanTocY.y = -2f;
                dangNhay = false;

                if (daAnNhay)
                {
                    vanTocY.y = Mathf.Sqrt(lucNhay * -2f * trongLuc);
                    dangNhay = true;
                    daAnNhay = false;
                }
            }

            Vector3 diChuyenThucTe = huongDi * vanToc;
            characterController.Move(diChuyenThucTe * Runner.DeltaTime);

            vanTocY.y += trongLuc * Runner.DeltaTime;
            characterController.Move(vanTocY * Runner.DeltaTime);
        }
        else
        {
            transform.Translate(huongDi * vanToc * Runner.DeltaTime, Space.World);
        }
    }

    [Networked] public float thoiGianChoTanCong { get; set; }
    private float thoiGianHoiChieu = 0.4f; // Thoi gian cua 1 don danh (cooldown)

    private void XuLyBanSungWithLagCompensation()
    {
        if (thoiGianChoTanCong > 0)
        {
            thoiGianChoTanCong -= Runner.DeltaTime;
            // Cho dangTanCong ve false truoc khi het cooldown mot chut de Animator kip reset trang thai
            if (thoiGianChoTanCong <= 0.1f) 
            {
                dangTanCong = false;
            }
        }

        if (daAnBan)
        {
            daAnBan = false;
            
            // Chi ban khi da het thoi gian cooldown
            if (thoiGianChoTanCong <= 0)
            {
                dangTanCong = true;
                thoiGianChoTanCong = thoiGianHoiChieu; // Set cooldown 0.4s

                Vector3 gocBan = transform.forward;
                Vector3 viTri = (viTriBan != null) ? viTriBan.position : transform.position + transform.forward;

                if (QuanLyObjectPool.Instance != null && prefabDan != null)
                {
                    QuanLyObjectPool.Instance.LayDoiTuong(prefabDan, viTri, Quaternion.LookRotation(gocBan));
                }

                if (Runner.LagCompensation.Raycast(viTri, gocBan, 100f, Object.InputAuthority, out LagCompensatedHit hit, lopPhuDich))
                {
                    if (hit.Hitbox != null)
                    {
                        NguoiChoiMang doiThu = hit.Hitbox.Root.GetComponent<NguoiChoiMang>();
                        if (doiThu != null && doiThu != this)
                        {
                            doiThu.NhanShatThu(20f, Object.InputAuthority);
                        }
                    }

                    if (hit.Point != Vector3.zero && QuanLyObjectPool.Instance != null && prefabHieuUngVaCham != null)
                    {
                        QuanLyObjectPool.Instance.LayDoiTuong(prefabHieuUngVaCham, hit.Point, Quaternion.LookRotation(hit.Normal));
                    }
                }
            }
        }
    }

    public void NhanShatThu(float dam, PlayerRef nguoiBan)
    {
        if (HasStateAuthority)
        {
            mauHienTai = Mathf.Max(0f, mauHienTai - dam);
            if (mauHienTai <= 0f)
            {
                DatLaiViTri();
                mauHienTai = mauToiDa;
            }
        }
        else
        {
            RPC_NhanShatThu(dam);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_NhanShatThu(float dam)
    {
        mauHienTai = Mathf.Max(0f, mauHienTai - dam);
        if (mauHienTai <= 0f)
        {
            DatLaiViTri();
            mauHienTai = mauToiDa;
        }
    }

    private void CapNhatAnimationState()
    {
        if (animator != null)
        {
            animator.SetBool("IsRunning", dangDiChuyen);
            animator.SetBool("IsJumping", dangNhay);
            animator.SetBool("IsAttack", dangTanCong);
        }
    }

    public void DatLaiViTri()
    {
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = viTriXuatPhat;
            characterController.enabled = true;
        }
        else
        {
            transform.position = viTriXuatPhat;
        }
        daDenDich = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BiXanh"))
        {
            if (HasInputAuthority || HasStateAuthority)
            {
                DatLaiViTri();
                if (quanLyTroChoi != null)
                {
                    quanLyTroChoi.TangSoLanThua();
                }
            }
        }
        else if (other.CompareTag(tagVungDich.ToString()))
        {
            daDenDich = true;
            if (quanLyTroChoi != null)
            {
                quanLyTroChoi.KiemTraChienThang();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagVungDich.ToString()))
        {
            daDenDich = false;
        }
    }
}
