using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Threading.Tasks;

public class QuanLyKetNoiFusion : MonoBehaviour
{
    public static QuanLyKetNoiFusion Instance { get; private set; }

    public NetworkRunner runnerPrefab;
    public NetworkObject prefabNguoiChoi;
    public string tenPhong = "HardestGame3D_Room";
    public int tickRateMacDinh = 60;

    public NetworkRunner runnerHienTai;

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

    public async Task<bool> TaoHoacThamGiaPhong(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            tenPhong = roomName;
        }

        if (runnerHienTai == null)
        {
            runnerHienTai = gameObject.GetComponent<NetworkRunner>();
            if (runnerHienTai == null)
            {
                runnerHienTai = gameObject.AddComponent<NetworkRunner>();
            }
            runnerHienTai.ProvideInput = true;
        }

        var sceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        QuanLySinhNguoiChoi spawner = gameObject.GetComponent<QuanLySinhNguoiChoi>();
        if (spawner == null)
        {
            spawner = gameObject.AddComponent<QuanLySinhNguoiChoi>();
        }

        if (prefabNguoiChoi == null)
        {
            prefabNguoiChoi = Resources.Load<NetworkObject>("NguoiChoiPrefab");
        }
        spawner.prefabNguoiChoi = prefabNguoiChoi;

        runnerHienTai.AddCallbacks(spawner);

        var sceneHienTai = SceneManager.GetActiveScene();

        var result = await runnerHienTai.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = tenPhong,
            SceneManager = sceneManager,
            Scene = SceneRef.FromIndex(sceneHienTai.buildIndex)
        });

        if (result.Ok && !spawner.daSinhNguoiChoi)
        {
            spawner.SinhTatCaNguoiChoi(runnerHienTai, runnerHienTai.LocalPlayer);
        }

        ThietLapTickRate(tickRateMacDinh);
        return result.Ok;
    }

    public void ThietLapTickRate(int tickRate)
    {
        if (runnerHienTai != null && runnerHienTai.IsRunning)
        {
            Debug.Log($"[QuanLyKetNoiFusion] Thiet lap Tick Rate thong qua NetworkProjectConfig trong Editor (gia tri mong muon: {tickRate})");
        }
    }

    public async void RoiPhong()
    {
        if (runnerHienTai != null && runnerHienTai.IsRunning)
        {
            await runnerHienTai.Shutdown();
            Debug.Log("[QuanLyKetNoiFusion] Da roi phong.");
        }
    }
}
