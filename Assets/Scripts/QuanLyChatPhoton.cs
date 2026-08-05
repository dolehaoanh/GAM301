using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;

public class QuanLyChatPhoton : MonoBehaviour, IChatClientListener
{
    public static QuanLyChatPhoton Instance { get; private set; }

    public string idUngDungChat = "";
    public string phienBanApp = "1.0";
    public string tenNguoiDung = "NguoiChoi";
    public string kenhChung = "Global";

    public ChatClient chatClient;
    public List<string> danhSachTinNhan = new List<string>();
    public System.Action<string> khiNhanTinNhan;

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

    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void KetNoiChat(string tenUser, string appId)
    {
        if (!string.IsNullOrEmpty(tenUser)) tenNguoiDung = tenUser;
        if (!string.IsNullOrEmpty(appId)) idUngDungChat = appId;

        chatClient = new ChatClient(this);
        chatClient.AuthValues = new AuthenticationValues(tenNguoiDung);
        
        ChatAppSettings appSettings = new ChatAppSettings();
        appSettings.AppIdChat = idUngDungChat;
        appSettings.AppVersion = phienBanApp;

        chatClient.ConnectUsingSettings(appSettings);
    }

    public void GuiTinNhanCongKhai(string nộiDung)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage(kenhChung, nộiDung);
        }
    }

    public void GuiTinNhanRiengTu(string nguoiNhan, string nộiDung)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.SendPrivateMessage(nguoiNhan, nộiDung);
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"[PhotonChat Debug] {message}");
    }

    public void OnDisconnected()
    {
        Debug.Log("[PhotonChat] Ngat ket noi chat.");
    }

    public void OnConnected()
    {
        Debug.Log("[PhotonChat] Da ket noi thanh cong. Dang tham gia kenh chung...");
        if (chatClient != null)
        {
            chatClient.Subscribe(new string[] { kenhChung });
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"[PhotonChat] Trang thai chat: {state}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            string dongTinNhan = $"[{channelName}] {senders[i]}: {messages[i]}";
            danhSachTinNhan.Add(dongTinNhan);
            khiNhanTinNhan?.Invoke(dongTinNhan);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string dongTinNhan = $"[Rieng Tu] {sender}: {message}";
        danhSachTinNhan.Add(dongTinNhan);
        khiNhanTinNhan?.Invoke(dongTinNhan);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"[PhotonChat] Da tham gia kenh {channels[0]}");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("[PhotonChat] Da roi kenh.");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    public void OnUserSubscribed(string channel, string user) { }

    public void OnUserUnsubscribed(string channel, string user) { }
}
