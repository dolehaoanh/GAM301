using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerControllerFSM : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector2 moveInput;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 9.99f;

    private void Awake()
    {
        // Get NavMeshAgent component đã gắn vào vàoPlayer
        agent = GetComponent<NavMeshAgent>();
    }

    // Method này đc gọi tự động bởi component PlayerInput (thiết lập với "Send Messages") khi action "Move" được trigger (W, A, S, D, phím mũi tên, hoặc Left Stick của tay cầm)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        // Chuyển input 2D (X,Y) sang vector hướng 3D (X, 0, Z)
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        // Nếu có input thì di chuyển agent trên NavMesh
        if (direction.sqrMagnitude > 0.01f)
        {
            // Di chuyển agent sử dung NavMeshAgent.Move - bởi vậy giữ Player dính với (clamped to) NavMesh
            agent.Move(direction * speed * Time.deltaTime);

            // Thêm: Quay Player để hướng mặt về hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
