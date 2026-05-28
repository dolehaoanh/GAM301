using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    [SerializeField] private Transform player;
    private float distance; // khoang cach hien tai so voi Player
    [SerializeField] private float atkDistance; // pham vi tan cong
    NavMeshAgent agent;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        if (distance <= atkDistance)
        {
            agent.isStopped = true;
            animator.SetBool("walk", false);
            animator.SetBool("atk", true);
        }
        else
        {
            agent.isStopped = false;
            animator.SetBool("walk", true);
            animator.SetBool("atk", false);
            agent.destination = player.position;
        }
    }
}
