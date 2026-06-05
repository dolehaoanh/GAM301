using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "XoayObject", story: "Xoay [Object] liên tục [x] độ", category: "Action", id: "6fae375128bc4782b48b6ad70755491c")]
public partial class XoayObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<float> X;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Object.Value.transform.rotation *= Quaternion.AngleAxis(X.Value,Vector3.up);
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

