using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject monsterPrefab; // Drag your MonsterPrefab here
    public Transform endPortalTarget; // Drag your EndPortal here

    [Header("Spawn Settings")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    void Start()
    {
        if (monsterPrefab == null || endPortalTarget == null)
        {
            Debug.LogError("Spawner is missing setup fields!");
            return;
        }

        // Start the infinite spawning loop
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 1. Spawn a new monster at the spawner's position
            GameObject newMonster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);

            // 2. ⚠️ UPDATED: Look for MonsterFSM instead of EnemyAI!
            MonsterFSM aiScript = newMonster.GetComponent<MonsterFSM>();
            if (aiScript != null)
            {
                aiScript.InitializeDestination(endPortalTarget);
            }

            // 3. Wait for a random time before spawning the next monster
            float randomWait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomWait);
        }
    }
}