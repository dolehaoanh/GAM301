using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class QuanLyPlayFab : MonoBehaviour
{
    public static QuanLyPlayFab Instance { get; private set; }

    public string idPlayFab = "";
    public string tenNguoiDung = "";
    public int diemSoCaoNhat = 0;
    public string tenBangXepHang = "HighScores";

    public System.Action<bool, string> khiDangNhapHoanTat;
    public System.Action<Dictionary<string, string>> khiTaiDuLieuHoanTat;
    public System.Action<List<PlayerLeaderboardEntry>> khiTaiBangXepHangHoanTat;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DangNhapCustomID(string customId)
    {
        if (string.IsNullOrEmpty(customId))
        {
            customId = SystemInfo.deviceUniqueIdentifier;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnDangNhapThanhCong, OnDangNhapThatBai);
    }

    private void OnDangNhapThanhCong(LoginResult result)
    {
        idPlayFab = result.PlayFabId;
        Debug.Log($"[PlayFab] Dang nhap thanh cong: {idPlayFab}");
        khiDangNhapHoanTat?.Invoke(true, idPlayFab);
        TaiDuLieuNguoiChoi();
    }

    private void OnDangNhapThatBai(PlayFabError error)
    {
        Debug.LogError($"[PlayFab] Dang nhap that bai: {error.GenerateErrorReport()}");
        khiDangNhapHoanTat?.Invoke(false, error.ErrorMessage);
    }

    public void LuuDuLieuNguoiChoi(Dictionary<string, string> duLieu)
    {
        var request = new UpdateUserDataRequest
        {
            Data = duLieu
        };

        PlayFabClientAPI.UpdateUserData(request, 
            result => Debug.Log("[PlayFab] Luu du lieu thanh cong."),
            error => Debug.LogError($"[PlayFab] Luu du lieu that bai: {error.GenerateErrorReport()}")
        );
    }

    public void TaiDuLieuNguoiChoi()
    {
        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        dict[item.Key] = item.Value.Value;
                    }
                }
                khiTaiDuLieuHoanTat?.Invoke(dict);
            },
            error => Debug.LogError($"[PlayFab] Tai du lieu that bai: {error.GenerateErrorReport()}")
        );
    }

    public void CapNhatDiemSo(int diem)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = tenBangXepHang,
                    Value = diem
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log("[PlayFab] Cap nhat diem bang xep hang thanh cong."),
            error => Debug.LogError($"[PlayFab] Cap nhat diem that bai: {error.GenerateErrorReport()}")
        );
    }

    public void LayBangXepHang()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = tenBangXepHang,
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result =>
            {
                khiTaiBangXepHangHoanTat?.Invoke(result.Leaderboard);
            },
            error => Debug.LogError($"[PlayFab] Lay bang xep hang that bai: {error.GenerateErrorReport()}")
        );
    }
}
