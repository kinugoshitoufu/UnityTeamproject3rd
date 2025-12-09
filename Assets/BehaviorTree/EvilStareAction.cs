using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EvilStare", story: "EvilStare", category: "Action", id: "f888f38a7af689b28f0f1a6fc23f9b68")]
public partial class EvilStareAction : Action
{

    protected override Status OnStart()
    {
        CoroutineRunner.Instance.StartCoroutine(SnakeScript.SnakeInstance.EvilStare());
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

