using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSM : MonoBehaviour
{
    // 1. Define the FSM States
    public enum FSMState
    {
        NormalWalk,
        ActionTriggered,
        SpeedBoost,
        Jump
    }

    [Header("State Machine")]
    public FSMState currentState = FSMState.NormalWalk;

    [Header("Journey Tracking")]
    public Transform destination;
    private Vector3 startPosition;
    private float totalDistance;
    private bool hasTriggeredAction = false;

    private NavMeshAgent agent;
    private float normalSpeed;
    private float normalAcceleration; // ⚡️ Store the normal acceleration

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = agent.speed;
        normalAcceleration = agent.acceleration;

        agent.autoTraverseOffMeshLink = false; // Turn off automatic ghost-gliding

        // Remember where we spawned
        startPosition = transform.position;

        if (destination != null)
        {
            totalDistance = Vector3.Distance(startPosition, destination.position);
            agent.SetDestination(destination.position);
        }
    }

    // Called by the Spawner to set the destination dynamically
    public void InitializeDestination(Transform target)
    {
        destination = target;
        agent = GetComponent<NavMeshAgent>();

        agent.autoTraverseOffMeshLink = false; // Turn off automatic ghost-gliding

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

        // 💡 NEW: If the agent touches a NavMesh Link, instantly jump over it!
        if (agent != null && agent.isOnOffMeshLink && currentState != FSMState.Jump)
        {
            StartCoroutine(LinkJumpRoutine());
        }

        // Continuous state monitoring
        switch (currentState)
        {
            case FSMState.NormalWalk:
                MonitorDistance();
                break;

            case FSMState.ActionTriggered:
                // Deciding what action to take (handled in Coroutine)
                break;

            case FSMState.SpeedBoost:
                // Double speed active (handled in Coroutine)
                break;

            case FSMState.Jump:
                // Performing jump physics (handled in Coroutine)
                break;
        }
    }

    // 📏 Monitors the distance and triggers the FSM transition at the 1/3 mark
    void MonitorDistance()
    {
        if (destination == null || hasTriggeredAction) return;

        float remainingDistance = Vector3.Distance(transform.position, destination.position);
        float percentageCompleted = 1f - (remainingDistance / totalDistance);

        // If we have completed 1/3 (33%) of the path
        if (percentageCompleted >= 0.33f)
        {
            hasTriggeredAction = true;
            currentState = FSMState.ActionTriggered;
            TriggerRandomAction();
        }
    }

    void TriggerRandomAction()
    {
        // Randomly choose 0 (Speed Boost) or 1 (Jump)
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

    // ⚡️ SPEED BOOST STATE: Double speed for 2 seconds
    IEnumerator SpeedBoostRoutine()
    {
        currentState = FSMState.SpeedBoost;
        Debug.Log("⚡️ FSM: Transitioned to SPEED BOOST state! Instant zip active.");

        // 1. Boost both speed and acceleration to max instantly!
        agent.acceleration = 9999f;          // Instant acceleration (no build-up!)
        agent.speed = normalSpeed * 4.44f;      // 4x speed is plenty with instant acceleration!

        yield return new WaitForSeconds(2f);

        // 2. Restore both speed and acceleration back to normal
        agent.speed = normalSpeed;
        agent.acceleration = normalAcceleration;

        Debug.Log("🚶‍♂️ FSM: Returning to NORMAL state.");
        currentState = FSMState.NormalWalk;
    }

    // 🦘 JUMP STATE: Performs a beautiful parabolic leap forward
    IEnumerator JumpRoutine()
    {
        currentState = FSMState.Jump;
        Debug.Log("🦘 FSM: Transitioned to JUMP state!");

        // 1. Temporarily disable NavMeshAgent so we can control the height (Y-axis)
        agent.enabled = false;

        Vector3 jumpStart = transform.position;
        // Leap 3 meters forward along the direction the monster is facing
        Vector3 jumpEnd = jumpStart + transform.forward * 3f;

        float elapsedTime = 0f;
        float jumpDuration = 1f;  // Jump takes exactly 1 second
        float jumpHeight = 2.5f;   // The peak height of the jump

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Linearly interpolate the horizontal position (X and Z)
            Vector3 currentPos = Vector3.Lerp(jumpStart, jumpEnd, t);

            // Add a parabolic height curve to the Y axis
            // Formula: y = 4 * Height * t * (1 - t)
            currentPos.y = Mathf.Lerp(jumpStart.y, jumpEnd.y, t) + (4f * jumpHeight * t * (1f - t));

            transform.position = currentPos;
            yield return null;
        }

        transform.position = jumpEnd;

        // 2. Re-enable the NavMeshAgent and recalculate the path
        agent.enabled = true;
        if (destination != null)
        {
            agent.SetDestination(destination.position);
        }

        Debug.Log("🚶‍♂️ FSM: Returning to NORMAL state.");
        currentState = FSMState.NormalWalk;
    }

    // 🦘 OFF-MESH LINK JUMP: Triggers automatically when touching a NavMesh Link
    IEnumerator LinkJumpRoutine()
    {
        currentState = FSMState.Jump;
        Debug.Log("🦘 FSM: Off-Mesh Link detected! Leap initiated.");

        // 1. Get the start and end positions of the NavMesh Link
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 jumpStart = transform.position;
        Vector3 jumpEnd = data.endPos;

        float elapsedTime = 0f;
        float jumpDuration = 0.8f; // Jump takes 0.8 seconds
        float jumpHeight = 2.0f;   // Height of the leap

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Horizontal movement (X and Z)
            Vector3 currentPos = Vector3.Lerp(jumpStart, jumpEnd, t);

            // Vertical movement (Y-axis parabolic arc)
            currentPos.y = Mathf.Lerp(jumpStart.y, jumpEnd.y, t) + (4f * jumpHeight * t * (1f - t));

            transform.position = currentPos;
            yield return null;
        }

        transform.position = jumpEnd;

        // 2. Tell the NavMeshAgent that we have successfully crossed the link!
        agent.CompleteOffMeshLink();

        currentState = FSMState.NormalWalk;
        Debug.Log("🚶‍♂️ FSM: Returned to NORMAL state after crossing link.");
    }
}