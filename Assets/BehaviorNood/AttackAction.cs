using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "Attack", category: "Action", id: "745afa6dd76a29a0c90cd0a79b183319")]
public partial class AttackAction : Action
{

    protected override Status OnStart()
    {
        if (Elephant.elephant == null)
        {
            return Status.Failure;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Elephant.elephant.Attack();
        return Elephant.elephant.RightJumpFinished ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

