using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LeftJump", story: "Left to Jump", category: "Action", id: "edad4008e3429e5dc40a5ae34e3190a4")]
public partial class LeftJumpAction : Action
{
    protected override Status OnStart()
    {
        if (Elephant.elephant == null)
        {
            return Status.Failure;
        }
        CoroutineRunner.Instance.StartCoroutine(Elephant.elephant.LeftJumpCoroutine());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Elephant.elephant.EventEnd ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

