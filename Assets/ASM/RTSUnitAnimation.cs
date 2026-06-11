using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RTSUnitAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Animation Smooth Settings")]
    [Tooltip("Độ mượt khi chuyển đổi thông số tốc độ trong Animator")]
    public float speedDampTime = 0.15f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Tìm kiếm thông minh Animator thực tế (chọn cái có gắn Controller trong các con)
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
            {
                animator = anim;
                break;
            }
        }

        // Fallback nếu không tìm thấy cái nào có controller thì lấy cái đầu tiên trong con
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning($"[RTS Unit Animation] Không tìm thấy Animator nào trên {gameObject.name} hoặc các con của nó!");
        }
    }

    private void Update()
    {
        // Safety checks: do nothing if animator is missing, agent is disabled, or unit is dead
        if (animator == null || agent == null || !agent.enabled) return;

        RTSUnit unit = GetComponent<RTSUnit>();
        if (unit != null && unit.currentState == RTSUnit.RTSUnitState.Dead) return;

        // 1. Lấy tốc độ di chuyển thực tế hiện tại của NavMeshAgent
        float currentSpeed = agent.velocity.magnitude;

        // 2. Truyền tốc độ này vào biến float "Speed" trong Animator Controller.
        animator.SetFloat("Speed", currentSpeed, speedDampTime, Time.deltaTime);
    }
}
