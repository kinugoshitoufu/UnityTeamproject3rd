using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpMove", story: "Move to Up", category: "Action", id: "31d71dd1fdd728d219f288f07e2c72c5")]
public partial class UpMoveAction : Action
{

    protected override Status OnStart()
    {
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        BehaviourTest.Instance.MoveUp();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

