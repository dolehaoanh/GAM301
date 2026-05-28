using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    // 1. Định nghĩa các Trạng thái của FSM
    public enum FSMState
    {
        NormalWalk,
        ActionTriggered,
        SpeedBoost,
        Jump
    }

    [Header("Máy Trạng thái (FSM)")]
    public FSMState currentState = FSMState.NormalWalk;

    [Header("Theo dõi Hành trình")]
    public Transform destination;
    private Vector3 startPosition;
    private float totalDistance;
    private bool hasTriggeredAction = false;

    private NavMeshAgent agent;
    private float normalSpeed;
    private float normalAcceleration; // Lưu trữ gia tốc bình thường

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = agent.speed;
        normalAcceleration = agent.acceleration;

        agent.autoTraverseOffMeshLink = false; // Tắt tự động di chuyển kiểu lướt qua OffMeshLink

        // Ghi nhớ vị trí sinh ra
        startPosition = transform.position;

        if (destination != null)
        {
            totalDistance = Vector3.Distance(startPosition, destination.position);
            agent.SetDestination(destination.position);
        }
    }

    // Được gọi bởi Spawner để đặt điểm đến động
    public void InitializeDestination(Transform target)
    {
        destination = target;
        agent = GetComponent<NavMeshAgent>();

        agent.autoTraverseOffMeshLink = false; // Tắt tự động di chuyển kiểu lướt qua OffMeshLink

        normalSpeed = agent.speed;
        normalAcceleration = agent.acceleration;

        startPosition = transform.position;

        if (agent != null && destination != null)
        {
            totalDistance = Vector3.Distance(startPosition, destination.position);
            agent.SetDestination(destination.position);
        }
    }

    void Update()
    {
        if (destination == null) return;

        // MỚI: Nếu tác nhân chạm vào một NavMesh Link, lập tức nhảy qua nó!
        if (agent != null && agent.isOnOffMeshLink && currentState != FSMState.Jump)
        {
            StartCoroutine(LinkJumpRoutine());
        }

        // Theo dõi trạng thái liên tục
        switch (currentState)
        {
            case FSMState.NormalWalk:
                MonitorDistance();
                break;

            case FSMState.ActionTriggered:
                // Quyết định hành động cần thực hiện (được xử lý trong Coroutine)
                break;

            case FSMState.SpeedBoost:
                // Tốc độ nhân đôi đang kích hoạt (được xử lý trong Coroutine)
                break;

            case FSMState.Jump:
                // Thực hiện vật lý nhảy (được xử lý trong Coroutine)
                break;
        }
    }

    // 📏 Theo dõi khoảng cách và kích hoạt chuyển trạng thái FSM tại mốc 1/3
    void MonitorDistance()
    {
        if (destination == null || hasTriggeredAction) return;

        float remainingDistance = Vector3.Distance(transform.position, destination.position);
        float percentageCompleted = 1f - (remainingDistance / totalDistance);

        // Nếu chúng ta đã đi được 1/3 (33%) quãng đường
        if (percentageCompleted >= 0.33f)
        {
            hasTriggeredAction = true;
            currentState = FSMState.ActionTriggered;
            TriggerRandomAction();
        }
    }

    void TriggerRandomAction()
    {
        // Chọn ngẫu nhiên 0 (Tăng tốc) hoặc 1 (Nhảy)
        int choice = Random.Range(0, 2);

        if (choice == 0)
        {
            StartCoroutine(SpeedBoostRoutine());
        }
        else
        {
            StartCoroutine(JumpRoutine());
        }
    }

    // TRẠNG THÁI TĂNG TỐC: Nhân đôi tốc độ trong 2 giây
    IEnumerator SpeedBoostRoutine()
    {
        currentState = FSMState.SpeedBoost;

        // 1. Tăng cả tốc độ và gia tốc lên mức tối đa ngay lập tức!
        agent.acceleration = 9999f;          // Gia tốc tức thời (không cần tích lũy!)
        agent.speed = normalSpeed * 4.44f;      // Tốc độ gấp 4.44 lần là đủ nhiều với gia tốc tức thời!

        yield return new WaitForSeconds(2f);

        // 2. Khôi phục cả tốc độ và gia tốc trở lại bình thường
        agent.speed = normalSpeed;
        agent.acceleration = normalAcceleration;

        currentState = FSMState.NormalWalk;
    }

    // TRẠNG THÁI NHẢY: Thực hiện một cú nhảy parabol tuyệt đẹp về phía trước
    IEnumerator JumpRoutine()
    {
        currentState = FSMState.Jump;

        // 1. Tạm thời vô hiệu hóa NavMeshAgent để chúng ta có thể kiểm soát chiều cao (trục Y)
        agent.enabled = false;

        Vector3 jumpStart = transform.position;
        // Nhảy 3 mét về phía trước theo hướng quái vật đang đối mặt
        Vector3 jumpEnd = jumpStart + transform.forward * 3f;

        float elapsedTime = 0f;
        float jumpDuration = 1f;  // Cú nhảy mất đúng 1 giây
        float jumpHeight = 2.5f;   // Chiều cao đỉnh của cú nhảy

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Nội suy tuyến tính vị trí nằm ngang (X và Z)
            Vector3 currentPos = Vector3.Lerp(jumpStart, jumpEnd, t);

            // Thêm một đường cong chiều cao parabol vào trục Y
            // Công thức: y = 4 * Chiều cao * t * (1 - t)
            currentPos.y = Mathf.Lerp(jumpStart.y, jumpEnd.y, t) + (4f * jumpHeight * t * (1f - t));

            transform.position = currentPos;
            yield return null;
        }

        transform.position = jumpEnd;

        // 2. Kích hoạt lại NavMeshAgent và tính toán lại đường đi
        agent.enabled = true;
        if (destination != null)
        {
            agent.SetDestination(destination.position);
        }

        currentState = FSMState.NormalWalk;
    }

    // NHẢY OFF-MESH LINK: Tự động kích hoạt khi chạm vào một NavMesh Link
    IEnumerator LinkJumpRoutine()
    {
        currentState = FSMState.Jump;

        // 1. Lấy vị trí bắt đầu và kết thúc của NavMesh Link
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 jumpStart = transform.position;
        Vector3 jumpEnd = data.endPos;

        float elapsedTime = 0f;
        float jumpDuration = 0.8f; // Cú nhảy mất 0.8 giây
        float jumpHeight = 2.0f;   // Chiều cao của cú nhảy

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Di chuyển ngang (X và Z)
            Vector3 currentPos = Vector3.Lerp(jumpStart, jumpEnd, t);

            // Di chuyển đứng (vòng cung parabol trục Y)
            currentPos.y = Mathf.Lerp(jumpStart.y, jumpEnd.y, t) + (4f * jumpHeight * t * (1f - t));

            transform.position = currentPos;
            yield return null;
        }

        transform.position = jumpEnd;

        // 2. Thông báo cho NavMeshAgent rằng chúng ta đã băng qua link thành công!
        agent.CompleteOffMeshLink();

        currentState = FSMState.NormalWalk;
    }
}