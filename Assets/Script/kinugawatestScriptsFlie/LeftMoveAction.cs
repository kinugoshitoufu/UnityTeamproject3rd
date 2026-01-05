using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LeftMove", story: "Move to Left", category: "Action", id: "74429655c3c5f04dba5b2355124d7fa8")]
public partial class LeftMoveAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        BehaviourTest.Instance.MoveLeft();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

