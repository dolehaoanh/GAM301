using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;

    private CharacterController controller;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if(!Object.HasInputAuthority)
        return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        controller.Move(dir * speed * Runner.DeltaTime);
    }
}