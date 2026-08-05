using Fusion;
using UnityEngine;

public class HoatAnhNhanVat : NetworkBehaviour
{
    private Animator hoatAnh;
    private ChiSoNhanVat chiSo;

    private void Awake()
    {
        // Tự động tìm Animator ở nhân vật con (không cần kéo thả)
        hoatAnh = GetComponentInChildren<Animator>();
        chiSo = GetComponent<ChiSoNhanVat>();
    }

    private void Update()
    {
        if (hoatAnh != null && chiSo != null)
        {
            hoatAnh.SetBool("IsRunning", chiSo.DangChay);
            hoatAnh.SetBool("IsAttacking", chiSo.DangTanCong);
            hoatAnh.SetBool("IsJumping", chiSo.DangNhay);
            hoatAnh.SetBool("IsHurt", chiSo.DangBiThuong);
            hoatAnh.SetBool("IsDead", chiSo.HP <= 0);
        }
    }
}
