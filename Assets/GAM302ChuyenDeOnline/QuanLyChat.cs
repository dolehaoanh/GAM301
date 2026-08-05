using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Chat;
using ExitGames.Client.Photon;

public class QuanLyChat : MonoBehaviour, IChatClientListener
{
    public string appIdChat = "0151b8ff-1cf1-45de-8025-916055597d8d";
    public TMP_InputField oNhapTinNhan;
    public TMP_InputField oNhapNguoiNhan;
    public TextMeshProUGUI vungHienThiTinNhan;
    public TextMeshProUGUI textTenLocal;

    private ChatClient clientChat;
    public string TenNguoiChoiLocal { get; private set; }

    private void Start()
    {
        TenNguoiChoiLocal = "NguoiChoi_" + Random.Range(100, 999);
        Debug.Log($"Khoi tao Photon Chat voi ID: {TenNguoiChoiLocal}");

        if (textTenLocal != null)
        {
            textTenLocal.text = $"Ten ban: {TenNguoiChoiLocal}";
        }

        clientChat = new ChatClient(this);
        clientChat.Connect(appIdChat, "1.0", new AuthenticationValues(TenNguoiChoiLocal));
        
        if (oNhapTinNhan != null)
        {
            oNhapTinNhan.onSubmit.AddListener(GuiTinNhan);
        }
    }

    private void Update()
    {
        if (clientChat != null)
        {
            clientChat.Service();
        }

        bool dangNhapText = (oNhapTinNhan != null && oNhapTinNhan.isFocused) || 
                            (oNhapNguoiNhan != null && oNhapNguoiNhan.isFocused);
        if (dangNhapText)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GuiTinNhanTuNut()
    {
        if (oNhapTinNhan != null)
        {
            GuiTinNhan(oNhapTinNhan.text);
        }
    }

    public void GuiTinNhan(string noiDung)
    {
        if (string.IsNullOrEmpty(noiDung)) return;

        string nguoiNhan = oNhapNguoiNhan != null ? oNhapNguoiNhan.text.Trim() : "";

        if (!string.IsNullOrEmpty(nguoiNhan))
        {
            clientChat.SendPrivateMessage(nguoiNhan, noiDung);
            if (vungHienThiTinNhan != null)
            {
                vungHienThiTinNhan.text += $"\n[Toi -> {nguoiNhan}]: {noiDung}";
            }
        }
        else
        {
            clientChat.PublishMessage("global", noiDung);
        }

        if (oNhapTinNhan != null)
        {
            oNhapTinNhan.text = "";
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (vungHienThiTinNhan == null) return;
        for (int i = 0; i < senders.Length; i++)
        {
            vungHienThiTinNhan.text += $"\n[{senders[i]}]: {messages[i]}";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (vungHienThiTinNhan == null) return;
        vungHienThiTinNhan.text += $"\n[Rieng Tu - {sender}]: {message}";
    }

    public void DebugReturn(DebugLevel level, string message) 
    {
        Debug.Log($"Photon Chat Debug ({level}): {message}");
    }
    public void OnDisconnected() 
    {
        Debug.LogWarning("Photon Chat bi ngat ket noi!");
    }
    public void OnConnected() 
    { 
        Debug.Log($"Ket noi Photon Chat thanh cong! ID: {TenNguoiChoiLocal}");
        if (textTenLocal != null)
        {
            textTenLocal.text = $"Ten ban: {TenNguoiChoiLocal} (Connected)";
        }
        clientChat.Subscribe(new string[] { "global" }); 
    }
    public void OnChatStateChange(ChatState state) {}
    public void OnSubscribed(string[] channels, bool[] results) {}
    public void OnUnsubscribed(string[] channels) {}
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {}
    public void OnUserSubscribed(string channel, string user) {}
    public void OnUserUnsubscribed(string channel, string user) {}
}
