using UnityEngine;
using TMPro;
using Fusion;

public class QuanLyTroChoi : NetworkBehaviour
{
    public static QuanLyTroChoi Instance { get; private set; }

    public NguoiChoiMang nguoiChoi1;
    public NguoiChoiMang nguoiChoi2;
    public TMP_Text textSoLan;
    public TMP_Text textThoiGian;
    public TMP_Text textMau;
    public TMP_Text textDiemSo;
    public GameObject panelWin;
    public AudioSource audioSource;
    
    [Networked] public int soLanThua { get; set; }
    [Networked] public bool dangPhatNhac { get; set; }
    [Networked] public bool troChoiDaThang { get; set; }
    [Networked] public float thoiGianTranDau { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void Spawned()
    {
        CapNhatGiaoDien();
        if (panelWin != null)
        {
            panelWin.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (!troChoiDaThang)
            {
                thoiGianTranDau += Runner.DeltaTime;
            }
        }
    }

    public override void Render()
    {
        CapNhatGiaoDien();
        if (panelWin != null)
        {
            panelWin.SetActive(troChoiDaThang);
        }
    }

    public void TangSoLanThua()
    {
        if (HasStateAuthority)
        {
            soLanThua++;
            DatLaiTroChoi();
        }
        else
        {
            RPC_TangSoLanThua();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TangSoLanThua()
    {
        soLanThua++;
        DatLaiTroChoi();
    }

    public void DatLaiTroChoi()
    {
        if (nguoiChoi1 != null) nguoiChoi1.DatLaiViTri();
        if (nguoiChoi2 != null) nguoiChoi2.DatLaiViTri();
    }

    public void KiemTraChienThang()
    {
        if (nguoiChoi1 != null && nguoiChoi2 != null)
        {
            if (nguoiChoi1.daDenDich && nguoiChoi2.daDenDich)
            {
                if (HasStateAuthority)
                {
                    troChoiDaThang = true;
                    if (QuanLyPlayFab.Instance != null)
                    {
                        int diemThuong = Mathf.Max(100, 1000 - Mathf.FloorToInt(thoiGianTranDau) * 10 - soLanThua * 50);
                        QuanLyPlayFab.Instance.CapNhatDiemSo(diemThuong);
                    }
                }
                else
                {
                    RPC_BaoChienThang();
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_BaoChienThang()
    {
        troChoiDaThang = true;
    }

    public void CapNhatGiaoDien()
    {
        if (textSoLan != null)
        {
            textSoLan.text = "SỐ LẦN: " + soLanThua;
        }

        if (textThoiGian != null)
        {
            int phut = Mathf.FloorToInt(thoiGianTranDau / 60f);
            int giay = Mathf.FloorToInt(thoiGianTranDau % 60f);
            textThoiGian.text = string.Format("THỜI GIAN: {0:00}:{1:00}", phut, giay);
        }

        NguoiChoiMang ncLocal = FindLocalPlayer();
        if (ncLocal != null)
        {
            if (textMau != null)
            {
                textMau.text = "MÁU: " + Mathf.CeilToInt(ncLocal.mauHienTai) + " / " + Mathf.CeilToInt(ncLocal.mauToiDa);
            }
            if (textDiemSo != null)
            {
                textDiemSo.text = "ĐIỂM: " + ncLocal.diemSo;
            }
        }
    }

    private NguoiChoiMang FindLocalPlayer()
    {
        NguoiChoiMang[] tatCaNguoiChoi = FindObjectsOfType<NguoiChoiMang>();
        foreach (var nc in tatCaNguoiChoi)
        {
            if (nc.HasInputAuthority) return nc;
        }
        return (tatCaNguoiChoi.Length > 0) ? tatCaNguoiChoi[0] : null;
    }

    public void ChoiLai()
    {
        if (HasStateAuthority)
        {
            soLanThua = 0;
            thoiGianTranDau = 0f;
            troChoiDaThang = false;
            DatLaiTroChoi();
        }
        else
        {
            RPC_ChoiLai();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChoiLai()
    {
        soLanThua = 0;
        thoiGianTranDau = 0f;
        troChoiDaThang = false;
        DatLaiTroChoi();
    }

    public void BatTatNhac()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying) audioSource.Pause();
            else audioSource.Play();
        }
    }

    public void TamDungHoacMenu()
    {
        if (Time.timeScale == 1f) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }
}
