using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class GiaoDienNguoiDung : MonoBehaviour
{
    public TMP_InputField inputTenPhong;
    public TMP_InputField inputTenNguoiChoi;
    public TMP_InputField inputAppIdChat;
    public Button btnTaoHoacThamGiaPhong;

    public GameObject panelChat;
    public TMP_Text textNoiDungChat;
    public TMP_InputField inputTinNhan;
    public TMP_InputField inputNguoiNhanRieng;
    public Toggle toggleChatRieng;
    public Button btnGuiChat;

    public GameObject panelBangXepHang;
    public TMP_Text textDanhSachBangXepHang;
    public Button btnXemBangXepHang;
    public Button btnDongBangXepHang;

    private void Start()
    {
        if (btnTaoHoacThamGiaPhong != null)
            btnTaoHoacThamGiaPhong.onClick.AddListener(OnTaoHoacThamGiaPhongClick);

        if (btnGuiChat != null)
            btnGuiChat.onClick.AddListener(OnGuiChatClick);

        if (btnXemBangXepHang != null)
            btnXemBangXepHang.onClick.AddListener(OnXemBangXepHangClick);

        if (btnDongBangXepHang != null)
            btnDongBangXepHang.onClick.AddListener(OnDongBangXepHangClick);

        if (QuanLyChatPhoton.Instance != null)
            QuanLyChatPhoton.Instance.khiNhanTinNhan += CapNhatHienThiChat;

        if (QuanLyPlayFab.Instance != null)
            QuanLyPlayFab.Instance.khiTaiBangXepHangHoanTat += HienThiBangXepHang;
    }

    private async void OnTaoHoacThamGiaPhongClick()
    {
        string tenPhong = (inputTenPhong != null && !string.IsNullOrEmpty(inputTenPhong.text)) ? inputTenPhong.text : "HardestGame3D_Room";
        string tenUser = (inputTenNguoiChoi != null && !string.IsNullOrEmpty(inputTenNguoiChoi.text)) ? inputTenNguoiChoi.text : "Player_" + Random.Range(100, 999);
        string appId = (inputAppIdChat != null) ? inputAppIdChat.text : "";

        if (QuanLyPlayFab.Instance != null)
        {
            QuanLyPlayFab.Instance.DangNhapCustomID(tenUser);
        }

        if (QuanLyChatPhoton.Instance != null)
        {
            QuanLyChatPhoton.Instance.KetNoiChat(tenUser, appId);
        }

        if (QuanLyKetNoiFusion.Instance != null)
        {
            await QuanLyKetNoiFusion.Instance.TaoHoacThamGiaPhong(tenPhong);
        }
    }

    private void OnGuiChatClick()
    {
        if (inputTinNhan == null || string.IsNullOrEmpty(inputTinNhan.text)) return;

        string tinNhan = inputTinNhan.text;
        bool chatRieng = (toggleChatRieng != null && toggleChatRieng.isOn);
        string nguoiNhan = (inputNguoiNhanRieng != null) ? inputNguoiNhanRieng.text : "";

        if (QuanLyChatPhoton.Instance != null)
        {
            if (chatRieng && !string.IsNullOrEmpty(nguoiNhan))
            {
                QuanLyChatPhoton.Instance.GuiTinNhanRiengTu(nguoiNhan, tinNhan);
            }
            else
            {
                QuanLyChatPhoton.Instance.GuiTinNhanCongKhai(tinNhan);
            }
        }
        inputTinNhan.text = "";
    }

    private void CapNhatHienThiChat(string tinNhanMoi)
    {
        if (textNoiDungChat != null && QuanLyChatPhoton.Instance != null)
        {
            textNoiDungChat.text = string.Join("\n", QuanLyChatPhoton.Instance.danhSachTinNhan.ToArray());
        }
    }

    private void OnXemBangXepHangClick()
    {
        if (panelBangXepHang != null) panelBangXepHang.SetActive(true);
        if (QuanLyPlayFab.Instance != null)
        {
            QuanLyPlayFab.Instance.LayBangXepHang();
        }
    }

    private void OnDongBangXepHangClick()
    {
        if (panelBangXepHang != null) panelBangXepHang.SetActive(false);
    }

    private void HienThiBangXepHang(List<PlayerLeaderboardEntry> danhSach)
    {
        if (textDanhSachBangXepHang == null || danhSach == null) return;

        string res = "--- BẢNG XẾP HẠNG ---\n";
        foreach (var item in danhSach)
        {
            res += string.Format("{0}. {1} - {2} ĐIỂM\n", item.Position + 1, item.DisplayName ?? item.PlayFabId, item.StatValue);
        }
        textDanhSachBangXepHang.text = res;
    }
}
