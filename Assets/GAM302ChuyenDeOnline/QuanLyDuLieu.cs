using System.Collections.Generic;
using Fusion;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class QuanLyDuLieu : MonoBehaviour
{
    public TextMeshProUGUI vungHienThiBangXepHang;
    public GameObject panelBangXepHang;

    private bool daDangNhap = false;

    private void Start()
    {
        DangNhapPlayFab();
    }

    private void DangNhapPlayFab()
    {
        string maDinhDanh = PlayerPrefs.GetString("PlayFab_CustomID_Client", "");
        if (string.IsNullOrEmpty(maDinhDanh))
        {
            maDinhDanh = SystemInfo.deviceUniqueIdentifier + "_" + Random.Range(1000, 9999);
            PlayerPrefs.SetString("PlayFab_CustomID_Client", maDinhDanh);
        }

        var yeuCau = new LoginWithCustomIDRequest 
        { 
            CustomId = maDinhDanh, 
            CreateAccount = true 
        };
        PlayFabClientAPI.LoginWithCustomID(yeuCau, KetQuaDangNhap, LoiDangNhap);
    }

    private void KetQuaDangNhap(LoginResult ketQua)
    {
        daDangNhap = true;
        Debug.Log("Dang nhap PlayFab thanh cong!");
    }

    private void LoiDangNhap(PlayFabError loi)
    {
        Debug.LogError("Loi PlayFab: " + loi.GenerateErrorReport());
        if (vungHienThiBangXepHang != null)
        {
            if (loi.ErrorMessage != null && loi.ErrorMessage.Contains("Player creations have been disabled"))
            {
                vungHienThiBangXepHang.text = "Loi PlayFab: Can bat 'Allow client to create players' trong PlayFab Settings > API Features.";
            }
            else
            {
                vungHienThiBangXepHang.text = "Loi PlayFab: " + loi.ErrorMessage;
            }
        }
    }

    public void LuuDiemLocalCurrentPlayer()
    {
        ChiSoNhanVat nhanVatLocal = null;
        ChiSoNhanVat[] cacNhanVat = UnityEngine.Object.FindObjectsByType<ChiSoNhanVat>(FindObjectsInactive.Exclude);
        foreach (var nv in cacNhanVat)
        {
            if (nv.Object != null && nv.Object.HasInputAuthority)
            {
                nhanVatLocal = nv;
                break;
            }
        }
        if (nhanVatLocal == null)
        {
            foreach (var nv in cacNhanVat)
            {
                if (nv.Object != null && nv.Object.HasStateAuthority)
                {
                    nhanVatLocal = nv;
                    break;
                }
            }
        }

        if (nhanVatLocal != null)
        {
            Debug.Log("LuuDiemLocalCurrentPlayer: Nhan vat ID = " + nhanVatLocal.Object.Id + ", DiemSo = " + nhanVatLocal.DiemSo);
            LuuDiemSo(nhanVatLocal.DiemSo);
        }
        else
        {
            Debug.LogWarning("LuuDiemLocalCurrentPlayer: Khong tim thay nhan vat local!");
        }
    }

    public void LuuDiemSo(int diem)
    {
        if (!daDangNhap)
        {
            DangNhapPlayFab();
            return;
        }

        var yeuCau = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "HighScore", Value = diem }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(yeuCau, 
            ketQua => 
            {
                Debug.Log("Luu diem " + diem + " thanh cong.");
                LayBangXepHang();
            },
            LoiDangNhap);
    }

    public void HienThiHoacAnBangXepHang()
    {
        if (panelBangXepHang != null)
        {
            bool hienTaiActive = !panelBangXepHang.activeSelf;
            panelBangXepHang.SetActive(hienTaiActive);
            if (hienTaiActive)
            {
                LayBangXepHang();
            }
        }
        else
        {
            LayBangXepHang();
        }
    }

    public void LayBangXepHang()
    {
        if (!daDangNhap)
        {
            if (vungHienThiBangXepHang != null)
            {
                vungHienThiBangXepHang.text = "Dang dang nhap PlayFab...";
            }
            DangNhapPlayFab();
            return;
        }

        var yeuCau = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(yeuCau, 
            ketQua => 
            {
                if (vungHienThiBangXepHang != null)
                {
                    string noiDung = "=== BANG XEP HANG ===";
                    if (ketQua.Leaderboard == null || ketQua.Leaderboard.Count == 0)
                    {
                        noiDung += "\n(Chua co du lieu BXH - Vui long bam Luu diem)";
                    }
                    else
                    {
                        foreach (var hang in ketQua.Leaderboard)
                        {
                            string ten = string.IsNullOrEmpty(hang.DisplayName) ? hang.PlayFabId.Substring(0, 6) : hang.DisplayName;
                            noiDung += $"\n#{hang.Position + 1}: {ten} - {hang.StatValue} diem";
                        }
                    }
                    vungHienThiBangXepHang.text = noiDung;
                }
            }, 
            LoiDangNhap);
    }
}
