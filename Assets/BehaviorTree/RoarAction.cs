using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Roar", story: "Roar", category: "Action", id: "c253dc7bf768b14270b4b0e6b318f116")]
public partial class RoarAction : Action
{

    protected override Status OnStart()
    {
        CoroutineRunner.Instance.StartCoroutine(SnakeScript.SnakeInstance.Roar());
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

