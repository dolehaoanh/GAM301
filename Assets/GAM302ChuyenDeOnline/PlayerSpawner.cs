using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            float viTriX = Random.Range(-4.5f, 4.5f);
            float viTriZ = Random.Range(-4.5f, 4.5f);
            float viTriY = 10f;

            if (Physics.Raycast(new Vector3(viTriX, 100f, viTriZ), Vector3.down, out RaycastHit hit, 200f))
            {
                viTriY = hit.point.y + 1f;
            }

            Vector3 viTriNgauNhien = new Vector3(viTriX, viTriY, viTriZ);
            NetworkObject spawnedObject = Runner.Spawn(playerPrefab, viTriNgauNhien, Quaternion.identity, player);
            
            CharacterController dieuKhien = spawnedObject.GetComponent<CharacterController>();
            if (dieuKhien != null)
            {
                dieuKhien.enabled = false;
                spawnedObject.transform.position = viTriNgauNhien;
                dieuKhien.enabled = true;
            }
        }
    }
}
