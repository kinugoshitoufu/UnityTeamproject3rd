using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RightMove", story: "Move to Right", category: "Action", id: "44ea56d658266745018c974fd0f41829")]
public partial class RightMoveAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        BehaviourTest.Instance.MoveRight();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

