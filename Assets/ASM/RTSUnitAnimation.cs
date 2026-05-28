using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class RTSUnitAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Animation Smooth Settings")]
    [Tooltip("Độ mượt khi chuyển đổi thông số tốc độ trong Animator")]
    public float speedDampTime = 0.1f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Lấy tốc độ di chuyển thực tế hiện tại của NavMeshAgent
        // (agent.velocity.magnitude trả về tốc độ thực tế tính theo m/s)
        float currentSpeed = agent.velocity.magnitude;

        // 2. Truyền tốc độ này vào biến float "Speed" trong Animator Controller.
        // Sử dụng Animator.SetFloat kèm theo speedDampTime để làm mịn chuyển động chuyển giao
        // giữa Idle (Đứng yên) và Walk (Đi bộ) hoặc Run (Chạy) mà không bị giật cục.
        animator.SetFloat("Speed", currentSpeed, speedDampTime, Time.deltaTime);
    }
}
