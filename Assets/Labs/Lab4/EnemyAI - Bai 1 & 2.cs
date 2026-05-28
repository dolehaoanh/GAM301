using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform destination;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // If it was already manually placed in the scene with a destination, move now
        if (destination != null)
        {
            agent.SetDestination(destination.position);
        }
    }

    // 💡 New Method: Called by the Spawner to initialize the target dynamically
    public void InitializeDestination(Transform target)
    {
        destination = target;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null && destination != null)
        {
            agent.SetDestination(destination.position);
        }
    }
}