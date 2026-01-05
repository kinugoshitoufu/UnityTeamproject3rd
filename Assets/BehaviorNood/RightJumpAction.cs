using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RightJump", story: "RighttoJump", category: "Action", id: "750eec23e37966b5699b628a580429f4")]
public partial class RightJumpAction : Action
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
        Elephant.elephant.RightJump();
        return Elephant.elephant.RightJumpFinished ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

