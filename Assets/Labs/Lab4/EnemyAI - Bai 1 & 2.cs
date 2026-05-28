using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform destination;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Nếu nó đã được đặt thủ công trong scene với điểm đích, di chuyển ngay bây giờ
        if (destination != null)
        {
            agent.SetDestination(destination.position);
        }
    }

    // 💡 Phương thức mới: Được gọi bởi Spawner để khởi tạo mục tiêu một cách động
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