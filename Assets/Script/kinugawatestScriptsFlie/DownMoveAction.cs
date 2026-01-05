using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DownMove", story: "Move to Down", category: "Action", id: "f96d036c54a89515c4db81fb6532f96e")]
public partial class DownMoveAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        BehaviourTest.Instance.MoveDown();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

