using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    State currentState;
    public Transform player;
    public Transform[] patrolPoints;
    public int patrolIndex;

    public float chaseDistance = 10f;
    public float atkDistance = 4f;

    NavMeshAgent agent;
    // Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Thiết lập trạng thái ban đầu
        ChangeState(State.Patrol);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle: Idle(); break;
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Attack: Attack(); break;
        }
    }

    void ChangeState(State newState)
    {
        currentState = newState;

        if (currentState == State.Idle)
        {
            // animator.Play("Idle)");
            agent.isStopped = true;
        }
        else if (currentState == State.Patrol)
        {
            agent.isStopped = false;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
        else if (currentState == State.Chase)
        {
            agent.isStopped = false;
        }
        else if (currentState == State.Attack)
        {
            agent.isStopped = true;
        }
    }

    void Idle()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= chaseDistance)
        {
            ChangeState(State.Chase);
        }
    }
    void Patrol()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= chaseDistance)
        {
            ChangeState(State.Chase);
            return;
        }
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length; // vì chỉ có 3 node (0,1,2) nên '% patrolPoints.Length' làm cho giá trị quay lại thành 0 khi vượt quá 2
            ChangeState(State.Patrol);

        }
    }
    void Chase()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= atkDistance)
        {
            ChangeState(State.Attack);
            return;
        }
        if (distance > chaseDistance)
        {
            ChangeState(State.Patrol);
            return;
        }
    }
    void Attack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > atkDistance)
        {
            ChangeState(State.Chase);
            return;
        }

        Debug.Log("Tấn công người chơi!");
    }
}
