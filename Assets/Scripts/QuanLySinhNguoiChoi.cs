using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;

public class QuanLySinhNguoiChoi : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkObject prefabNguoiChoi;
    public Vector3 viTriNguoiChoi1 = new Vector3(-7.5f, 0.45f, 0.5f);
    public Vector3 viTriNguoiChoi2 = new Vector3(7.5f, 0.45f, -0.5f);

    public bool daSinhNguoiChoi = false;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[QuanLySinhNguoiChoi] OnPlayerJoined: player={player}, LocalPlayer={runner.LocalPlayer}");
        if (!daSinhNguoiChoi)
        {
            SinhTatCaNguoiChoi(runner, player);
        }
    }

    public void SinhTatCaNguoiChoi(NetworkRunner runner, PlayerRef player)
    {
        if (daSinhNguoiChoi) return;
        if (runner == null || !runner.IsRunning) return;

        if (prefabNguoiChoi == null)
        {
            prefabNguoiChoi = Resources.Load<NetworkObject>("NguoiChoiPrefab");
        }
        if (prefabNguoiChoi == null)
        {
            Debug.LogError("[QuanLySinhNguoiChoi] Khong tim thay prefabNguoiChoi!");
            return;
        }

        daSinhNguoiChoi = true;

        PlayerRef playerToAssign = (player != PlayerRef.None) ? player : runner.LocalPlayer;

        Debug.Log("[QuanLySinhNguoiChoi] Sinh Player 1...");
        NetworkObject no1 = runner.Spawn(prefabNguoiChoi, viTriNguoiChoi1, Quaternion.identity, playerToAssign,
            (runner, obj) =>
            {
                NguoiChoiMang nc = obj.GetComponent<NguoiChoiMang>();
                if (nc != null)
                {
                    nc.viTriXuatPhat = viTriNguoiChoi1;
                    nc.idNguoiChoi = 1;
                    nc.tagVungDich = "VungSafePhai";
                }
            });

        Debug.Log("[QuanLySinhNguoiChoi] Sinh Player 2...");
        NetworkObject no2 = runner.Spawn(prefabNguoiChoi, viTriNguoiChoi2, Quaternion.identity, playerToAssign,
            (runner, obj) =>
            {
                NguoiChoiMang nc = obj.GetComponent<NguoiChoiMang>();
                if (nc != null)
                {
                    nc.viTriXuatPhat = viTriNguoiChoi2;
                    nc.idNguoiChoi = 2;
                    nc.tagVungDich = "VungSafeTrai";
                }
            });
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!daSinhNguoiChoi && runner != null && runner.IsRunning)
        {
            SinhTatCaNguoiChoi(runner, runner.LocalPlayer);
        }
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
