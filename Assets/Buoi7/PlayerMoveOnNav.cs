using UnityEngine;
using UnityEngine.AI;

public class PlayerMoveOnNav : MonoBehaviour
{
    public Camera mainCamera; //khai nếu nhỡ xoá Main Camera khỏi scene
    public Animator animator;
    public NavMeshAgent agent;
    RaycastHit hit = new RaycastHit();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Thử fix ko điều khiển đc player di chuyển
        if (agent != null)
        {
            agent.Warp(transform.position); // Forces the agent to snap to the closest NavMesh point
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
                agent.destination = hit.point;
            }
            //Raycast chi tao 1 tia di theo 1 huong nao do, cham vao doi tuong dau tien
            //RaycastAll tao ra 1 mang tat ca cac doi tuong ma tia do di qua
            //ray.direction (huong cua raycast) la VUONG GOC VOI MAN HINH
        }
        // Neu muon dieu khien Animation:
        // if (agent.velocity.sqrMagnitude > 0) //dung sqr de dam bao gia tri ko bi am
        // {
        //     animator.SetBool("run", true);
        // }
        // else
        // {
        //     animator.SetBool("run",false);
        // }
    }
}
