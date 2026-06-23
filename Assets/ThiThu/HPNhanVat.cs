using System.Collections;
using UnityEngine;

public class HPNhanVat : MonoBehaviour
{
    public float mauToiDa = 100f;
    public float mauHienTai = 100f;
    public float thoiGianChoHoiPhuc = 0.5f;
    public float luongHoiPhuc = 1f;
    public float chuKyHoiPhuc = 0.1f;

    private float thoiGianTuLanBiThuong;
    private float boDemHoiPhuc;

    private void Start()
    {
        mauHienTai = mauToiDa;
    }

    private void Update()
    {
        thoiGianTuLanBiThuong += Time.deltaTime;

        if (thoiGianTuLanBiThuong >= thoiGianChoHoiPhuc && mauHienTai < mauToiDa)
        {
            boDemHoiPhuc += Time.deltaTime;
            if (boDemHoiPhuc >= chuKyHoiPhuc)
            {
                boDemHoiPhuc = 0f;
                mauHienTai = Mathf.Min(mauHienTai + luongHoiPhuc, mauToiDa);
            }
        }
    }

    public void NhanSatThuong(float satThuong)
    {
        mauHienTai = Mathf.Max(mauHienTai - satThuong, 0f);
        thoiGianTuLanBiThuong = 0f;
        boDemHoiPhuc = 0f;
    }
}
