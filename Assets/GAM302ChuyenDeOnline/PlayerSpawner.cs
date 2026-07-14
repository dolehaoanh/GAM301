using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-4.5f, 4.5f), 1f, Random.Range(-4.5f, 4.5f));
            NetworkObject spawnedObject = Runner.Spawn(playerPrefab, randomPosition, Quaternion.identity, player);
            
            CharacterController cc = spawnedObject.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                spawnedObject.transform.position = randomPosition;
                cc.enabled = true;
            }
        }
    }
}
