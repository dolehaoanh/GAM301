using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TanCongPlayer", story: "Bắn [Bullet] từ [SpawnPoint] về phía [Player] mỗi [X] giây", category: "Action", id: "d0e566000eba4144f0969d76f8b8ed5b")]
public partial class TanCongPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Bullet;
    [SerializeReference] public BlackboardVariable<Transform> SpawnPoint;
    [SerializeReference] public BlackboardVariable<Transform> Player;
    [SerializeReference] public BlackboardVariable<float> X;

    public float _x;
    protected override Status OnStart()
    {
        _x = X.Value;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if( _x > 0)
        {
            _x -= Time.deltaTime;
        }
        else
        {
            Shoot();
            _x = X.Value;
        }
        return Status.Running;
    }

    void Shoot()
    {
        Vector3 direction = Player.Value.transform.position - GameObject.transform.position;
        GameObject dan = (GameObject)GameObject.Instantiate(Bullet, GameObject.transform.position, Quaternion.identity); // (GameObject) là gán kiểu đối tượng sẽ sinh ra là kiểu Game Object
        Rigidbody rb = dan.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * 10f;
        Debug.Log("Da ban dan!");
    }
    protected override void OnEnd()
    {
    }
}

