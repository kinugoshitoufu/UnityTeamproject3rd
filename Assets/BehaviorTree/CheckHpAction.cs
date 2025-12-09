using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckHP", story: "CheckHP", category: "Action", id: "6f3286b286096d38c6329f1c3b551d66")]
public partial class CheckHpAction : Action
{

    protected override Status OnStart()
    {
        SnakeScript.SnakeInstance.CheckHP();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

