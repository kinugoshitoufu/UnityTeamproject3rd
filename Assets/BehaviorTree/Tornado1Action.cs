using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Tornado1", story: "Tornado1", category: "Action", id: "f99edcdf17b0648befb9d84dc8c1be20")]
public partial class Tornado1Action : Action
{
    [SerializeReference] public BlackboardVariable<bool> Variable;
    bool done = false;
    bool first = false;
    protected override Status OnStart()
    {
        Variable.Value = false;
        CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.Wait(SnakeScript.SnakeInstance.Tornado1(), () => Variable.Value = true));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Variable.Value ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

