using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Tornado2", story: "Tornado2", category: "Action", id: "691f3d576809cd3814dec900a4ab8e20")]
public partial class Tornado2Action : Action
{

    protected override Status OnStart()
    {
        //CoroutineRunner.Instance.StartCoroutine(SnakeScript.SnakeInstance.Tornado2());
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

